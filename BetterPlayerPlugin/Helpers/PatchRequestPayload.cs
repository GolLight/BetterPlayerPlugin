// <copyright file="PatchRequestPayload.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer.Helpers
{
    /// <summary>
    /// FileTransformation 插件请求负载的容器。
    /// 使用 record class 是一种更简洁的 DTO 定义方式。.
    /// </summary>
    public record PatchRequestPayload
    {
        /// <summary>
        /// Gets 获取或设置 index.html 的内容。.
        /// </summary>
        public string? Contents { get; init; }
    }
}