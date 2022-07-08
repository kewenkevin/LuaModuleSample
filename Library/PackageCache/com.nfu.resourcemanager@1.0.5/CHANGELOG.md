# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).



## [0.7.6] - 2021-12-3

[Fix]

1. 去掉 LoadAssethandler 对象池
2. 修复加载引用计数

## [0.7.5] - 2021-9-7

[Fix]

1. 修复 performance 错误

[Optimize]

1. 优化运行时GC，对象使用对象池回收

## [0.7.4] - 2021-8-24

[Feature]

1. `ResLoader` 支持协程

   ```c#
   var loader = ResLoader.Alloc();
   loader.Add2Load(assetPath).Load();
   yield return loader;
   loader.GetAsset(assetPath); //获取资源对象
   loader.MainAsset; //第一个添加资源对象
   ```

   

## [0.7.3] - 2021-8-18

[Fix]

1. 向前兼容

## [0.7.2] - 2021-8-17

[Feature]

1. 收集已加载过的和当前加载资源列表，日志和导出，Resource 面板 `Collect/Resources # Assets #`
2. 收集自动 `GC` 的 `ResLoader`，日志和导出，Resource 面板 `Collect/ResLoader GC #`
3. 支持版本 `ApplicableGameVersion` 比较

## [0.7.1] - 2021-7-20

[Feature]

1. 支持加载**循环依赖资源**
2. 增加 [版本文件对比编辑器](Documentation~/Comparer.md)
3. 增加 `Bundle Name Format` 配置，资源包可添加后缀，解决资源包和文件夹同名冲突

## [0.6.10] - 2021-6-17

[Feature]

1. 支持 [部分资源构建](Documentation~/PartBuild.md)
2. 增加编辑器单独打 `Lua` 包功能，菜单 `Build Lua` 
3. 添加平台文件夹名称选项 `Platform Name Style`,支持区分大小写
4. 编辑器模式支持重新加载Shader 选项 `EditorMode Reload Shader`

## [0.6.9] - 2021-5-26

[Feature]

1. 增加资源包同步到 `Android` 设备编辑器，菜单 `Device Sync`

## [0.6.8] - 2021-4-23
[Feature]

1. 增加极速加载接口的加载速度，最快可以当帧直接返回。（类同步）
2. BuildResource接口提供ForceRebuild的参数支持

## [0.6.7] - 2021-4-20
[Fix]
1. 修复AssetListProvider之间相互干扰

## [0.6.6] - 2021-4-20
[Fix]
1. 修复EditorComponent下打包报错问题
2. 修复DefaultAssetListProvider失效问腿
3. 修复资源热更模式选择Editor下每次回到Ignore模式的BUG
4. 修复资源表中资源未能正常通过分析的问题

## [0.6.4] - 2021-4-19
[Fix]
1. 所有ResLoader的当回池或回收后,其取消事件再不会向上抛出.
2. 所有subAsset不再从allAsset的0号位获取,使用遍历从allAssets中判断剔除主资源.


## [0.6.3] - 2021-4-18

1. 增加标签打包功能，增加一个标签的attribute,被打上对应标签后将认为和RootPath下资源同等使用。支持`|`分割，自定义多标签。
```C#
[AssetBundleAnalysisIncludeLabel]
public static string AssetBundleAnalysisIncludeLabel =  "PackedResource|ArtPackedResource";
```
2. 为标签打包功能，增加两个右键菜单，可以快速标记文件夹及路径，或取消资源相关标记。
3. 对外提供一个批量传入路径进行自动化标记的接口。
```C#
// 清除所有资源标记
AssetIncludeFilterLabelEditor.ClearAllAssetIncludeMark()
        
// 标记一组路径（支持文件及文件夹）
// paths:文件或文件夹路径列表
// invert:是否是反（取消）标记
AssetIncludeFilterLabelEditor.MarkPaths(paths,invert)
```
4. 增加一个自定义打包配置文件的attribute,配置的Assets中每一项为要作为根资源参与打包的资源。
```C#
[PackedAssetListPathAttribute] 
public static string PackedAssetListPathAttribute = Utility.Path.GetRegularPath(Path.Combine(Application.dataPath, "ResourceCustomConfigs/PackedAssetCollection.xml"));
```
5. 提供一个类来帮助标记到配置表
```C#
AssetPackListCollection collection = new AssetPackListCollection();
AssetPackListCollection.Load();
AssetPackListCollection.AssignAsset("Assets/ArtRes/a.mat");
AssetPackListCollection.Save();
```
6. 为AssetPackListCollection提供一个辅助数据提供器。你可以自己定制从你的文件读取打包配置。所有provider的数据会取并集进入打包。
```C#
public class MyExcelAssetPackListDataProvider : IAssetPackListDataProvider
{
    public event GameFrameworkAction<int, int> OnLoadingAsset = null;

    public event GameFrameworkAction OnLoadCompleted = null;
    
    
     public bool LoadConfig(ref SortedDictionary<string, AssetPackedConfig> data){
        //实现自己的读取逻辑
     }
     
     public bool SaveConfig(ref SortedDictionary<string, AssetPackedConfig> data){
       //实现自己的保存逻辑
     }

}
```


## [0.6.2] - 2021-4-18

1. 增加热更新时版本号匹配的模式选择，默认选择IgnoreVersion只要本地和服务器的版本号不同即更新，选择Normal，则只有服务器配置版本比本地新时才更新。
2. 增加UMTResource.LoadAssetRapid以及resloader.Add2LoadRapid方法，急速模式加载（异步混合同步方式）。
3. 大幅度调整底层引用计数方式及加载机制，优化资源加载速度，降低任务产生数量。
4. 推荐对小资源尽可能合包，不会增加内存负担。
5. 优化引用计数显示，可以看到真正的引用次数。


## [0.6.1] - 2021-3-15

1. 增加 `LoadAssetImmediate` 同步加载方法
2. 增加依赖剔除功能 `Strip Auto Dependency`
3. 修复加载类型错误

## [0.6.0] - 2021-3-2

1. 新的[资源配置编辑器](Documentation~/Settings.md)，使用前请阅读 [v0.6.0-change](Documentation~/v0.6.0-change.md)
2. 修复资源路径包含特殊字符没进资源包

## [0.5.5] - 2021-2-23

1. 修复资源移动位置后没有生成 `assetbundle`

## [0.5.4] - 2021-2-19

1. 增加OBB资源支持


## [0.5.3] - 2020-2-7

1. 修复Download系统中gzip的问题
2. 增加EditorComponent的注释tooltips

## [0.5.2] - 2020-2-3

1. 修复 0.5.1 编辑器模式下 OnPreprocessAnalysisBegin 调用两次

## [0.5.1] - 2020-2-1

1. 修复dll依赖
2. 优化性能

## [0.5.0] - 2020-1-21

1. `StreamingAssets` 路径默认`AssetBundles`根目录，`ResourceStreamingAssetsPathAttribute`可配置`StreamingAssets`目录
2. 升级到该版本需要删除`StreamingAssets`下之前生成的旧文件`Assets`，`GameFrameworkList.dat`，`GameFrameworkVersion.dat`


## [0.4.8] - 2020-11-18

1. 热更下载资源gzip格式自动解压的支持

## [0.4.7] - 2020-11-18

1. 修复本地打包时依赖分析过程中如果出现异常。分析会被无限循环调用

## [0.4.4] - 2020-11-18

1. ResLoader增加重复加载的支持，注意，如果上一批次未完成就开始下一个批次，批次完成的回调会只执行第二次的，依次类推

## [0.4.3] - 2020-10-27

1. 打包增加一条报错，不包含Asset的空Resource的名字会被打印
2. 修改Atlas相关日志，使用资源内部Log控制
3. Editor下回调增加异常处理，保证回调中异常不会影响后续处理


## [0.4.2] - 2020-10-22

1. LoadAsset回调中增加闭包，保证返回的assetname与传入相同
2. 修复Editor下获取SubAssets的数组包含一个额外自身的异常问题

## [0.4.0] - 2020-10-12

1. 修复EditorResourceComponent中使用UnityEditor接口导致打包报错问题

## [0.4.0] - 2020-10-12

1. 修复一些回调调用时机的BUG
2. 增加了取消的接口，取消后后续回调将不会调用
3. 增加了SubAsset的支持

## [0.3.1] - 2020-10-12

1. 增加ResLoader的GC自动卸载支持
2. 增加GameObject 通过预制体直接加载并创建，同时自动创建引用计数的功能

## [0.2.12] - 2020-10-12

1. 增加原始资源的Binary打包支持
   * [RawAssetPath] 标签 ,指定原始打包资源根目录
2. 增加获取原始资源实际加载路径的接口



## [0.2.9] - 2020-09-29

1. 资源加载回调异常 EDITOR下阻塞下次运行，真机不影响下次运行



## [0.2.7] - 2020-09-27

1. 加载及卸载场景的支持





## [0.2.6] - 2020-09-24

1. jekins打包支持
2. 热更新进度获取方式提供





## [0.1.20] - 2020-09-21

1. 修复一系列真机问题

2. 支持资源变体的打包分析及使用



## [0.1.11] - 2020-09-16

实际使用对大小写不再敏感



## [0.1.10] - 2020-09-16

修复打包lua导入后无法识别问题



## [0.1.9] - 2020-09-16

jekins自动打包支持



## [0.1.7] - 2020-09-15

为了UGUI的sprite增加atlas支持



## [0.1.6] - 2020-09-15

为了UGUI的sprite加载，增加LoadAsset的资源类型指定。



## [0.1.5] - 2020-09-15

增加了一些为了lua效率的接口封装在UMTResources



## [0.1.4] - 2020-09-14

修复了资源分析策略的反射程序集问题导致无法找到自定义的策略类

完善文档



## [0.1.3] - 2020-09-14

增加loader的获取资源接口

修复一个loader未加载完成回池的事件异常bug



## [0.1.2] - 2020-09-14

修复命名空间

调整example不打入包，example通过unitypackage导入

修改包介绍信息



## [0.1.1] - 2020-09-11

修复一些包依赖bug



## [0.1.0] - 2020-09-11

资源管理器V2 package化第一版本