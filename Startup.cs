using System;
using MediaBrowser.Common.Plugins;
using MediaBrowser.Model.Logging;
using Jellyfin.Plugin.FileTransformation;
using System.Reflection;

namespace BetterPlayerPlugin
{
    public class Startup : IServerEntryPoint
    {
        private readonly ILogManager _logManager;

        public Startup(ILogManager logManager)
        {
            _logManager = logManager;
        }

        public void Run()
        {
            var ftType = typeof(PluginInterface);
            var regTransformer = ftType.GetMethod("RegisterTransformer", BindingFlags.Public | BindingFlags.Static);
            var regContentProvider = ftType.GetMethod("RegisterContentProvider", BindingFlags.Public | BindingFlags.Static);
            regTransformer?.Invoke(null, new object[] { typeof(WebHtmlTransformer) });
            regContentProvider?.Invoke(null, new object[] { typeof(BetterPlayerJsProvider) });
        }

        public void Dispose() { }
    }
}
