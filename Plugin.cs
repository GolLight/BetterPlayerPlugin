// 相对路径: /Plugin.cs
using MediaBrowser.Common.Configuration;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Configuration;
using MediaBrowser.Model.Plugins;
using MediaBrowser.Model.Serialization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Jellyfin.Plugin.BetterPlayer.Configuration;

namespace Jellyfin.Plugin.BetterPlayer
{
    // 您的插件主类
    public class Plugin : BasePlugin<PluginConfiguration>, IHasWebPages
    {
        public override string Name => "Better Web Player Extension";
        public override Guid Id => Guid.Parse("b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67");
        public override string Description => "Enhances the Jellyfin web player interface.";
        
        public static Plugin Instance { get; private set; } = null!;
        
        // 供其他服务访问的内部属性
        internal IServerConfigurationManager ConfigurationManager { get; set; }
        internal IApplicationPaths ApplicationPaths { get; set; }
        public ILogger<Plugin> Logger { get; set; } // 设为 public 方便 StartupService 访问

        public Plugin(
            IApplicationPaths applicationPaths,
            IXmlSerializer xmlSerializer,
            ILogger<Plugin> logger,
            IServerConfigurationManager configurationManager)
            : base(applicationPaths, xmlSerializer)
        {
            Instance = this;
            
            ConfigurationManager = configurationManager;
            ApplicationPaths = applicationPaths;
            Logger = logger;
        }

        public IEnumerable<PluginPageInfo> GetPages()
        {
            yield return new PluginPageInfo
            {
                Name = Name,
                EmbeddedResourcePath = GetType().Namespace + ".Configuration.configPage.html"
            };
        }
        
        // ✨ 重写 Dispose 方法，进行反注册清理
        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                UnregisterFileTransformationProviders();
            }
        }

        private void UnregisterFileTransformationProviders()
        {
            Logger.LogInformation("[BP-INFO] Attempting to unregister BetterPlayer transformation.");
            try
            {
                var ftType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == "Jellyfin.Plugin.FileTransformation.PluginInterface");

                if (ftType == null)
                {
                    Logger.LogWarning("[BP-WARN] FileTransformation plugin not found, skipping unregister.");
                    return;
                }

                // 查找反注册方法 (假设参数是 transformation ID)
                var unregisterMethod = ftType.GetMethod("UnregisterTransformation");

                if (unregisterMethod == null)
                {
                    Logger.LogError("[BP-ERROR] FileTransformation plugin lacks UnregisterTransformation method.");
                    return;
                }

                // 传递您的插件 GUID 作为 ID 来反注册
                const string PluginGuid = "b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67"; 
                unregisterMethod.Invoke(null, new object[] { PluginGuid });

                Logger.LogInformation("[BP-INFO] BetterPlayer transformation successfully unregistered [BP-UNREG-OK].");
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "[BP-ERROR] Reflection unregister failed.");
            }
        }
    }
}