using System;
using MediaBrowser.Model.Logging;

namespace BetterPlayerPlugin
{
    public class WebHtmlTransformer
    {
        private readonly ILogger _logger;

        public WebHtmlTransformer(ILogManager logManager)
        {
            _logger = logManager.CreateLogger(nameof(WebHtmlTransformer));
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
                _logger.Error($"WebHtmlTransformer.Transform 异常: {ex}");
                return originalContent;
            }
        }
    }
}
