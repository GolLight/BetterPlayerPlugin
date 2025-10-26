using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlayer// 确保这个命名空间与您的项目一致
{
    public class BetterPlayerJsProvider
    {
        private readonly ILogger<BetterPlayerJsProvider> _logger;

        public BetterPlayerJsProvider(ILogger<BetterPlayerJsProvider> logger)
        {
            _logger = logger;
        }

        public string GetPath() => "/better_player.js";

        // FT 插件要求的签名
        public byte[] GetContent()
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                // ✨ 修正：将资源路径更新为新的根命名空间
                using var stream = assembly.GetManifestResourceStream("Jellyfin.Plugin.BetterPlayer.Resources.better_player.js");
                if (stream == null)
                {
                    _logger.LogError("未找到嵌入的 better_player.js 资源。");
                    return Array.Empty<byte>();
                }
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                return ms.ToArray();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "BetterPlayerJsProvider.GetContent 异常");
                return Array.Empty<byte>();
            }
        }

        public string GetContentType() => "application/javascript";
    }
}
