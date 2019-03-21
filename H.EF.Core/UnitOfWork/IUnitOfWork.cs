using System;

namespace H.EF.Core
{
    /// <summary>
    /// 工作单元操作接口
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// 提交并且刷新改变
        /// </summary>
        void CommitAndRefreshChanges();

        /// <summary>
        /// 回滚提交的改变
        /// </summary>
        void RollbackChanges();

        /// <summary>
        /// 数据上下文提交
        /// </summary>
        /// <returns>影响的行数</returns>
        int Commit();
    }
}
