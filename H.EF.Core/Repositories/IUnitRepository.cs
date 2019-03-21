using H.EF.Core.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace H.EF.Core.Repositories
{
    /// <summary>
    /// 仓储接口
    /// </summary>
    public interface IUnitRepository : IDisposable
    {
        /// <summary>
        /// 查询满足条件的第一个
        /// </summary>
        /// <typeparam name="TEntity">返回值类型</typeparam>
        /// <param name="criteria">查询条件</param>
        /// <returns>查询结果</returns>
        TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class;

        /// <summary>
        /// 根据ID查找
        /// </summary>
        /// <typeparam name="TEntity">返回值类型</typeparam>
        /// <param name="id">主键值</param>
        /// <returns>查询结果</returns>
        TEntity FindById<TEntity>(string id) where TEntity : class;

        /// <summary>
        /// 追加对象
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entity">追加对象</param>
        /// <param name="isCommit"></param>
        void Insert<TEntity>(TEntity entity, bool isCommit = false) where TEntity : class;

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entity">需要更新的实体</param>
        void Update<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// EF扩展更新
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="updateExpression">更新表达式</param>
        /// <param name="criteria">查询表达式</param>
        /// <returns></returns>        
        int UpdateByExpression<TEntity>(Expression<Func<TEntity, TEntity>> updateExpression,
            Expression<Func<TEntity, bool>> criteria = null) where TEntity : class;

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entity">需要删除的实体</param>
        void Delete<TEntity>(TEntity entity) where TEntity : class;

        /// <summary>
        /// EF扩展删除
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="criteria">查询表达式</param>
        /// <returns></returns>
        int DeleteByExpression<T>(Expression<Func<T, bool>> criteria = null) where T : class;

        /// <summary>
        /// EF扩展删除
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="criteria">查询表达式</param>
        /// <returns></returns>
        int DeleteByExpressionCommit<T>(Expression<Func<T, bool>> criteria = null) where T : class;

        /// <summary>
        /// 表接口
        /// </summary>
        /// <typeparam name="TEntity">查询类型</typeparam>
        ///  <param name="criteria">条件表达式</param>
        /// <returns>当前类型的表接口</returns>
        IQueryable<TEntity> All<TEntity>(Expression<Func<TEntity, bool>> criteria = null) where TEntity : class;

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity">查询类型</typeparam>
        /// <param name="pageQuery"></param>
        ///  <param name="criteria">条件表达式</param>
        /// <returns>OperateResultPage</returns>
        OperateResultPage<IQueryable<TEntity>> QueryWithPage<TEntity>(PageQuery pageQuery, Expression<Func<TEntity, bool>> criteria = null) where TEntity : class;

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="TEntity">查询类型</typeparam>
        ///  <param name="criteria">条件表达式</param>
        /// <returns>当前条件是否存在</returns>
        bool Any<TEntity>(Expression<Func<TEntity, bool>> criteria = null) where TEntity : class;

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entities">插入列表</param>
        void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class;

    }
}
