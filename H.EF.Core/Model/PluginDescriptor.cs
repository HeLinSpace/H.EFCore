using System;
using System.IO;
using System.Reflection;

namespace H.EF.Core.Model
{
    /// <summary>
    /// 插件描述
    /// </summary>
    public class PluginDescriptor : IComparable<PluginDescriptor>
    {
        /// <summary>
        /// 插件Key，唯一标识
        /// </summary>
        public virtual string PluginKey { get; set; }

        /// <summary>
        /// Plugin type
        /// </summary>
        public virtual string PluginFileName { get; set; }

        /// <summary>
        /// 模块key
        /// </summary>
        public virtual string ModuleKey { get; set; }

        /// <summary>
        /// Plugin type
        /// </summary>
        public virtual Type PluginType { get; set; }

        /// <summary>
        /// 应用程序shadow的程序集,用于引用
        /// </summary>
        public virtual Assembly ReferencedAssembly { get;  set; }

        /// <summary>
        /// 原始程序集文件
        /// </summary>
        public virtual FileInfo OriginalAssemblyFile { get;  set; }

        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }

        /// <summary>
        /// 带命名空间类名
        /// </summary>
        public virtual string ClassFullName { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public virtual string Version { get; set; }

        /// <summary>
        /// 作者
        /// </summary>
        public virtual string Author { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public virtual int DisplayOrder { get; set; }

        /// <summary>
        /// 是否安装
        /// </summary>
        public virtual bool Installed { get; set; }

        /// <summary>
        /// 比较器
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public int CompareTo(PluginDescriptor other)
        {
            return PluginKey.CompareTo(other.PluginKey);
        }

        /// <summary>
        /// 重写tostring方法
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return PluginKey;
        }

        /// <summary>
        /// 重写比较方法
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            var other = obj as PluginDescriptor;
            return other != null &&
                PluginKey != null &&
                PluginKey.Equals(other.PluginKey);
        }

        /// <summary>
        /// 重写获取hascode方法
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return PluginKey.GetHashCode();
        }
    }
}
