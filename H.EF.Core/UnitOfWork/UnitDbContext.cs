using System;
using System.Linq;
using Z.EntityFramework.Plus;
using Microsoft.EntityFrameworkCore;
using H.EF.Core.Options;
using H.EF.Core.Helpers;

namespace H.EF.Core
{
    /// <summary>
    /// 工作单元上下文
    /// </summary>
    public class UnitDbContext : DbContext, IDbContext
    {
        private DbContextOption _option;

        /// <summary>
        /// ctor
        /// </summary>
        /// <param name="connectString"></param>
        public UnitDbContext(DbContextOption option)
        {
            if (option == null)
                throw new ArgumentNullException(nameof(option));
            if (string.IsNullOrEmpty(option.ConnectionString))
                throw new ArgumentNullException(nameof(option.ConnectionString));
            _option = option;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            switch (_option.DbType)
            {
                case DbType.ORACLE:
                    throw new NotSupportedException("Oracle EF Core Database Provider is not yet available.");
                case DbType.MYSQL:
                    optionsBuilder.UseMySQL(_option.ConnectionString);
                    break;
                case DbType.SQLITE:
                    optionsBuilder.UseSqlite(_option.ConnectionString);
                    break;
                case DbType.MEMORY:
                    optionsBuilder.UseInMemoryDatabase(_option.ConnectionString);
                    break;
                case DbType.NPGSQL:
                    //optionsBuilder.UseNpgsql(_option.ConnectionString);
                    break;
                default:
                    optionsBuilder.UseSqlServer(_option.ConnectionString);
                    break;
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // 加载数据库实体
            LoadEntities(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// 加载数据库实体
        /// </summary>
        /// <param name="modelBuilder"></param>
        private void LoadEntities(ModelBuilder modelBuilder)
        {
            AssemblyHelpers _assemblyHelper = new AssemblyHelpers();
            var assemblys = _assemblyHelper.GetAssemblies();
            foreach (var assembly in assemblys)
            {
                var types = assembly.GetTypes();
                var assignTypeFrom = typeof(IBaseModel<>);
                var currentTypes = types.Where(t => t.IsClass && !t.IsGenericType && !t.IsAbstract && (assignTypeFrom.IsAssignableFrom(t) ||
                            (assignTypeFrom.IsGenericTypeDefinition && _assemblyHelper.DoesTypeImplementOpenGeneric(t, assignTypeFrom))) &&
                            !t.IsInterface);
                foreach (var type in currentTypes)
                {
                    if (modelBuilder.Model.FindEntityType(type) == null)
                        modelBuilder.Model.AddEntityType(type);
                }
            }
        }


        public new DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return base.Set<TEntity>();
        }

        public void CommitAndRefreshChanges()
        {
            bool saveFailed = false;

            do
            {
                try
                {
                    this.SaveChanges();

                    saveFailed = false;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    saveFailed = true;

                    ex.Entries.ToList()
                              .ForEach(entry =>
                              {
                                  entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                              });

                }
            } while (saveFailed);
        }

        public void RollbackChanges()
        {
            base.ChangeTracker.Entries()
                              .ToList()
                              .ForEach(entry => entry.State = EntityState.Unchanged);
        }


        public void Attach<TEntity>(TEntity item) where TEntity : class
        {
            base.Entry<TEntity>(item).State = EntityState.Unchanged;
        }

        public void SetModified<TEntity>(TEntity item) where TEntity : class
        {
            base.Entry<TEntity>(item).State = EntityState.Modified;
        }

        public void ApplyCurrentValues<TEntity>(TEntity original, TEntity current) where TEntity : class
        {
            base.Entry<TEntity>(original).CurrentValues.SetValues(current);
        }

        public int Commit()
        {
            return base.SaveChanges();
        }
        //public new void Dispose()
        //{
        //    Dispose(true);
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    base.Dispose(disposing);
        //}
    }
}
