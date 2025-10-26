using MediaBrowser.Model.Plugins; // IHasWebPages, PluginPageInfo 接口所在
using MediaBrowser.Model.Serialization; // IXmlSerializer 接口所在
using MediaBrowser.Common.Configuration; // IApplicationPaths 接口所在
using MediaBrowser.Common.Plugins; // BasePlugin 所在
using System;
using System.Collections.Generic;
using Jellyfin.Plugin.BetterPlayer.Configuration;

namespace Jellyfin.Plugin.BetterPlayer// 确保这个命名空间与您的项目一致
{
    /// <summary>
    /// Better Player 插件的主类。
    /// </summary>
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        // 确保您的配置类命名空间被正确引用
        
        /// <summary>
        /// Gets the current plugin instance.
        /// </summary>
        public static Plugin? Instance { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Plugin"/> class.
        /// </summary>
        // 注意：这里需要确保 IApplicationPaths 和 IXmlSerializer 在 .csproj 中被正确引用（MediaBrowser.Common/Model）
        public Plugin(IApplicationPaths applicationPaths, IXmlSerializer xmlSerializer)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
        }
        
        /// <inheritdoc />
        public override string Name => "Better Web Player Extension";

        /// <inheritdoc />
        public override Guid Id => Guid.Parse("b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67"); // 您的插件 GUID

        /// <inheritdoc />
        public IEnumerable<PluginPageInfo> GetPages()
        {
            // 简化 EmbeddedResourcePath 的引用方式
            return
            [
                new PluginPageInfo
                {
                    Name = Name,
                    // 确保这里的路径与您的 .csproj 中的 <EmbeddedResource> 路径匹配
                    EmbeddedResourcePath = $"{GetType().Namespace}.Configuration.configPage.html" 
                }
            ];
        }
    }
}
