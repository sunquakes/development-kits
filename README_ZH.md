# 开发者工具

<p align="center">
  <img src="Resources/Images/logo.png" alt="DevTools Logo" width="128" height="128">
</p>

<p align="center">
  <strong>轻量级开发者工具箱，助力日常开发任务</strong>
</p>

<p align="center">
  <a href="#功能">功能</a> •
  <a href="#安装">安装</a> •
  <a href="#使用">使用</a> •
  <a href="#开发">开发</a> •
  <a href="README.md">English</a>
</p>

---

## 功能

- **MD5 哈希计算** - 计算 32 位和 16 位 MD5 哈希值（大写/小写）
- **条形码生成器** - 生成 CODE 128 条形码
- **二维码生成器** - 为文本/网址生成二维码
- **Base64 ↔ 图片** - Base64 字符串与图片互转
- **JSON 格式化** - 格式化、展开、折叠 JSON 数据
- **手写签名** - 绘制签名并转换为 Base64 或保存为图片

## 安装

根据您的平台下载最新版本：

| 平台 | 架构 | 下载 |
|------|------|------|
| Windows | x64 (64位) | `DevTools-win-x64.exe` |
| Windows | x86 (32位) | `DevTools-win-x86.exe` |
| Windows | ARM64 | `DevTools-win-arm64.exe` |

## 使用

1. 下载对应平台的可执行文件
2. 直接运行 `DevTools.exe`（无需安装）
3. 在首页选择需要的工具

## 开发

### 环境要求

- .NET 8.0 SDK
- Windows 操作系统

### 构建

```bash
# 还原依赖
dotnet restore

# 构建
dotnet build

# 运行
dotnet run

# 发布（自包含单文件）
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

### 项目结构

```
DevTools/
├── Pages/              # 应用页面
│   ├── HomePage.xaml
│   ├── Md5Page.xaml
│   ├── BarcodePage.xaml
│   ├── QrPage.xaml
│   ├── Base64ImagePage.xaml
│   └── JsonFormatPage.xaml
├── Resources/          # 资源（图片、字符串、字体）
│   ├── Images/
│   ├── Strings.resx
│   ├── Strings.zh-CN.resx
│   └── Strings.en-US.resx
├── Helpers/            # 工具类
├── MainWindow.xaml     # 主窗口
└── App.xaml            # 应用入口
```

## 本地化

应用程序支持多语言：
- 简体中文
- English (en-US)

界面语言自动匹配系统语言。

## 许可证

Apache License 2.0

## 更新日志

查看 [CHANGELOG.md](CHANGELOG.md) 了解版本历史。
