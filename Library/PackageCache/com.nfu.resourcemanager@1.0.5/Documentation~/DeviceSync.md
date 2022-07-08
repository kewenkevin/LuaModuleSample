# 设备同步

直接同步最新的资源包到 `Android` 设备或模拟器上，加快开发测试，避免同时使用热更服务器。



**注意**

- 仅支持 Android 设备或模拟器同步

## 功能介绍

![AssetBundle - DeviceSync](Pics\AssetBundle - DeviceSync.PNG)

菜单 `Device Sync` 打开 `Resource Device Sync` 设备同步窗口

- Device

  设备列表，如果未显示点击 `Refresh` 刷新按钮

- Package

  包名，`Application.identifier`

- Help

  帮助信息
  
- Local

  本地资源包版本信息，下拉列表包含 `Output Path` 输出目录已生成的资源包版本号，可以选择不同的版本同步到设备上。

  如果没有信息，则需要先选择 `Build` 菜单生成一次资源包

- Applicable Game Version

  游戏版本号

- Internal Resource Version

  内部资源版本号

- Remote

  设备或模拟器远程资源版本信息。

  如果没有信息，按如下操作

  - 第一次使用同步，需要先运行游戏完成资源管理器初始化配置
  - 开启热更模式 `Resource Mode` = `Updatable`
  - 设置资源版本号增量模式 `Resource Update Check Mode` = `Normal`

- Total (#TotalFile) #TotalSize

  统计版本比较所有变化的文件数量和大小

- Changed (#Total) #Size

  发生修改的资源列表

- Added (#Total) #Size

  新增的资源列表

- Removed (#Total) #Size

  移除的资源列表

- Refresh 按钮

  刷新设备列表，本地版本列表，远程版本

- Sync 按钮

  点击进行同步

## 可能的问题

- `Device` 设备下拉菜单没有列出设备

  模拟器自带的 `adb` 和 Unity 使用的 `adb` 版本不一致

  ```tex
  Android_SDK_HOME\platform-tools\adb.exe
  ```

  夜神模拟器替换

  ```tex
C:\Program Files (x86)\Nox\bin\nox_adb.exe
  ```
  
  