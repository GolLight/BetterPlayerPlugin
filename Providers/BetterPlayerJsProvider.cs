// 相对路径: /Api/BetterPlayerJsProvider.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MediaBrowser.Model.Services;
using Microsoft.Extensions.Logging;

namespace Jellyfin.Plugin.BetterPlayer.Api
{
    public class GetClientScript : IReturn<byte[]>
    {
        [ApiMember(Name = "ClientScript", Description = "Custom script file", Is:-Required = false, Route = "/BetterPlayerPlugin/better_player.js", Method = "GET")]
        public string? Path { get; set; }
    }

    public class BetterPlayerJsProvider : IApiService
    {
        public const string Route = "/BetterPlayerPlugin/better_player.js";

        private readonly ILogger<BetterPlayerJsProvider> _logger;

        public BetterPlayerJsProvider(ILogger<BetterPlayerJsProvider> logger)
        {
            _logger = logger;
        }

        public object Get(GetClientScript request)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using var stream = assembly.GetManifestResourceStream("Jellyfin.Plugin.BetterPlayer.Resources.better_player.js");
                
                if (stream == null)
                {
                    _logger.LogError("[BP-ERROR] 未找到嵌入的 better_player.js 资源。");
                    return new HttpResult(new byte[0], "application/javascript");
                }
                
                using var ms = new MemoryStream();
                stream.CopyTo(ms);
                
                return new HttpResult(ms.ToArray(), "application/javascript");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BP-ERROR] BetterPlayerJsProvider.Get 异常。");
                return new HttpResult(new byte[0], "application/javascript");
            }
        }
    }

    public class HttpResult : IHas;//ContentType
    {
        public HttpResult(byte[] content, string contentType)
        {
            Content = content;
            ContentType = contentType;
        }

        public byte[] Content { get; }
        public string ContentType { get; }

        public void WriteTo(Stream stream)
        {
            stream.Write(Content, 0, Content.Length);
        }
    }
}