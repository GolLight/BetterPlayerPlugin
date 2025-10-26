// 相对路径: /Transformers/WebHtmlTransformer.cs (假设您将此类放在 Transformers 文件夹)
using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlayer.Transformers
{
    // ✨ 注意：类现在位于 Jellyfin.Plugin.BetterPlayer.Transformers 命名空间
    public class WebHtmlTransformer
    {
        private readonly ILogger<WebHtmlTransformer> _logger;
        private const string ScriptPath = "/better_player.js";
        private const string ScriptTagRegex = "<script.*?src=\"/better_player.js\".*?>.*?</script>";

        public WebHtmlTransformer(ILogger<WebHtmlTransformer> logger)
        {
            _logger = logger;
        }

        // FT 插件要求的方法，用于判断要转换哪个文件。
        // 在 RegisterTransformation 模式下，这个方法被 fileNamePattern 替代，但保留此签名以防万一。
        public string TargetFile() => "/web/index.html"; 
        
        /// <summary>
        /// FT 插件将调用的转换方法。
        /// </summary>
        public string Transform(string originalContent)
        {
            try
            {
                var scriptTag = $"<script src=\"{ScriptPath}\" defer></script>";
                string indexContents = originalContent;

                // 清理旧的脚本标签
                indexContents = Regex.Replace(indexContents, ScriptTagRegex, "", RegexOptions.IgnoreCase);

                int bodyClosing = indexContents.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);

                if (bodyClosing != -1)
                {
                    var result = indexContents.Insert(bodyClosing, scriptTag);
                    _logger.LogInformation("BetterPlayer Injection SUCCESS [BP-INJ-101]");
                    return result;
                }

                _logger.LogWarning("BetterPlayer Injection FAILED: </body> not found [BP-INJ-102]");
                return indexContents;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "WebHtmlTransformer.Transform 异常");
                return originalContent;
            }
        }
    }
}