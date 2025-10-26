// 相对路径: /PluginServiceRegistrator.cs
using MediaBrowser.Controller.Plugins;
using MediaBrowser.Controller;
using Microsoft.Extensions.DependencyInjection;
using Jellyfin.Plugin.BetterPlayer.Services;
// using Jellyfin.Plugin.BetterPlayer.Api; // IApiService 模式不再需要

namespace Jellyfin.Plugin.BetterPlayer
{
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            // 注册 StartupService (IScheduledTask)
            serviceCollection.AddSingleton<StartupService>();
            
            // ✨ 注册 Controller：使用 AddControllersAsServices 将所有继承自 Controller 的类注册
            // 这通常在 Jellyfin 的启动时自动完成，但显式注册或使用 AddMvc() 也是常见的做法。
            // 最好的方式是依赖 Jellyfin 基础框架自动发现 Controller，
            // 但为了确保，我们可以使用 AddSingleton/AddTransient 注册它本身，或者依赖 AddControllers()。
            
            // 简化的 DI 注册（假定 Jellyfin 框架会处理 Controller 的发现）：
            // 如果 Jellyfin 发现失败，请尝试 serviceCollection.AddControllers().AddPluginParts(Assembly.GetExecutingAssembly());
            serviceCollection.AddTransient<Jellyfin.Plugin.BetterPlayer.Api.BetterPlayerJsController>();
        }
    }
}