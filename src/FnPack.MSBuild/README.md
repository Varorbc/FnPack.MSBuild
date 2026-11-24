# FnPack.MSBuild

飞牛应用打包的 MSBuild 任务包。安装后在 `Publish` 之后自动把发布产物打包成 `.fpk`。

## 快速开始

1. 准备飞牛应用目录（含 `manifest`、`cmd`、`config` 等）。

2. 在项目文件中指定应用目录和版本号：

   ```xml
   <PropertyGroup>
     <PackageVersion>1.0.0</PackageVersion>
     <FnPackAppDir>$(MSBuildProjectDirectory)/fpk</FnPackAppDir>
   </PropertyGroup>

   <ItemGroup>
     <PackageReference Include="FnPack.MSBuild" Version="1.0.0" PrivateAssets="all" />
   </ItemGroup>
   ```

3. 正常发布即可：

   ```shell
   dotnet publish -c Release
   ```

   发布成功后在输出目录自动生成 `<appname>.fpk`。

## 属性

| 属性 | 必填 | 说明 |
| --- | --- | --- |
| `FnPackAppDir` | 是 | 飞牛应用源目录，包含 `manifest` 等文件 |
| `FnPackOutputDir` | 否 | 飞牛应用输出目录，默认 `$(ArtifactsPath)`，未启用 artifacts 输出时为 `$(OutputPath)` |
| `FnPackServerDir` | 否 | 发布产物填充目录，默认 `$(FnPackAppDir)/app/server` |
| `PackageVersion` | 否 | 写入 manifest 的 `version` 字段，默认 `1.0.0` |

## 支持平台

支持 win-x64、linux-x64、linux-arm64、osx-x64、osx-arm64。

## 相关链接

完整说明见 [GitHub 仓库](https://github.com/Varorbc/FnPack.MSBuild)。