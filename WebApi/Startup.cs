using System;
using System.Text;
using System.Threading.Tasks;
using H.EF.Core;
using H.EF.Core.Extensions;
using H.EF.Core.Filters;
using H.EF.Core.Helpers;
using H.EF.Core.Options;
using H.EF.Core.Repositories;
using log4net;
using log4net.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Videos.IService;
using Videos.Service;

namespace WebApi
{
    public class Startup
    {
        public static ILoggerRepository repository { get; set; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //初始化log4net
            repository = LogManager.CreateRepository("NETCoreRepository");
            Log4NetHelper.SetConfig(repository, "log4net.config");
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddCors(option => option.AddPolicy("cors", policy => policy.AllowAnyHeader().AllowAnyMethod().AllowCredentials().AllowAnyOrigin()));

            //添加jwt验证：
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(o =>
                {
                    o.Events = new JwtBearerEvents()
                    {
                        OnMessageReceived = context =>
                        {
                            context.Token = context.Request.Query["Authorization"];
                            return Task.CompletedTask;
                        }
                    };
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = "Username",
                        ValidIssuer = "http://localhost:58002",
                        ValidAudience = "api",
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Configuration["SecurityKey"])),

                        /***********************************TokenValidationParameters的参数默认值***********************************/
                        // RequireSignedTokens = true,
                        // SaveSigninToken = false,
                        // ValidateActor = false,
                        // 将下面两个参数设置为false，可以不验证Issuer和Audience，但是不建议这样做。
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        // ValidateIssuerSigningKey = false,
                        // 是否要求Token的Claims中必须包含Expires
                        // RequireExpirationTime = true,
                        // 允许的服务器时间偏移量
                        // ClockSkew = TimeSpan.FromSeconds(300),
                        // 是否验证Token有效期，使用当前时间与Token的Claims中的NotBefore和Expires对比
                        // ValidateLifetime = true
                    };
                });

            services.AddMvc(option =>
            {
                option.Filters.Add(new GlobalExceptionFilter());
            });

            services.Configure<IISOptions>(options =>
            {
                options.ForwardClientCertificate = false;
            });

            services.AddMemoryCache();//启用MemoryCache
            services.Configure<MemoryCacheEntryOptions>(
                    options => options.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)) //设置MemoryCache缓存有效时间为5分钟。
                .Configure<DistributedCacheEntryOptions>(option =>
                    option.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5));//设置Redis缓存有效时间为5分钟。

            return InitIoC(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseCors("cors");

            app.UseAuthentication();//注意添加这一句，启用验证

            app.UseStaticFiles();

            app.UseErrorHandling();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                  name: "PlatRoute",
                  template: "admin/[controller]/[action]/{id?}");
            });
        }
        /// <summary>
        /// IoC初始化
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        private IServiceProvider InitIoC(IServiceCollection services)
        {
            var connectionString = Configuration.GetConnectionString("SqlServer");
            var dbContextOption = new DbContextOption
            {
                ConnectionString = connectionString,
                DbType = DbType.MSSQLSERVER
            };
            IoCContainer.Register(Configuration);//注册配置
            IoCContainer.Register(dbContextOption);//注册数据库配置信息

            IoCContainer.RegisterSingleInstance<IDbContext, UnitDbContext>();
            IoCContainer.RegisterSingleInstance<IUnitRepository, UnitRepository>();
            IoCContainer.RegisterSingleInstance<IVideoManageService, VideoManageService>();
            IoCContainer.Initialize(services);

            return IoCContainer.Build(services);
        }


    }
}
