using System;
using System.IO;
using System.Reflection;
using MediaBrowser.Model.Logging;

namespace BetterPlayerPlugin
{
    public class BetterPlayerJsProvider
    {
        private readonly ILogger _logger;

        public BetterPlayerJsProvider(ILogManager logManager)
        {
            _logger = logManager.CreateLogger(nameof(BetterPlayerJsProvider));
        }

        public string GetPath() => "/better_player.js";

        // FT 插件要求的签名
        public byte[] GetContent()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("BetterPlayerPlugin.Resources.better_player.js");
                if (stream == null)
                {
                    _logger.Error("未找到嵌入的 better_player.js 资源。");
                    return Array.Empty<byte>();
                }
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                _logger.Error($"BetterPlayerJsProvider.GetContent 异常: {ex}");
                return Array.Empty<byte>();
            }
        }

        public string GetContentType() => "application/javascript";
    }
}
