using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging; // ILoggerFactory 就在这里
using Jellyfin.Plugin.BetterPlayer;

namespace Jellyfin.Plugin.BetterPlayer
{
    // 注意：ILoggerFactory 通常不需要泛型参数
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        private readonly ILoggerFactory _loggerFactory; // ✨ 新增：用于创建特定类型的 Logger

        public Startup(ILoggerFactory loggerFactory) // ✨ 修正：请求 ILoggerFactory
        {
            _loggerFactory = loggerFactory;
            // 从工厂创建 Startup 自己的 Logger
            _logger = loggerFactory.CreateLogger<Startup>(); 
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
                    _logger.LogError("FileTransformation 插件未找到，无法注册 WebHtmlTransformer 和 BetterPlayerJsProvider。");
                    return;
                }
                var regTransformer = ftType.GetMethod("RegisterTransformer", BindingFlags.Public | BindingFlags.Static);
                var regContentProvider = ftType.GetMethod("RegisterContentProvider", BindingFlags.Public | BindingFlags.Static);
                if (regTransformer == null || regContentProvider == null)
                {
                    _logger.LogError("FileTransformation 插件缺少必要的注册方法。");
                    return;
                }
                
                // ✨ 修正 CS1503：从工厂创建正确的 ILogger<T> 实例
                var webHtmlLogger = _loggerFactory.CreateLogger<WebHtmlTransformer>();
                var jsProviderLogger = _loggerFactory.CreateLogger<BetterPlayerJsProvider>();

                // 修正 CS0246：如果 BetterPlayerJsProvider 确实在 Jellyfin.Plugin.BetterPlayer 命名空间下，
                // 那么它现在应该能够被解析。

                regTransformer.Invoke(null, new object[] { new WebHtmlTransformer(webHtmlLogger) });
                regContentProvider.Invoke(null, new object[] { new BetterPlayerJsProvider(jsProviderLogger) });
                
                _logger.LogInformation("BetterPlayerPlugin 已成功注册 WebHtmlTransformer 和 BetterPlayerJsProvider。");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BetterPlayerPlugin 启动时发生异常");
            }
        }

        public void Dispose() { }
    }
}