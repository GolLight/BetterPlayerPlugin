using MediaBrowser.Common.Plugins;
using MediaBrowser.Controller.Plugins;
using System;

namespace BetterPlayerPlugin
{
    public class Plugin : BasePlugin, IPlugin
    {
        public override string Name => "Better Web Player Extension";
        public override Guid Id => Guid.Parse("b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67");

        public Plugin(IApplicationPaths applicationPaths)
            : base(applicationPaths)
        {
        }
    }
}
