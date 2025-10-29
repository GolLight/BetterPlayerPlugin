// <copyright file="PluginServiceRegistrator.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

// using Jellyfin.Plugin.BetterPlayer.Api; // IApiService 模式不再需要
namespace Jellyfin.Plugin.BetterPlayer
{
    using Jellyfin.Plugin.BetterPlayer.Services;
    using MediaBrowser.Controller;
    using MediaBrowser.Controller.Plugins;
    using Microsoft.Extensions.DependencyInjection;

    /// <summary>
    /// 用于将插件服务注册到 Jellyfin DI 容器的类。.
    /// </summary>
    public class PluginServiceRegistrator : IPluginServiceRegistrator
    {
        /// <summary>
        /// 注册插件所需的各种服务到 DI 容器。.
        /// </summary>
        /// <param name="serviceCollection">DI 服务集合。.</param>
        /// <param name="applicationHost">服务器应用主机接口。.</param>
        public void RegisterServices(IServiceCollection serviceCollection, IServerApplicationHost applicationHost)
        {
            // 注册 StartupService (IScheduledTask)
            serviceCollection.AddSingleton<StartupService>();

            // ✨ 注册 Controller：使用 AddControllersAsServices 将所有继承自 Controller 的类注册
            // 这通常在 Jellyfin 的启动时自动完成，但显式注册或使用 AddSingleton/AddTransient 注册它本身，或者依赖 AddControllers()。

            // 简化的 DI 注册（假定 Jellyfin 框架会处理 Controller 的发现）：
            // 如果 Jellyfin 发现失败，请尝试 serviceCollection.AddControllers().AddPluginParts(Assembly.GetExecutingAssembly());
            serviceCollection.AddTransient<Jellyfin.Plugin.BetterPlayer.Api.BetterPlayerJsController>();
        }
    }
}
