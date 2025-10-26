# Better Web Player Extension (BetterPlayerPlugin)

## 🎥 项目概述：Web 播放增强封装包

**Better Web Player Extension** 是一个轻量级的 Jellyfin 插件。本插件的唯一目的和核心价值在于：**将桂鸢原作者开发的优秀前端脚本 `better_player.js` 包装成一个标准的 Jellyfin 插件，方便用户通过存储库和插件管理系统进行安装和更新。**

本插件通过 C# 后端逻辑，利用 **File Transformation (FT) 插件** 机制，将脚本安全、稳定地注入到 Jellyfin Web 播放页面。

### 🤖 开发者说明

本项目由 **GolLight** 独立开发和维护，并全程利用 **Google Gemini AI** 和 **VS Code Agent** 进行代码生成、架构设计和调试辅助。

---

## 🙏 核心功能来源与致谢

本插件所封装的核心前端增强逻辑（`better_player.js`）**全部** 来源于以下优秀项目。本插件本身不包含任何前端功能逻辑，所有播放器增强特性均由原脚本提供。

| 属性 | 内容 | 链接 |
| :--- | :--- | :--- |
| **原作者** | **桂鸢 (guiyuanyuanbao)** | N/A |
| **原项目仓库** | betterJellyfinWebPlayer-extension | [https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension](https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension) |
| **核心脚本文件** | `better_player.js` | [https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension/blob/main/batter_player.js](https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension/blob/main/batter_player.js) |

**致谢：** 我们在此衷心感谢原作者 `桂鸢` 提供的优秀前端脚本。

---

## ✨ 插件特性 (功能由 `better_player.js` 提供)

* 增强的键盘和触控交互。
* 鼠标悬停时的 Trickplay 预览缩略图。
* 其他自定义播放器改进。

---

## 🚀 安装步骤

### ⚠️ 先决条件

1.  Jellyfin 服务器版本：**10.11.0 或更高**。
2.  必须安装 **File Transformation Plugin (FT 插件)**，本插件依赖其注入功能。
    > **FT 插件链接:** [https://github.com/IAmParadox27/jellyfin-plugin-file-transformation](https://github.com/IAmParadox27/jellyfin-plugin-file-transformation)

### 1. 添加自定义存储库

在您的 Jellyfin 管理面板中，导航到 **插件** -> **存储库**，并添加以下 URL：

> **存储库 URL (GitHub Raw Link)：**
>
> `https://raw.githubusercontent.com/[您的GitHub用户名]/[您的仓库名]/main/repository.json`
>
> *请确保用您的实际 GitHub 用户名和仓库名替换占位符。*

### 2. 完成安装

1.  导航到 **目录 (Catalog)** 选项卡。
2.  找到并安装 **"Better Web Player Extension"**。
3.  安装后，**重启 Jellyfin 服务器** 以使插件生效。

---

## 🛠️ 技术详情 (For Developers & Debugging)

| 属性 | 内容 | 备注 |
| :--- | :--- | :--- |
| **插件名称空间** | `BetterPlayerPlugin` | C# 内部命名空间 |
| **插件 GUID** | `b5eaeb4a-57d9-4703-9e63-2c2ad6a7fc67` | 唯一标识符 |
| **目标框架** | `.NET 9.0` | 编译环境 |
| **Jellyfin 依赖** | `10.11.0` | 目标 API 版本 |
| **核心机制** | C# **反射** | 用于安全注册到 FT 插件 |

**核心文件:**
* `Plugin.cs`
* `Startup.cs`
* `BetterPlayerJsProvider.cs`
* `WebHtmlTransformer.cs`
* `Resources/better_player.js`
* `plugin.json` (发布元数据)

您可以使用 `dotnet publish` 命令在 Linux 环境下编译项目。