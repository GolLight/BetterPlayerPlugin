// <copyright file="WebHtmlTransformer.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer.Transformers
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.Extensions.Logging;

    // ✨ 注意：类现在位于 Jellyfin.Plugin.BetterPlayer.Transformers 命名空间
    public class WebHtmlTransformer
    {
        private readonly ILogger<WebHtmlTransformer> logger;
        private const string ScriptPath = "/better_player.js";
        private const string ScriptTagRegex = "<script.*?src=\"/better_player.js\".*?>.*?</script>";

        public WebHtmlTransformer(ILogger<WebHtmlTransformer> logger)
        {
            this.logger = logger;
        }

        // FT 插件要求的方法，用于判断要转换哪个文件。
        // 在 RegisterTransformation 模式下，这个方法被 fileNamePattern 替代，但保留此签名以防万一。
        public string TargetFile() => "/web/index.html";

        /// <summary>
        /// FT 插件将调用的转换方法。.
        /// </summary>
        /// <returns></returns>
        public string Transform(string originalContent)
        {
            try
            {
                var scriptTag = $"<script src=\"{ScriptPath}\" defer></script>";
                string indexContents = originalContent;

                // 清理旧的脚本标签
                indexContents = Regex.Replace(indexContents, ScriptTagRegex, string.Empty, RegexOptions.IgnoreCase);

                int bodyClosing = indexContents.LastIndexOf("</body>", StringComparison.OrdinalIgnoreCase);

                if (bodyClosing != -1)
                {
                    var result = indexContents.Insert(bodyClosing, scriptTag);
                    this.logger.LogInformation("BetterPlayer Injection SUCCESS [BP-INJ-101]");
                    return result;
                }

                this.logger.LogWarning("BetterPlayer Injection FAILED: </body> not found [BP-INJ-102]");
                return indexContents;
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "WebHtmlTransformer.Transform 异常");
                return originalContent;
            }
        }
    }
}