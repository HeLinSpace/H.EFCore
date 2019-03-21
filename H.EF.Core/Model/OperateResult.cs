namespace H.EF.Core.Model
{
    /// <summary>
    /// OperateResult
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OperateResult<T>: OperateResult
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// OperateResultPage
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class OperateResultPage<T>: OperateResultPage
    {
        /// <summary>
        /// 数据
        /// </summary>
        public T Rows { get; set; }
    }

    /// <summary>
    /// OperateResultPage
    /// </summary>
    public class OperateResultPage
    {
        /// <summary>
        /// 数据
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// records
        /// </summary>
        public int Records { get; set; }

        /// <summary>
        /// records
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public OperateStatus Status { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class OperateResult
    {
        /// <summary>
        /// 状态
        /// </summary>
        public OperateStatus Status { get; set; }
    }

    /// <summary>
    /// 操作状态
    /// </summary>
    public enum OperateStatus
    {
        /// <summary>
        /// 失败
        /// </summary>
        Error = 0,

        /// <summary>
        /// 成功
        /// </summary>
        Success
    }
}