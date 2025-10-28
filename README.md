# Better Web Player Extension (Better Player)

## 🎥 项目概述：Web 播放增强脚本注入框架

**Better Web Player Extension** 的核心价值在于提供一个**灵活、通用的 JavaScript 脚本注入框架**，专门为 Jellyfin Web 客户端设计。

### 核心目的与价值

* **当前版本功能 (v1.x):** 插件默认内置并注入了桂鸢原作者开发的优秀前端脚本 `better_player.js`，为用户提供即时的播放器增强体验。
* **未来目标 (v2.x+):** **实现自定义脚本注入功能。** 本插件将允许用户通过设置界面，指定任何外部 JavaScript 脚本的 URL，并将其安全、稳定地注入到 Jellyfin Web 播放页面中，以实现高度个性化的播放器增强。

本插件通过 C# 后端逻辑，利用 **File Transformation Plugin (FT 插件)** 机制，实现脚本的安全、稳定注入。

### 🤖 开发者说明

本项目由 **GolLight** 独立开发和维护，并全程利用 **Google Gemini AI** 和 **VS Code Agent** 进行代码生成、架构设计和调试辅助。

---

## 🙏 核心功能来源与致谢

本插件所封装的核心前端增强逻辑（`better_player.js`）**全部** 来源于以下优秀项目。本插件本身不包含任何前端功能逻辑，所有播放器增强特性均由原脚本提供。

| 属性 | 内容 | 链接 |
| :--- | :--- | :--- |
| **原作者** | **桂鸢 (guiyuanyuanbao)** | [https://github.com/guiyuanyuanbao/](https://github.com/guiyuanyuanbao/) |
| **原项目仓库** | betterJellyfinWebPlayer-extension | [https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension](https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension) |
| **核心脚本文件** | `better_player.js` | [https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension/blob/main/batter_player.js](https://github.com/guiyuanyuanbao/Jellyfin-betterJellyfinWebPlayer-extension/blob/main/batter_player.js) |

**致谢：** 我们在此衷心感谢原作者 `桂鸢` 提供的优秀前端脚本。

### 👏 插件模板与架构参考致谢

本插件的 **项目结构、C# 架构和 File Transformation (FT) 注入机制** 大量参考了以下项目，在此表示诚挚感谢：

* **项目名称:** InPlayerEpisodePreview
* **仓库:** [https://github.com/Namo2/InPlayerEpisodePreview](https://github.com/Namo2/InPlayerEpisodePreview)
* **作者:** Namo2

---

## ✨ 插件特性 (功能由默认注入的 `better_player.js` 提供)

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
> **`https://raw.githubusercontent.com/GolLight/BetterPlayerPlugin/master/manifest.json`**

### 2. 完成安装

1.  导航到 **目录 (Catalog)** 选项卡。
2.  找到并安装 **"Better Player"** (或 **"Better Web Player Extension"**，取决于您的 `manifest.json` 配置)。
3.  安装后，**重启 Jellyfin 服务器** 以使插件生效。

---

## ⚙️ 核心原理流程图

该图展示了插件在 Jellyfin 启动时如何注册注入点，以及浏览器如何加载脚本的机制：
```mermaid
graph TD
    subgraph 启动与注册 (Jellyfin Server Boot)
        A[StartupService 启动] --> B{FT 插件已安装?};
        
        B -- 是 --> C[反射调用: FT 插件注册 WebHtmlInjector.FileTransformer];
        B -- 否 (降级) --> D[WebHtmlInjector.Direct()：直接修改 index.html];
        
        C --> E(注入点：通过 FT 拦截机制建立);
        D --> E;
    end
    
    subgraph 脚本分发与加载 (Browser Access)
        F[用户在浏览器访问 Web 播放器] --> G(浏览器请求 index.html);
        
        G --> H{服务器响应: HTML 文件被注入};
        
        H --> I[HTML 包含脚本标签: <script src='/BetterPlayerPlugin/better_player.js'>];
        
        I --> J[浏览器请求: GET /BetterPlayerPlugin/better_player.js];
        
        J --> K[BetterPlayerJsController (API) 拦截请求];
        
        K --> L[从插件 DLL (Resources) 加载 better_player.js];
        
        L --> M(🚀 脚本执行：播放器增强完成);
    end
    
    A --> F;

    style B fill:#FEEFB3,stroke:#CC9900;
    style E fill:#DDEEFF,stroke:#3C88A8,stroke-width:2px;
    style M fill:#E8F7DD,stroke:#6AA84F,font-weight:bold;