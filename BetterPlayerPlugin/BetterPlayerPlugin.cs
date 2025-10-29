// <copyright file="BetterPlayerPlugin.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using Jellyfin.Plugin.BetterPlayer.Configuration;
    using MediaBrowser.Common.Configuration;
    using MediaBrowser.Common.Plugins;
    using MediaBrowser.Controller.Configuration;
    using MediaBrowser.Model.Plugins;
    using MediaBrowser.Model.Serialization;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// Better Player 插件的主入口点。.
    /// </summary>
    public class BetterPlayerPlugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BetterPlayerPlugin"/> class.
        /// 初始化 <see cref="BetterPlayerPlugin"/> 类的新实例。.
        /// </summary>
        /// <param name="applicationPaths">应用程序路径服务。.</param>
        /// <param name="xmlSerializer">XML 序列化服务。.</param>
        /// <param name="logger">日志记录器服务。.</param>
        /// <param name="configurationManager">服务器配置管理器服务。.</param>
        public BetterPlayerPlugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer,
            ILogger<BetterPlayerPlugin> logger,
            IServerConfigurationManager configurationManager)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;

            this.ConfigurationManager = configurationManager;
            this.Logger = logger;

            this.PublicApplicationPaths = applicationPaths;
        }

        /// <summary>
        /// Gets 获取或设置插件的单例实例。.
        /// </summary>
        public static BetterPlayerPlugin Instance { get; private set; } = null!;

        // ===============================================================
        // C. 非静态属性 (Public 在前, SA1202)
        // ===============================================================

        /// <summary>
        /// Gets 获取插件的唯一名称。.
        /// </summary>
        public override string Name => "Better Player";

        /// <summary>
        /// Gets 获取插件的唯一描述。.
        /// </summary>
        public override string Description => "The Better Player (Better Web Player Extension) is a Jellyfin plugin based on a generic script injection framework. By injecting the better_player.js script, it aims to deliver an immersive, Bilibili-like experience, comprehensively enhancing keyboard, mouse, and mobile touch controls for highly efficient and intuitive web playback operation.";

        /// <summary>
        /// Gets 获取插件的唯一 ID。.
        /// </summary>
        public override Guid Id => Guid.Parse("b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67");

        /// <summary>
        /// Gets or sets 获取或设置插件的日志记录器。.
        /// </summary>
        public ILogger<BetterPlayerPlugin> Logger { get; set; }

        // Internal Properties (SA1202: internal 在 public 之后)

        /// <summary>
        /// Gets or sets 获取或设置服务器配置管理器。.
        /// </summary>
        internal IServerConfigurationManager ConfigurationManager { get; set; }

        /// <summary>
        /// Gets 获取应用程序路径，用于访问文件系统。.
        /// </summary>
        internal IApplicationPaths PublicApplicationPaths { get; }

        // ===============================================================
        // D. 方法
        // ===============================================================

        /// <summary>
        /// 获取插件配置页面信息。.
        /// </summary>
        /// <returns>插件配置页面信息列表。.</returns>
        public IEnumerable<PluginPageInfo> GetPages()
        {
            yield return new PluginPageInfo
            {
                Name = this.Name,
                EmbeddedResourcePath = this.GetType().Namespace + ".Configuration.configPage.html",
            };
        }

        /// <summary>
        /// 注销 FileTransformationProvider。.
        /// </summary>
        private void UnregisterFileTransformationProviders()
        {
            this.Logger.LogInformation("[BP-INFO] Attempting to unregister BetterPlayer transformation.");
            try
            {
                var ftType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == "Jellyfin.Plugin.FileTransformation.PluginInterface");

                if (ftType == null)
                {
                    this.Logger.LogWarning("[BP-WARN] FileTransformation plugin not found, skipping unregister.");
                    return;
                }

                var unregisterMethod = ftType.GetMethod("UnregisterTransformation");

                if (unregisterMethod == null)
                {
                    this.Logger.LogError("[BP-ERROR] FileTransformation plugin lacks UnregisterTransformation method.");
                    return;
                }

                const string PluginGuid = "b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67";
                unregisterMethod.Invoke(null, new object[] { PluginGuid });

                this.Logger.LogInformation("[BP-INFO] BetterPlayer transformation successfully unregistered [BP-UNREG-OK].");
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex, "[BP-ERROR] Reflection unregister failed.");
            }
        }
    }
}
