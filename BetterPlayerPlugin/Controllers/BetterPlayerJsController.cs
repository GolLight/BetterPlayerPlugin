// <copyright file="BetterPlayerJsController.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer.Api
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;

    /// <summary>
    /// 提供 better_player.js 客户端脚本文件的 HTTP 访问接口。.
    /// </summary>
    [ApiController]
    [Route("BetterPlayerPlugin")] // 路由基础路径 /BetterPlayerPlugin
    public class BetterPlayerJsController : ControllerBase
    {
        private readonly ILogger<BetterPlayerJsController> logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BetterPlayerJsController"/> class.
        /// 构造函数用于 DI 注入 <see cref="ILogger{TCategoryName}"/>。.
        /// </summary>
        /// <param name="logger">日志记录器服务。.</param>
        public BetterPlayerJsController(ILogger<BetterPlayerJsController> logger)
        {
            this.logger = logger;
        }

        /// <summary>
        /// 获取客户端增强脚本 better_player.js 的内容。.
        /// </summary>
        /// <returns>一个文件结果，包含客户端脚本。.</returns>
        [HttpGet("better_player.js")]
        [Produces("application/javascript")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetClientScript()
        {
            this.logger.LogDebug("[BP-DEBUG] better_player.js requested.");
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                const string resourceName = "Jellyfin.Plugin.BetterPlayer.Resources.better_player.js";

                var stream = assembly.GetManifestResourceStream(resourceName);

                if (stream == null)
                {
                    this.logger.LogError("[BP-ERROR] 未找到嵌入资源 {ResourceName}。", resourceName);
                    return this.NotFound();
                }

                // 框架在文件传输完成后，会负责关闭这个 Stream。
                return this.File(stream, "application/javascript");
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "[BP-ERROR] GetClientScript 异常。");
                return this.StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
