using System.IO;
using System.Text;
using MediaBrowser.Controller.Plugins;

namespace BetterPlayerPlugin
{
    public class WebHtmlTransformer : IFileTransformer
    {
        public string TargetFile() => "/web/index.html";

        public Stream Transform(Stream input)
        {
            using var reader = new StreamReader(input, Encoding.UTF8);
            var html = reader.ReadToEnd();
            var scriptTag = "<script src=\"/better_player.js\" defer></script>";
            var idx = html.LastIndexOf("</body>", System.StringComparison.OrdinalIgnoreCase);
            if (idx >= 0)
            {
                html = html.Insert(idx, scriptTag);
            }
            return new MemoryStream(Encoding.UTF8.GetBytes(html));
        }
    }
}
