// 相对路径: /PluginServiceRegistrator.cs
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;
using Jellyfin.Plugin.BetterPlayer.Services;
using Jellyfin.Plugin.BetterPlayer.Api;

namespace Jellyfin.Plugin.BetterPlayer
{
    // ✨ IPluginServiceRegistrator 是现代 Jellyfin 插件注册服务的方法
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            // 注册 StartupService：因为它实现了 IScheduledTask，Jellyfin 会在启动时执行它
            serviceCollection.AddSingleton<StartupService>();
            
            // 注册 IApiService：让 Jellyfin 知道 BetterPlayerJsProvider 提供的 API 路由
            serviceCollection.AddTransient<BetterPlayerJsProvider>();
        }
    }
}