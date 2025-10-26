// <copyright file="WebHtmlInjector.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer.Helpers
{
    using System;
    using System.IO;
    using System.Reflection;
    using System.Text.RegularExpressions;
    using Jellyfin.Plugin.BetterPlayer.Api;
    using MediaBrowser.Common.Net;
    using Microsoft.Extensions.Logging;

    public class PatchRequestPayload
    {
        public string? Contents { get; set; }
    }

    public static class WebHtmlInjector
    {
        private static ILogger<Plugin> Logger => Plugin.Instance!.Logger;

        private const string ScriptTagRegex = "<script plugin=\"BetterPlayerPlugin\".*?></script>";
        private const string ClientScriptRoute = "/BetterPlayerPlugin/better_player.js";

        public static string FileTransformer(PatchRequestPayload payload)
        {
            Logger.LogDebug("[BP-DEBUG] Attempting to inject script by using FileTransformation plugin.");

            string scriptElement = GetScriptElement();
            string indexContents = payload.Contents!;

            indexContents = Regex.Replace(indexContents, ScriptTagRegex, string.Empty);
            string regex = Regex.Replace(indexContents, "(</body>)", $"{scriptElement}$1");

            return regex;
        }

        public static void Direct()
        {
            Logger.LogDebug("[BP-DEBUG] Attempting to inject script by changing index.html file directly.");

            // ✨ 修复 CS0122：改为使用 Plugin.cs 中新公开的 PublicApplicationPaths 属性
            var applicationPaths = Plugin.Instance!.PublicApplicationPaths;

            if (string.IsNullOrWhiteSpace(applicationPaths.WebPath))
            {
                return;
            }

            var indexFile = Path.Combine(applicationPaths.WebPath, "index.html");
            if (!File.Exists(indexFile))
            {
                return;
            }

            string indexContents = File.ReadAllText(indexFile);
            string scriptElement = GetScriptElement();

            if (indexContents.Contains(scriptElement))
            {
                Logger.LogDebug("[BP-DEBUG] Found client script injected in {0}", indexFile);
                return;
            }

            indexContents = Regex.Replace(indexContents, ScriptTagRegex, string.Empty);

            int bodyClosing = indexContents.LastIndexOf("</body>", StringComparison.Ordinal);
            if (bodyClosing == -1)
            {
                Logger.LogWarning("[BP-WARN] Could not find closing body tag in {0}", indexFile);
                return;
            }

            indexContents = indexContents.Insert(bodyClosing, scriptElement);
            try
            {
                File.WriteAllText(indexFile, indexContents);
                Logger.LogDebug("[BP-DEBUG] Finished injecting preview script code in {0}", indexFile);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "[BP-ERROR] Encountered exception while writing to {0}", indexFile);
            }
        }

        private static string GetScriptElement()
        {
            NetworkConfiguration networkConfiguration =
                Plugin.Instance!.ConfigurationManager.GetNetworkConfiguration();

            string basePath = string.Empty;
            try
            {
                var configType = networkConfiguration.GetType();
                var basePathField = configType.GetProperty("BaseUrl");
                var confBasePath = basePathField?.GetValue(networkConfiguration)?.ToString()?.Trim('/');

                if (!string.IsNullOrEmpty(confBasePath))
                {
                    basePath = $"/{confBasePath}";
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, "[BP-ERROR] Unable to get base path from config, using '/'.");
            }

            string versionTag = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "1.0.0.0";

            return
                $"<script plugin=\"BetterPlayerPlugin\" version=\"{versionTag}\" src=\"{basePath}{ClientScriptRoute}\"></script>";
        }
    }
}