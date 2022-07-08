# Changelog

All notable changes to this package will be documented in this file.


## [1.4.0 - preview] 2021-07-21

### [Improvement]

-   修改接口用来适配YGUI新增列表组件ListView
-   同步YText字体和颜色样式控制器所对应的样式选择器体验
-   重构，与YGUI解耦，依赖Core层包
-   给YRating的Current和total增加控制器逻辑
-   控制器编辑时屏蔽 lua 关键字
-   [本对化方案V3，内部项目调研改进版](https://youzu.feishu.cn/docs/doccn8ZbWu1IledIs9ZFgXtiCdf)


## [1.3.7 - preview] 2021-03-04

### [Improvement]
-  修改 对YGUI 包的 最小依赖1.1.6

## [1.3.6 - preview] 2021-03-03

### [Improvement]

- 将UIExpansion编辑器配置面板迁移至Project Settings面板

### [Fix]

- 在Controller内，使用YGUI中的TextEditorUntility.DrawTextColorStyleField进行TextColor相关界面的绘制（统一设置体验）。
- 优化Controller的编辑窗口左侧的Gear添加列表显示（修复展开的Gear列表会有一点点显示不全）

## [1.3.6] 2021-02-20

### [Fix]

- 修改属性绑定导出Vector2类型生成代码bug


## [1.3.5] 2021-01-28

### [Fix]

- 修复TextFontGear和TextColorGear使用了Editor代码导致打包报错的bug



## [1.3.4] 2021-01-28

### [Fix]

- 控制器导出不能正常获取问题
- 控制器属性枚举改为显示定义



## [1.3.4] 2021-01-28

### [Fix]

- 控制器属性枚举中间插入定义bug

## [1.3.2] 2021-01-28

### [Fix]

- 控制器属性枚举中间插入定义bug

## [1.3.1] 2021-01-20

### [Fix]

- Lua代码导出，UIPanel改名UIWidget支持

## [1.3.0] 2021-01-20

### [Feature]

- Controller新增支持YTextFontStyle和YTextColorStyle切换
- Controller新增支持对本体对象的状态操作控制
- Controller新增支持OverallAlpha功能，用来设置对象整体Alpha变化功能
- Controller页签备注信息现在显示在UIExpansion Controller页面页签Tips中
- Controller新增支持Image Material切换
- Controller新增PercentPosition, Position位置随Canvas适配调整
- Controller新增获取当前所选状态接口

## [1.2.2] 2021-01-18

### [Fix]

- 修复因自动生成UIBinder时，通过反射方式获取路径不正确而引起打包过程中断的bug

## [1.2.1] 2021-01-13
### [Fix]
- 修复生成Binder脚本时若目标路径不存在引起的报错

## [1.2.0] 2021-01-07

### [Feature]

- 重构UIExpansion Inspector GUI代码，使其显示符合预期，并且增加对预制体模组的操作锁，现在如果预制体不在预制体mode中将无法修改其绑定label，但是可以允许查
- UIExpansionWindow面板现在将正确隐藏所有UIExpansion节点的子对象，现在无论是预制体的UIExpansion还是场景内单挂UIExpansion节点 现在统一在父类UIExpansion Window面板内只允许绑定UIExpansion Model Name（模组名） 用来防止重重复绑定导致数据错乱的问题，对节点和子对象的绑定推荐通过Inspector面板修改
- Controller添加对RawImageColor切换的支持
- 将Controller新建或修改页面改为常驻式窗口 防止在制作复杂界面时因误操作导致工作数据丢失
- UIExpansion Config Window配置中添加添加强制简洁模式设置 方便美术一键全局简洁，同时Inspector面板也提供单独开关 开关单个UIExpansion的显示
- 添加界面打开关闭动画配置功能
- UIEXpansionWindow的Display模式和Search支持子模块ModuleName的显示
- 添加Debug提示日志 用于debug
- 控制器名导出枚举

### [Fix]

- 修复因特殊操作导致UIExpansionWindow界面onEnable函数无法正常调用时显示异常的bug，修复UIExpansionWindow面板强制Refresh功能
- 绑定和导出分行显示，避免过长显示不完整
- 修复UIExpansion面板下已有Controller点击折叠或新建后导致原有显示控制器不显示的bug
- 修复部分电脑可能因为Complier流程非正常执行或中断导致Binder和Linker类型未能正确注册而引起label报错
- UIExpansitionWindows中page可以给模块绑定name，模块自身也可以绑定name，一个模块会有两个name的问题
- 修复UIBinder自动生成工具在Compiler时通过反射取程序集时，有时候取不到，导致触发TryCatch报错
- 修复跨线程搜索器路径问题
- 修复animation控件播放不存在动画时未能正确执行分支逻辑并抛出异常的bug
- 修复inspector面板自动保存功能 缓一帧执行 防止出现不同机型同帧事件触发错乱问题
- 修复Controller和transition面板关闭后 右侧inspector面板重绘多次的问题
- 修改代码自动生成TableView等容器的属性绑定类型不正确问题
- 修改代码自动生成生成方法没有和page名保持一致问题
- 修复Controller控制器SizeDelta属性失效报错的bug
- 修复一个对象的绑定未能使用同一个实例的bug






## [1.1.5] 2020-11-25

### [Feature]

- 提供了全新的UIBinder生成工具 免去了从前还需要手动register的烦恼，但是注意原显目内Customer Binder文件需要手动修改成新版本格式





## [1.1.4] 2020-11-25

### [Fix]

- 代码整理，去除无用功能 剔除UIExpansion Editor Window上 lua export功能和相关设置 无用文件一并剔除
- 修复Controller控制器SizeDelta属性失效报错的bug




## [1.1.3] 2020-11-24

### [Fix]

- 修复在缺少config配置时，在特殊情况下会导致编辑器报错，现在移除报错，提供更鲜明的提示方式提示用户去设置配置
- 修复列表刷新解绑一系列相关问题




## [1.1.2] 2020-11-23

### [Feature]

- Lua绑定生成代码规范化


## [1.1.1] 2020-11-23

### [Fix]

- 修复Transition动效在播放完成时，因在不正确时机清空持有的Tweener对象，而引起的访空报错
- 修复因Transition动效初次构建时未能正确构建，而引起在runtime下动效播放异常的bug
- 修复因模组或界面lua生成工具的初始化过程在UIExpansionEditor OnEnable生命周期内，导致当选中挂载了UIExpansion的GameObject时，且延迟生成UIExpansionEditorConfig配置后，不能及时构建luaExporter而导致的报空错误




## [1.0.37] 2020-11-19

### [Fix]

- 修复因错误过滤binding label标签导致保存失效并重复提示警告的bug
- 修复因新特性 控制器备注名 对过往老版本预制体上控制器不兼容引起的bug




## [1.0.36] 2020-11-18

### [Feature]

- 绑定脚本未能正常保存

## [1.0.35] 2020-11-17

### [Feature]

- 数据绑定导出列表类型的支持

## [1.0.34] 2020-11-17

### [Feature]

- 控件绑定及导出功能


## [1.0.33] 2020-11-17

### [Feature]

- Editor窗口Binding面板 添加Display模式下搜索绑定label功能 方便程序定位绑定关系




## [1.0.32] 2020-11-17

### [Feature]

- 添加绑定属性导出功能，抛弃原有ExportLua的方式，新的导出方式可以在已经编辑好的lua文件中追加导出，不会影响原有代码。
- 整合绑定脚本与绑定属性导出功能




## [1.0.31] 2020-11-13

### [Feature]

- 添加UIExpansion Editor Config Window作为UIExpansion编辑器配置面板，同时新增Inpsector面板下对于lua module文件的绑定功能




## [1.0.30] 2020-11-12

### [Feature]

- 增加指定一个lua绑定的参数属性，Bind在Invoke其getItemCls时传入



## [1.0.27] 2020-11-09

### [Feature]

- 优化快捷绑定界面操作舒适度，现在在Runtime模式下不再提醒不能保存的提示，改为面板直接禁止修改仅预览模式

### [Fix]

- 修复UIExpasionWindow和UIExpansionBindingInspectorHeaderGUI文件内对unity2018适配问题




## [1.0.26] 2020-11-05

### [Fix]

- 修复因Unity调整Awake()执行生命周期而引起的复杂结构prefab中，子类UIExpansion初始化失败的bug




## [1.0.25] 2020-11-05

### [Fix]

- 修复UIExpansion PauseTransiton未将切换变量正确应用的bug




## [1.0.24] 2020-11-05

### [Fix]

- 修复因错误引用Namespace而引起的编译报错




## [1.0.23] 2020-11-04

### [Feature]

- 在GameObject Inspector Header处新增便捷binding面板，以便开发者快速定位并绑定Label，默认AutoSave，同时也提供Save按钮

### [Fix]

- 添加缺失的C#侧回调方法以供lua层调用，需要Re generator UIExpansion wrap file




## [1.0.22] 2020-10-29

### [Feature]

- UIExpansion Inspec面板t添加runtime 动效快速播放按钮
- UIExpansion添加PlayAllTransitions接口 功能为播放此UIExpansion下所有动效1次(需要重新生成 UIExpansion Wrap lua层才可以调用)
- UIExpansion控制器controller新增标签备注功能，允许开发者对标签页进行中文备注，优化开发体验

### [Fix]

- 修复UIExpansionWindow Runtime切换UIExpansion显示对象时因未正确遍历对象parent终结点为空时而引起的bug
- 修复binding面板快速预览功能 2019与2018版本的兼容问题




## [1.0.21] 2020-10-21

### [Feature]

- UIExpansion编辑器增加支持Runtime时显示功能，可在runtime时查看调整参数用于预览，但是不允许保存，保存必须是Editor模式
- 添加支持Hierarchy界面点选，若界面上存在多个UIExpansion组件将按照点选目标自动切换显示，点击child对象也将自动寻找到父对象UIExpansion组件用于显示