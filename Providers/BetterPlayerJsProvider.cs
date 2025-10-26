// 相对路径: /Api/BetterPlayerJsProvider.cs
using Microsoft.AspNetCore.Mvc; // ✨ 新增：用于 ControllerBase, HttpGet
using Microsoft.AspNetCore.Http; // ✨ 新增：用于 FileResult 的 ContentType
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Reflection;

namespace Jellyfin.Plugin.BetterPlayer.Api
{
    // ✨ 继承 ControllerBase 并设置 [ApiController] 和 [Route]
    [ApiController]
    [Route("BetterPlayerPlugin")] // 路由基础路径 /BetterPlayerPlugin
    public class BetterPlayerJsController : ControllerBase
    {
        private readonly ILogger<BetterPlayerJsController> _logger;

        // 构造函数用于 DI 注入 ILogger
        public BetterPlayerJsController(ILogger<BetterPlayerJsController> logger)
        {
            _logger = logger;
        }

        // ✨ HttpGet 方法，路由完整路径为 /BetterPlayerPlugin/better_player.js
        [HttpGet("better_player.js")]
        [Produces("application/javascript")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult GetClientScript()
        {
            _logger.LogDebug("[BP-DEBUG] better_player.js requested.");
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                const string resourceName = "Jellyfin.Plugin.BetterPlayer.Resources.better_player.js";

                // 🌟 关键修改：移除 'using var'，改为普通的 'var'
                var stream = assembly.GetManifestResourceStream(resourceName);
                
                if (stream == null)
                {
                    _logger.LogError("[BP-ERROR] 未找到嵌入资源 {ResourceName}。", resourceName);
                    return NotFound();
                }
                
                // 框架在文件传输完成后，会负责关闭这个 Stream。
                return File(stream, "application/javascript");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[BP-ERROR] GetClientScript 异常。");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}