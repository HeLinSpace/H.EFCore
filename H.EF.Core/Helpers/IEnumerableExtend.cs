using System;
using System.Collections.Generic;
using System.Linq;

namespace H.EF.Core
{
    /// <summary>
    /// 列表接口扩展
    /// </summary>
    public static class IEnumerableExtend
    {
        /// <summary>
        /// 判断列表是否为null或者空
        /// </summary>
        /// <typeparam name="T">列表类型</typeparam>
        /// <param name="list">判断源</param>
        /// <returns>判断结果</returns>
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> list)
        {
            return list == null || list.Count() == 0;
        }

        /// <summary>
        /// 返回列表默认值
        /// </summary>
        /// <typeparam name="TEntity">泛型类型</typeparam>
        /// <param name="source">源列表</param>
        /// <returns>列表的第一个对象</returns>
        public static TEntity FirstOrDefaultWithNullList<TEntity>(this IEnumerable<TEntity> source) where TEntity : new()
        {
            if (!source.IsNullOrEmpty())
            {
                return source.FirstOrDefault();
            }

            return new TEntity();
        }

        /// <summary>
        /// 递归查找子集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="criteria"></param>
        /// <param name="selectCurrent"></param>
        /// <param name="parentKey"></param>
        /// <param name="childList"></param>
        public static void RecursionFindChild<T>(this IEnumerable<T> source,
            Func<T, string, bool> criteria,
            Func<T, string> selectCurrent,
            string parentKey,
            List<T> childList)
        {
            var tempList = source.Where(s => criteria(s, parentKey));
            childList.AddRange(tempList);
            foreach (var item in tempList)
            {
                var currentParent = selectCurrent(item);
                RecursionFindChild(source, criteria, selectCurrent, currentParent, childList);
            }
        }
    }
}
