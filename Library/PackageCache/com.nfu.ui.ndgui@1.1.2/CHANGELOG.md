# Changelog

All notable changes to this package will be documented in this file. 

## [1.2.0-preview] 2021-07-21

### [Improvement]

-   重构不再依赖UIExpansion的包，只依赖UI的Core层包
-   将YRating的total和current抽象到Core包接口IRating中并实现
-   [本对化方案V3，内部项目调研改进版](https://youzu.feishu.cn/docs/doccn8ZbWu1IledIs9ZFgXtiCdf)
-   ybutton组件在栈类型面板中使用的scaletween时的异常修复
-   YImage新增支持Cloned Material 以供对象单独修改特有属性
-	新增列表组件ListView，以供项目组选择使用，UMTTutorial中有演示Demo

## [1.1.9-preview] 2021-05-13

### [Bug/Fix]
- 修复YRichText中delegate方法OnGetPrefab和OnReleasePrefab被错误标识成static，导致无法按实列对象定制回调的问题

## [1.1.8-preview] 2021-05-12

### [Bug/Fix]
- 修复YList在同一帧下同时删除和赋值对象时，因unity getchild方法未能正确获得子对象引起的访问空对象异常

## [1.1.7-preview] 2021-04-21

### [Bug/Fix]
- ybutton组件在栈类型面板中使用的scaletween时的异常修复

### [Improvement]
-   优化YTextStyle（字型样式)和YTextColorStyle（颜色样式）的选择体验
-   重构不再依赖UIExpansion的包，只依赖UI的Core层包
-   将YRating的total和current抽象到Core包接口IRating中并实现
-   [本对化方案V3，内部项目调研改进版](https://youzu.feishu.cn/docs/doccn8ZbWu1IledIs9ZFgXtiCdf)
-   ybutton组件在栈类型面板中使用的scaletween时的异常修复
-   YImage新增支持Cloned Material 以供对象单独修改特有属性


## [1.1.6-preview] 2021-03-04

### [Improvement]

-   新增示例组件用法的Sample
	  - 字体样式和字色样式
	  - 示例用的原子和模组库（不含逻辑）
-   编译器下隐藏package 的 samples目录，避免导入samples到工程中有meta冲突


### [Bug/Fix]
-   优化YText/YRichText 在属性查看（Inspector）中 ColorStyle 选择按钮的显示位置
-   修复创建YTextColorStyle资源时的初始化报错



## [1.1.5] 2021-02-24

### [Bug/Fix]

- YTogglegroup 绑定 seleted 无效bug

## [1.1.4] 2021-01-19

### [Bug/Fix]
- YScrollView创建Prefab时报错



## [1.1.3] 2021-01-15

### [Bug/Fix]
- YList 同一帧增删导致数据绑定异常




## [1.1.2] 2021-01-14

### [Bug/Fix]
- YButtonEditor(Inspector) 切换开启文字样式设置时的自动初始化Button Text修复




## [1.1.1] 2021-01-08

### [Bug/Fix]
- YButton 不可交互态的时候scaletween也禁用




## [1.1.0] 2021-01-07

### [Bug/Fix]
- 整合 Horizontal Rating 和 Vertical Rating 到 YRating
- YRating在更新Sprite资源后会重设对应的Image到NativeSize
- 修复YText的Color在Animation编辑动画的过程中被错误更新为当前颜色样式的问题
- YText和YRichText在创建时会使用默认的字体样式和颜色样式
- 当颜色样式为空时可以通过原始的Color属性修改文字颜色


### [Feature]
- YText修改颜色样式的窗口改为可视化的颜色样式选择器
- YImage, YButton, YScrollbar 中的Image相关的材质和颜色不可在编辑模式下的Prefab实例中被修改
- YButton, YToggle, YDropDown, YInputField 恢复 Selection 父类中的transition设置，同时不可在编辑模式下的Prefab实例中被修改
- 删除 YButton 的 ColorTween 因与父类 Selection 中 Transition 的 ColorTint 效果重复
- YTextColorStyle增加二色垂直渐变的效果实现




## [1.0.37] 2020-11-26

### [Fix]

- 修复YList控件在Editor模式下预制的对象无法绑定问题
- 增加YToggleGroup的当前选中索引以及相关变化事件

## [1.0.36] 2020-11-25

### [Fix]

- 修复YText原来的多语言系统在错误的时机调取了ConfigManager本地文件导致报错




## [1.0.35] 2020-11-13

### [Feature]

- 修复YList空间在Editor模式下生成的子对象与预制体断联的bug




## [1.0.34] 2020-11-13

### [Feature]

- 按钮Drag事件从YButton中剥离，新建名为YDragButton的控件，来防止吞并拖拽事件导致ScrollRect控件无法操作的问题

P.S UIExpansion需要执行ReBuildBinder(BinderCustomSettings内需要添加YDragButton)，并且注册新生成的Binder和TreeItem




## [1.0.33] 2020-11-09

### [Feature]

- YButton添加新拖拽事件
	- OnDrag(mouse.ScreenPosition : Vector2) 当拖拽时自动持续触发
	- OnDragOut(mouse.ScreenPosition : Vector2) 当鼠标拖拽出按钮框时自动持续触发
	- OnDragBegin(mouse.ScreenPosition : Vector2) 当拖拽开始时自动触发一次
	- OnDragEnd(mouse.ScreenPosition : Vector2) 当拖拽结束时自动触发一次
	- OnDragOutEnd(mouse.ScreenPosition : Vector2) 当拖拽结束并在框外时自动触发一次
P.S 需要执行ReBuildBinder




## [1.0.32] 2020-11-06

### [Fix]

- 修复YScrollView内因缺少宏定义和书写不规范引起的编译错误




## [1.0.31] 2020-11-06

### [Fix]

- 修复YScrollView内Namespace错误引用的bug




## [1.0.30] 2020-11-06

### [Fix]

- 修复YScrollView内确实分号而引起编译错误的bug




## [1.0.29] 2020-11-05

### [Fix]

- 修复YTableViewEditor在2018版本下绘制Inspector部分数据时未能完整显示的bug