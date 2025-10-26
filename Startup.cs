using System;
using System.Linq;
using System.Reflection;
using MediaBrowser.Model.Logging;

namespace BetterPlayerPlugin
{
    public class Startup : IServerEntryPoint
    {
        private readonly ILogManager _logManager;
        private readonly ILogger _logger;

        public Startup(ILogManager logManager)
        {
            _logManager = logManager;
            _logger = _logManager.CreateLogger(nameof(Startup));
        }

        public void Run()
        {
            try
            {
                var ftType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(a => a.GetTypes())
                    .FirstOrDefault(t => t.FullName == "Jellyfin.Plugin.FileTransformation.PluginInterface");
                if (ftType == null)
                {
                    _logger.Error("FileTransformation 插件未找到，无法注册 WebHtmlTransformer 和 BetterPlayerJsProvider。");
                    return;
                }
                var regTransformer = ftType.GetMethod("RegisterTransformer", BindingFlags.Public | BindingFlags.Static);
                var regContentProvider = ftType.GetMethod("RegisterContentProvider", BindingFlags.Public | BindingFlags.Static);
                if (regTransformer == null || regContentProvider == null)
                {
                    _logger.Error("FileTransformation 插件缺少必要的注册方法。");
                    return;
                }
                regTransformer.Invoke(null, new object[] { new WebHtmlTransformer(_logManager) });
                regContentProvider.Invoke(null, new object[] { new BetterPlayerJsProvider(_logManager) });
                _logger.Info("BetterPlayerPlugin 已成功注册 WebHtmlTransformer 和 BetterPlayerJsProvider。");
            }
            catch (Exception ex)
            {
                _logger.Error($"BetterPlayerPlugin 启动时发生异常: {ex}");
            }
        }

        public void Dispose() { }
    }
}
