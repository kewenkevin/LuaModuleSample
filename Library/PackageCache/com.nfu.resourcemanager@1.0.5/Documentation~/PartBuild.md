# 部分资源构建

```c#
bool ResourceBuilderController.BuildPartResources(Resource[] partResources, string assetPathRoot)
```

部分资源包生成，基于上一次的构建资源版本增量生成，用于开发测试，提升效率，`partResources` 为部分资源，`assetPathRoot` 资源根目录，部分资源必须在同一个根目录，每次生成前会清除该根目录。

- 至少一次完整的构建，包含版本(Full)
- 仅支持二进制资源包
- 不能用于发布版本的构建，因为缺少资源的完整性
- `IsPartBuild` 为 true 为部分构建，否则为完整构建



## 单独生成Lua资源包

**菜单**

`Build Lua`

**实现**

```c#
static void BuildResources.BuildLua(ResourceBuilderController controller, string rootPath, string[] assetPaths, string[] filePaths = null)
```
 `BuildResources.BuildLua` 提供一个生成 `lua` 文件格式的 `Resource`方法，参数 `assetPaths` 为资源路径，参数 `filePaths` 为真实的文件路径，如果为空则为 `AssetPath`，支持 `Assets` 之外的真实路径

**样例**


```c#
//lua 输出目录，在Assets之外目录
string outputPath = "Library/LuaCache";
//lua AssetPath 根路径
string rootAssetPath = "Assets/Lua";

//清理目录
if (Directory.Exists(outputPath))
    Directory.Delete(outputPath, true);

//生成 lua，如果未使用配置的<LuaPreprocessBuild>，则需要改为定制的生成lua，返回输出目录
BuildResources.CompileLua(outputPath);

//获取打包的lua文件和 AssetPath
List<string> assetPaths = new List<string>();
List<string> files = new List<string>();
int startIndex = outputPath.Length;
if (!(outputPath.EndsWith("/") || outputPath.EndsWith("\\")))
    startIndex++;
foreach (var file in MetaFile.EnumerateFiles(outputPath, true))
{
    string relFileName = file.Substring(startIndex);
    assetPaths.Add(Path.Combine(rootAssetPath, relFileName));
    files.Add(file);
}

ResourceBuilderController controller = new ResourceBuilderController();
controller.Load(incrementalVersion: false);

//搜索已有的最大的资源版本目录，作为部分构建的基础版本
string lastestVersionFolderName = controller.FindLastestVersionFolderName();
int resVer;
int index;
index = lastestVersionFolderName.LastIndexOf('_');
resVer = int.Parse(lastestVersionFolderName.Substring(index + 1));
//设置版本号
controller.InternalResourceVersion = resVer;
//触发 IResBuildEventHandler 事件
controller.PartBuildTriggerEvent = true;

//生成部分资源包，如果lua分包规则不同，需要定制该方法
BuildResources.BuildLua(controller, rootAssetPath, assetPaths.ToArray(), files.ToArray());

//复制到 StreamingAssets
if (EditorResourceSettings.CopyToStreamingAssets && controller.IsCurrentPlatform(controller.CurrentPlatform))
{
    string workingPath, outputPackagePath, outputFullPath, outputPackedPath;
    controller.GetOutputPath(controller.CurrentPlatform, out workingPath, out outputPackagePath, out outputFullPath, out outputPackedPath);            
    BuildResources.CopyToStreamingAsset(rootAssetPath, workingPath, outputPackagePath, outputFullPath, outputPackedPath);
}
```

