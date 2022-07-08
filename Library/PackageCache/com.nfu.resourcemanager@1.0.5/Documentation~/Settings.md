# 资源设置

v0.6.0 之前老版本升级后使用配置请参考 [v0.6.0-change](v0.6.0-change.md) 注意事项



## 设置界面（参考）

![AssetBundle](Pics\AssetBundle.PNG)

- Analysis
  资源分析

- Build
  生成资源

- Resource Version

  资源版本号

- Platform
  默认 `Follow project` 跟随项目平台，可以选择多个平台

- Mode

  资源模式，模式包含（Editor, Package, Updatable），可以通过菜单切换

- Variant

  只对编辑器有效，设置变体

- Zip All Resources

  进行zip压缩

- Output Path

  生成的输出目录

- StreamingAssets Subpath

  `StreamingAssets` 子目录

- Set AssetBundleName

  勾选后自动设置 `AssetImporter` 的 `assetBundleName` 和 `assetBundleVariant`

- Strip Auto Dependency

  剔除自动依赖，被引用次数小于2的资源

- Exclude

  全局排除的 `AssetPath`，正则表达式格式

- **Before Build**

  资源生成之前

- Auto Analysis
  在生成之前执行分析

- Asset Root Path

  资源根目录，默认值：`Assets/ResourcesAssets`，`|`分隔多个目录

- Bundle Name Format

  定制资源包名称格式，解决 `AssetBundle` 名称与文件夹同名冲突

  如：

  ```tex
  {$AssetBundle}_bundle
  ```

- Platform Name Style

  解决平台名称大小写，如 `iOS` 

  Default：Platform.IOS

  Invariant：BuildTarget.iOS

  Lower: 小写 `ios`

- **After Build**

  资源生成之后

- Copy To StreamingAssets
  生成之后复制到 `StreamingAssets` 目录

- **PreprocessBuilds**

  资源生成之前的准备工作

- **Rules**

  `AssetPath` 寻址生成规则





## PreprocessBuilds

资源生成之前的准备工作，如图集生成，Lua编译等，通过实现 `IResourcePreprocessBuild` 接口扩展



配置

- Provider

  实现 `IResourcePreprocessBuild` 接口



### Atlas

分析前根据文件夹生成图集
![AssetBundle - PreprocessBuild -  Atlas](Pics\AssetBundle - PreprocessBuild -  Atlas.PNG)

- Sprite Root Path
  生成图集根目录，因为是根目录，所以注意结束要带 `$`，匹配项目中所有的目录，基础资源和变体资源目录
- Template
  图集设置模板，生成的图集设置都从模板读取

### Lua

分析前编译`Lua`源代码生成 `.lua.bytes`
![AssetBundle - PreprocessBuild -  Lua](Pics\AssetBundle - PreprocessBuild -  Lua.PNG)

- Source Path
  默认：Lua，源代码位置
- Source Extension
  默认：.lua|.lua.txt|.bytes，源代码扩展名
- Output Path
  默认：Assets/Lua，输出路径  
- Output Extension
  默认：.lua.bytes，输出扩展名
- Compile Enabled
  默认：true，开启编译
- Compile Max Parallel
  默认：10，编译并发数



## Rules

`AssetPath` 寻址生成规则，通过实现 `IAddressableProvider` 接口扩展



配置

- Include

  包含的 `AssetPath`，正则表达式格式

- Exclude

  排除的 `AssetPath`，正则表达式格式

- Provider

  实现 `IAddressableProvider` 接口



### Atlas

图集资源规则

![AssetBundle - AddressableProvider -  Atlas](Pics\AssetBundle - AddressableProvider -  Atlas.PNG)

### Lua

Lua 资源规则

- Root Path

  默认：Assets/Lua，Lua 根目录

![AssetBundle - AddressableProvider -  Lua](Pics\AssetBundle - AddressableProvider -  Lua.PNG)



### AssetPath

手动配置规则

- Bundle Name

  资源包名称



![AssetBundle - AddressableProvider -  AssetPath-Test](Pics\AssetBundle - AddressableProvider -  AssetPath-Test.PNG)

TestPath目录内的所有资源放在testpath资源包内



#### 支持 String.Format 语法

详细语法请参考 [String.Format](String.Format.md)

Format 参数

- AssetPath

  返回 `AssetPath`

##### 样例

文件夹路径

```
{$AssetPath:#DirectoryPath}
```



