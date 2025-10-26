using System;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlayer// 确保这个命名空间与您的项目一致
{
    public class WebHtmlTransformer
    {
        private readonly ILogger<WebHtmlTransformer> _logger;

        public WebHtmlTransformer(ILogger<WebHtmlTransformer> logger)
        {
            _logger = logger;
        }

        public string TargetFile() => "/web/index.html";

        // FT 插件要求的签名
        public string Transform(string originalContent)
        {
            try
            {
                var scriptTag = "<script src=\"/better_player.js\" defer></script>";
                var idx = originalContent.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);
                if (idx >= 0)
                {
                    var result = originalContent.Insert(idx, scriptTag);
                    return result;
                }
                return originalContent;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebHtmlTransformer.Transform 异常");
                return originalContent;
            }
        }
    }
}
