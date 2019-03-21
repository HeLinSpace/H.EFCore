using System;
using System.Collections.Generic;
using H.EF.Core.Model;

namespace H.EF.Core
{
    /// <summary>
    /// 插件提供器接口
    /// </summary>
    public interface IPluginProvider
    {
        /// <summary>
        /// 初始化插件
        /// </summary>
        void Initialize();

        /// <summary>
        /// 通过提供类型查找插件描述
        /// </summary>
        /// <param name="providerType">提供类型</param>
        /// <returns>插件描述</returns>
        PluginDescriptor FindPlugin(Type providerType);

        /// <summary>
        /// 引用的插件列表
        /// </summary>
        IEnumerable<PluginDescriptor> ReferencedPlugins { get; set; }

        /// <summary>
        /// 版本不支持的插件列表
        /// </summary>
        IEnumerable<string> IncompatiblePlugins { get; set; }
    }
}
