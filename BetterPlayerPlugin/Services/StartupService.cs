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

    public class StartupService(ILogger<BetterPlayerPlugin> logger) : IScheduledTask
    {
        public string Name => "BetterPlayerPlugin Startup";

        public string Key => "BetterPlayerPlugin.Startup.b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67";

        public string Description => "Handles FileTransformation registration for BetterPlayer.";

        public string Category => "Startup Services";

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

        public IEnumerable<TaskTriggerInfo> GetDefaultTriggers()
        {
            yield return new TaskTriggerInfo()
            {
                Type = TaskTriggerInfoType.StartupTrigger,
            };
        }
    }
}