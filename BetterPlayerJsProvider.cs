using System.IO;
using System.Reflection;
using MediaBrowser.Controller.Plugins;

namespace BetterPlayerPlugin
{
    public class BetterPlayerJsProvider : IContentProvider
    {
        public string GetPath() => "/better_player.js";

        public Stream GetContent()
        {
            var assembly = Assembly.GetExecutingAssembly();
            return assembly.GetManifestResourceStream("BetterPlayerPlugin.Resources.better_player.js");
        }
    }
}
