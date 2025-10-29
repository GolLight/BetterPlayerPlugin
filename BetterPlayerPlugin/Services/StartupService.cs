// <copyright file="StartupService.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.Loader;
    using System.Threading;
    using System.Threading.Tasks;
    using Jellyfin.Plugin.BetterPlayer.Helpers;
    using MediaBrowser.Model.Tasks;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// Jellyfin 服务器启动时执行的计划任务服务。
    /// 用于处理客户端脚本的注入逻辑，优先使用 FileTransformation 插件。.
    /// </summary>
    /// <param name="logger">日志记录器服务.</param>
    public class StartupService(ILogger<BetterPlayerPlugin> logger) : IScheduledTask
    {
        /// <summary>
        /// Gets 获取任务的名称。.
        /// </summary>
        public string Name => "BetterPlayerPlugin Startup";

        /// <summary>
        /// Gets 获取任务的唯一标识键。.
        /// </summary>
        public string Key => "BetterPlayerPlugin.Startup.b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67";

        /// <summary>
        /// Gets 获取任务的描述.
        /// </summary>
        public string Description => "Handles FileTransformation registration for BetterPlayer.";

        /// <summary>
        /// Gets 获取任务所属的类别。.
        /// </summary>
        public string Category => "Startup Services";

        /// <summary>
        /// 执行计划任务。在 Jellyfin 服务器启动或应用更新后执行。.
        /// </summary>
        /// <param name="progress">进度报告接口。.</param>
        /// <param name="cancellationToken">取消令牌.</param>
        /// <returns>表示异步操作的 <see cref="Task"/>。.</returns>.
        public Task ExecuteAsync(IProgress<double> progress, CancellationToken cancellationToken)
        {
            const string PluginGuid = "b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67";

            List<JObject> payloads =
            [
                new()
                {
                    { "id", PluginGuid },
                    { "fileNamePattern", "index.html" },
                    { "callbackAssembly", this.GetType().Assembly.FullName },
                    { "callbackClass", typeof(WebHtmlInjector).FullName },
                    { "callbackMethod", nameof(WebHtmlInjector.FileTransformer) },
                }
            ];

            Assembly? fileTransformationAssembly =
                AssemblyLoadContext.All.SelectMany(x => x.Assemblies).FirstOrDefault(x =>
                    x.FullName?.Contains(".FileTransformation") ?? false);

            if (fileTransformationAssembly == null)
            {
                logger.LogInformation("[BP-INFO] FileTransformation plugin not found. Fallback to direct injection.");
                WebHtmlInjector.Direct();
                return Task.CompletedTask;
            }

            Type? pluginInterfaceType = fileTransformationAssembly.GetType("Jellyfin.Plugin.FileTransformation.PluginInterface");

            if (pluginInterfaceType == null)
            {
                logger.LogInformation("[BP-INFO] FileTransformation PluginInterface not found. Fallback to direct injection.");
                WebHtmlInjector.Direct();
                return Task.CompletedTask;
            }

            logger.LogInformation("[BP-INFO] Registering BetterPlayer for FileTransformation plugin.");
            MethodInfo? registerMethod = pluginInterfaceType.GetMethod("RegisterTransformation");

            foreach (JObject payload in payloads)
            {
                registerMethod?.Invoke(null, [payload]);
            }

            logger.LogInformation("[BP-INFO] BetterPlayer FileTransformation registration complete [BP-SCHED-OK].");
            return Task.CompletedTask;
        }

        /// <summary>
        /// 获取任务的默认触发器列表。.
        /// </summary>
        /// <returns>包含启动触发器信息的 <see cref="IEnumerable{TaskTriggerInfo}"/>。.</returns>.
        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo()
            {
                Type = TaskTriggerInfoType.StartupTrigger,
            };
        }
    }
}
