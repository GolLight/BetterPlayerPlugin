// <copyright file="PluginConfiguration.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

namespace Jellyfin.Plugin.BetterPlayer.Configuration;

using MediaBrowser.Model.Plugins; // 核心依赖：BasePluginConfiguration 仍然在这里

/// <summary>
/// 配置选项枚举.
/// </summary>
public enum BetterPlayerOptions
{
    /// <summary>
    /// 默认播放器行为.
    /// </summary>
    Default,

    /// <summary>
    /// 启用全部增强功能.
    /// </summary>
    Enhanced,

    /// <summary>
    /// 仅启用 Trickplay.
    /// </summary>
    TrickplayOnly,
}

/// <summary>
/// 插件配置.
/// </summary>
public class PluginConfiguration : BasePluginConfiguration
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PluginConfiguration"/> class.
    /// 初始化 <see cref="PluginConfiguration"/> 类的新实例..
    /// </summary>
    public PluginConfiguration()
    {
        // set default options here
        // 保持默认值，以确保插件在没有 UI 配置的情况下也能运行
        // this.Options = BetterPlayerOptions.Enhanced;
        // this.EnableCustomUI = true;
        // this.MaxPreviewWidth = 320;
        // this.CustomScriptUrl = string.Empty;
    }

    /*
    ====================================================================================================
    以下配置项在前端 UI 上暂时被注释（"敬请期待"），C# 属性也被注释以匹配前端状态.
    一旦功能启用，请取消注释并确保构造函数中的默认值被设置.
    ====================================================================================================
    */

    // /// <summary>
    // /// Gets or sets a value indicating whether 是否启用自定义 UI..
    // /// </summary>
    // public bool EnableCustomUI { get; set; }

    // /// <summary>
    // /// Gets or sets 预览缩略图最大宽度..
    // /// </summary>
    // public int MaxPreviewWidth { get; set; }

    // /// <summary>
    // /// Gets or sets 自定义脚本 URL..
    // /// </summary>
    // public string CustomScriptUrl { get; set; }

    // /// <summary>
    // /// Gets or sets 播放器增强选项..
    // /// </summary>
    // public BetterPlayerOptions Options { get; set; }
}
