namespace H.EF.Core
{
    /// <summary>
    /// DB上下文接口
    /// </summary>
    public interface IDbContext : IUnitOfWork
    {
        /// <summary>
        /// 获取表对象
        /// </summary>
        /// <typeparam name="TEntity">返回类型（类型必须和数据库对应，并且在Map文件设定映射关系）</typeparam>
        /// <returns>表对象接口</returns>
        Microsoft.EntityFrameworkCore.DbSet<TEntity> Set<TEntity>()
            where TEntity : class;

        /// <summary>
        /// 附件对象到上下文
        /// </summary>
        /// <typeparam name="TEntity">附加类型</typeparam>
        /// <param name="item">附加对象 <</param>
        void Attach<TEntity>(TEntity item) where TEntity : class;

        /// <summary>
        /// 设定对象状态为更新状态
        /// </summary>
        /// <typeparam name="TEntity">更新类型</typeparam>
        /// <param name="item">更新对象</param>
        void SetModified<TEntity>(TEntity item) where TEntity : class;

        /// <summary>
        /// 替换当前值 <paramref name="original"/>
        /// </summary>
        /// <typeparam name="TEntity">替换类型</typeparam>
        /// <param name="original">旧对象</param>
        /// <param name="current">当前对象</param>
        void ApplyCurrentValues<TEntity>(TEntity original, TEntity current) where TEntity : class;
    }
}
