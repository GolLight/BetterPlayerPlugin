using System;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Logging;
// 假设您的 BetterPlayerJsProvider 仍然是唯一的脚本提供者
// 并且您的 Startup.cs 中注册了 GetPath() 返回的 "/better_player.js"

namespace Jellyfin.Plugin.BetterPlayer
{
    // ✨ 注意：这个类不实现 IWebHtmlTransformer 接口，依赖 Startup.cs 中的反射调用！
    // 它的方法签名必须与 FileTransformation 插件期望的签名匹配。
    public class WebHtmlTransformer
    {
        private readonly ILogger<WebHtmlTransformer> _logger;
        // 定义脚本的路径和标签，我们使用 BetterPlayerJsProvider 定义的路径
        private const string ScriptPath = "/better_player.js";
        
        // 用于防止重复注入的正则表达式（可选，但推荐）
        // 假设您的脚本标签将包含 plugin="BetterPlayer"
        private const string ScriptTagRegex = "<script.*?src=\"/better_player.js\".*?>.*?</script>";

        public WebHtmlTransformer(ILogger<WebHtmlTransformer> logger)
        {
            _logger = logger;
        }

        // FT 插件要求的签名：返回目标文件。
        // 由于 FT 插件注册时会调用这个方法，必须保留。
        public string TargetFile() => "/web/index.html";
        
        /// <summary>
        /// FT 插件要求的签名：对 HTML 内容进行转换。
        /// </summary>
        /// <param name="originalContent">原始 HTML 内容。</param>
        /// <returns>注入脚本后的 HTML 内容。</returns>
        public string Transform(string originalContent)
        {
            try
            {
                // 1. 定义要注入的脚本标签 (使用 defer 属性)
                var scriptTag = $"<script src=\"{ScriptPath}\" defer></script>";
                string indexContents = originalContent;

                // 2. [可选] 清理旧的脚本标签，防止重复注入 (参考示例代码)
                indexContents = Regex.Replace(indexContents, ScriptTagRegex, "", RegexOptions.IgnoreCase);

                // 3. 找到 </body> 标签的位置
                int bodyClosing = indexContents.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);

                if (bodyClosing != -1)
                {
                    // 4. 在 </body> 之前插入脚本标签
                    var result = indexContents.Insert(bodyClosing, scriptTag);
                    _logger.LogInformation("Successfully injected script tag before </body>.");
                    return result;
                }

                _logger.LogWarning("Could not find closing body tag for injection.");
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