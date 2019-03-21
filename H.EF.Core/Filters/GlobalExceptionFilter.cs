using Microsoft.AspNetCore.Mvc.Filters;
using H.EF.Core.Helpers;
using Microsoft.AspNetCore.Mvc;
using H.EF.Core.Model;

namespace H.EF.Core.Filters
{
    /// <summary>
    /// 全局异常捕捉
    /// </summary>
    public class GlobalExceptionFilter : IExceptionFilter
    {
        /// <summary>
        /// 发生错误记录日志
        /// </summary>
        /// <param name="filterContext"></param>
        public void OnException(ExceptionContext filterContext)
        {
            var type = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType;
            //Log4NetHelper.WriteError(type, filterContext.Exception);
            ExceptionResult data = new ExceptionResult();
            data.MessageType = filterContext.Exception.GetType().Name;
            data.Messagecontent = filterContext.Exception.Message;
            data.ExceptionMethod = filterContext.Exception.TargetSite.ToString();
            data.ExceptionSource = filterContext.Exception.Source + filterContext.Exception.StackTrace;

            filterContext.Result = new JsonResult(new OperateResult<ExceptionResult>() { Data = data, Status = OperateStatus.Success });
        }
    }
}
