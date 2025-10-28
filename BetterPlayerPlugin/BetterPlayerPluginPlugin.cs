// <copyright file="Plugin.cs" company="PlaceholderCompany">
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

    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "Better Player";

        public override Guid Id => Guid.Parse("b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67");

        public override string Description => "The Better Player (Better Web Player Extension) is a Jellyfin plugin based on a generic script injection framework. By injecting the better_player.js script, it aims to deliver an immersive, Bilibili-like experience, comprehensively enhancing keyboard, mouse, and mobile touch controls for highly efficient and intuitive web playback operation.";

        public static Plugin Instance { get; private set; } = null!;

        internal IServerConfigurationManager ConfigurationManager { get; set; }

        public ILogger<Plugin> Logger { get; set; }

        // ✨ 修复 CS0122 根源: BasePlugin.ApplicationPaths 是 protected。
        //    我们通过构造函数注入并存储在一个 public/internal 属性中，
        //    或者直接在 Plugin.cs 中创建一个新属性来暴露它。
        //    最简单且兼容的方式是：在构造函数中保存注入的 IApplicationPaths。
        //    我们将其命名为 **PublicApplicationPaths** 以避免与基类 protected 属性冲突，
        //    并设置为 internal。
        internal IApplicationPaths PublicApplicationPaths { get; } // 新增属性

        public Plugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer,
            ILogger<Plugin> logger,
            IServerConfigurationManager configurationManager)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;

            this.ConfigurationManager = configurationManager;
            this.Logger = logger;

            // ✨ 将注入的路径对象赋值给我们创建的 internal 属性
            this.PublicApplicationPaths = applicationPaths;
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            yield return new PluginPageInfo
            {
                Name = this.Name,
                EmbeddedResourcePath = this.GetType().Namespace + ".Configuration.configPage.html",
            };
        }

        // UnregisterFileTransformationProviders 保持不变
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