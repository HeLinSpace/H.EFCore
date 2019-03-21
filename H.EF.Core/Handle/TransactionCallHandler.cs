using System;
using System.Linq;
using System.Reflection;
using Castle.DynamicProxy;

namespace H.EF.Core
{
    /// <summary>
    /// 拦截器
    /// </summary>
    public class TransactionInterceptor : IInterceptor
    {
        /// <summary>
        /// 拦截方法
        /// </summary>
        /// <param name="invocation"></param>
        public void Intercept(IInvocation invocation)
        {
            MethodInfo methodInfo = invocation.MethodInvocationTarget;
            if (methodInfo == null)
            {
                methodInfo = invocation.Method;
            }

            TransactionHandlerAttribute transaction =
                methodInfo.GetCustomAttributes<TransactionHandlerAttribute>(true).FirstOrDefault();
            if (transaction != null)
            {
                //实现事务性工作
                invocation.Proceed();
                if (invocation.ReturnValue != null)
                {
                    IoCContainer.Resolve<IDbContext>().Commit();
                }
            }
            else
            {
                // 没有事务时直接执行方法
                invocation.Proceed();
            }
        }
    }

    /// <summary>
    /// 事务属性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true)]
    public class TransactionHandlerAttribute : Attribute
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public TransactionHandlerAttribute()
        {
        }
    }
}