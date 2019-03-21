namespace H.EF.Core.Model
{
    /// <summary>
    /// 分页查询模型
    /// </summary>
    public class PageQuery : BaseQuery
    {
        /// <summary>
        /// 查询页
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// 查询条数
        /// </summary>
        public int Rows { get; set; } = 30;

        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sidx { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public string Sord { get; set; } = "desc";
    }
}
