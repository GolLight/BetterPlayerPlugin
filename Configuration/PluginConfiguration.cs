// <copyright file="PluginConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer.Configuration;

using MediaBrowser.Model.Plugins; // 核心依赖：BasePluginConfiguration 仍然在这里

/// <summary>
/// 配置选项枚举。.
/// </summary>
public enum BetterPlayerOptions
{
    /// <summary>
    /// 默认播放器行为。.
    /// </summary>
    Default,

    /// <summary>
    /// 启用全部增强功能。.
    /// </summary>
    Enhanced,

    /// <summary>
    /// 仅启用 Trickplay。.
    /// </summary>
    TrickplayOnly,
}

/// <summary>
/// 插件配置。.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// 初始化 <see cref="PluginConfiguration"/> 类的新实例。.
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        this.Options = BetterPlayerOptions.Enhanced;
        this.EnableCustomUI = true;
        this.MaxPreviewWidth = 320;

        // ✨ 修正：使用空字符串作为可空字符串的默认值，符合 Nullable 启用时的最佳实践。
        this.CustomScriptUrl = string.Empty;
    }

    /// <summary>
    /// Gets or sets a value indicating whether 是否启用自定义 UI。.
    /// </summary>
    public bool EnableCustomUI { get; set; }

    /// <summary>
    /// Gets or sets 预览缩略图最大宽度。.
    /// </summary>
    public int MaxPreviewWidth { get; set; }

    /// <summary>
    /// Gets or sets 自定义脚本 URL。.
    /// </summary>
    // ✨ 修正：如果 .csproj 中 <Nullable>enable</Nullable>，则字符串类型需要使用 ? 标记或在构造函数中初始化。
    // 由于您已经在构造函数中初始化为 string.Empty，可以保持原样，或标记为可空（取决于代码库的规范）。
    public string CustomScriptUrl { get; set; }

    /// <summary>
    /// Gets or sets 播放器增强选项。.
    /// </summary>
    public BetterPlayerOptions Options { get; set; }
}