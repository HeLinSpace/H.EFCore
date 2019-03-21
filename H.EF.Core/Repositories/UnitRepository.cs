using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Z.EntityFramework.Plus;
using H.EF.Core.Model;

namespace H.EF.Core.Repositories
{
    /// <summary>
    /// 工作单元仓储
    /// </summary>
    public class UnitRepository : IUnitRepository
    {
        private readonly IDbContext _context;

        #region Ctor
        /// <summary>
        /// 实例化方法
        /// </summary>
        /// <param name="context"></param>
        public UnitRepository(IDbContext context)
        {
            this._context = context;
        }
        #endregion

        #region Methods
        /// <summary>
        /// 查询满足条件的第一个
        /// </summary>
        /// <typeparam name="TEntity">返回值类型</typeparam>
        /// <param name="criteria">查询条件</param>
        /// <returns>查询结果</returns>
        public TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> criteria) where TEntity : class
        {
            return _context.Set<TEntity>().FirstOrDefault(criteria);
        }

        /// <summary>
        /// 根据ID查找
        /// </summary>
        /// <typeparam name="TEntity">返回值类型</typeparam>
        /// <param name="id">主键值</param>
        /// <returns>查询结果</returns>
        public TEntity FindById<TEntity>(string id) where TEntity : class
        {
            return _context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// 追加对象
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entity">追加对象</param>
        public void Insert<TEntity>(TEntity entity, bool isCommit = false) where TEntity : class
        {
            if (entity == null)
            {
                return;
            }
            _context.Set<TEntity>().Add(entity);

            _context.Commit();
        }

        /// <summary>
        /// 更新对象
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entity">需要更新的实体</param>
        public void Update<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return;
            }
            _context.Set<TEntity>().Attach(entity);
            _context.SetModified(entity);
            _context.Commit();
        }

        /// <summary>
        /// EF扩展更新
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="updateExpression">更新表达式</param>
        /// <param name="criteria">查询表达式</param>
        /// <returns></returns>      
        public int UpdateByExpression<TEntity>(Expression<Func<TEntity, TEntity>> updateExpression,
            Expression<Func<TEntity, bool>> criteria = null) where TEntity : class
        {
            var source = this.All<TEntity>(criteria);
            return source.Update(updateExpression);
        }

        /// <summary>
        /// 删除对象
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entity">需要删除的实体</param>
        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            if (entity == null)
            {
                return;
            }
            this._context.Set<TEntity>().Remove(entity);
            _context.Commit();
        }

        /// <summary>
        /// EF扩展删除
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="criteria">查询表达式</param>
        /// <returns></returns>
        public int DeleteByExpression<T>(Expression<Func<T, bool>> criteria = null) where T : class
        {
            var source = _context.Set<T>();
            if (criteria == null)
            {
                return _context.Set<T>().Delete();
            }
            return source.Where(criteria).Delete();
        }

        /// <summary>
        /// EF扩展删除
        /// </summary>
        /// <typeparam name="T">参数类型</typeparam>
        /// <param name="criteria">查询表达式</param>
        /// <returns></returns>
        public int DeleteByExpressionCommit<T>(Expression<Func<T, bool>> criteria = null) where T : class
        {
            int result = 0;
            var source = _context.Set<T>();
            if (criteria == null)
            {
                result = _context.Set<T>().Delete();
            }
            else
            {
                if (source.Where(criteria).Count() > 0)
                {
                    return source.Where(criteria).Delete();
                }
            }

            _context.Commit();

            return result;
        }

        /// <summary>
        /// 表接口
        /// </summary>
        /// <typeparam name="TEntity">查询类型</typeparam>
        ///  <param name="criteria">条件表达式</param>
        /// <returns>当前类型的表接口</returns>
        public IQueryable<TEntity> All<TEntity>(Expression<Func<TEntity, bool>> criteria = null) where TEntity : class
        {
            return criteria == null ? _context.Set<TEntity>() : _context.Set<TEntity>().Where(criteria);
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <typeparam name="TEntity">查询类型</typeparam>
        ///  <param name="pageSize">查询条数</param>
        ///  <param name="pageNo">当前页</param>
        ///  <param name="criteria">条件表达式</param>
        /// <returns>OperateResultPage</returns>
        public OperateResultPage<IQueryable<TEntity>> QueryWithPage<TEntity>(PageQuery pageQuery, Expression<Func<TEntity, bool>> criteria = null) where TEntity : class
        {
            var result = new OperateResultPage<IQueryable<TEntity>>();

            var data = criteria == null ? _context.Set<TEntity>() : _context.Set<TEntity>().Where(criteria);

            //if (!string.IsNullOrEmpty(pageQuery.Sidx))
            //{
            //    data.OrderBy(pageQuery.Sidx,pageQuery.Sord);
            //}

            result.Page = pageQuery.Page;
            result.Status = OperateStatus.Success;
            result.Records = data.Count();
            result.Rows = data.Skip((pageQuery.Page - 1) * pageQuery.Rows).Take(pageQuery.Rows);
            result.Total = data.Count() % pageQuery.Rows > 0 ? 1 + data.Count() / pageQuery.Rows : data.Count() / pageQuery.Rows;


            return result;
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <typeparam name="TEntity">查询类型</typeparam>
        ///  <param name="criteria">条件表达式</param>
        /// <returns>当前条件是否存在</returns>
        public bool Any<TEntity>(Expression<Func<TEntity, bool>> criteria = null) where TEntity : class
        {
            return All(criteria).Select(s => 1).Distinct().Any();
        }

        /// <summary>
        /// 批量插入
        /// </summary>
        /// <typeparam name="TEntity">参数类型</typeparam>
        /// <param name="entities">插入列表</param>
        public void BulkInsert<TEntity>(IEnumerable<TEntity> entities) where TEntity : class
        {
            if (entities.IsNullOrEmpty())
            {
                return;
            }
            foreach (var entitieItem in entities)
            {
                _context.Set<TEntity>().Add(entitieItem);
            }

            _context.Commit();

            //(_context as DbContext).BulkInsert(entities);
        }
        #endregion

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            _context.Dispose();
            //this.Dispose(true);
            //GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            _context.Commit();
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            _context.Dispose();
        }
    }
}
