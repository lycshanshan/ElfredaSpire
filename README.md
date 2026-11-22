# ElfredaSpire

《杀戮尖塔 2》的艾尔芙蕾妲角色模组，当前版本 0.0.6，适配游戏 0.113.1，依赖 RitsuLib 0.6.15。

## 安装

1. 安装游戏 0.113.1 和 RitsuLib 0.6.15 完整运行包。
2. 将安装包中的 `ElfredaSpire` 文件夹复制到游戏的 `mods` 文件夹，内含 DLL、JSON 和 PCK 三个文件。
3. 启动游戏并启用模组，选择“花海贤者”。

RitsuLib 需要保留完整的 `compat`、`shared` 目录和根目录文件。

## 构建

需要 .NET SDK 9 或 10、Godot 4.5.1 .NET 和对应的 Windows 导出模板。

复制 `Local.props.example` 为 `Local.props`，填写：

- `Sts2Dir`：游戏安装目录。
- `RitsuLibDir`：RitsuLib 0.6.15 完整运行包目录。
- `GodotEXEPath`：Godot 4.5.1 .NET 可执行文件路径。

```powershell
dotnet build ElfredaSpire.csproj -c Release
dotnet msbuild ElfredaSpire.csproj -t:ExportPck
```

构建目标框架为 `net9.0`，RitsuLib 引用游戏 API `0.113.0` 分支。生成的 DLL、JSON 和 PCK 位于 `artifacts/ElfredaSpire/`。

更新内容见 [CHANGELOG.md](CHANGELOG.md)。
