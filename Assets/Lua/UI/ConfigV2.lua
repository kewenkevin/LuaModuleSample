local UIConfig = require("LuaModules.UILib.Config.UIConfig")
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")
local config = UIConfig.new()

config.basics.orthographicSize = 680
config.basics.cullingMask = 32
---项目可根据需要选择模式，默认不配或1为兼容性最强的混合绑定方式
config.basics.bindMode = 1

config.paths.assetPathPrefix = "Assets/Sample/ResourceManager/ResourcesAssets/UI/GUI/Pages/"
config.paths.luaPathPrefix = "UI/Pages/"
config.paths.luaUIModulesPathPrefix = "UI/ModulePages/"

----Read Config for Excel table-------------------------
local sourceConfig = require("UI.Config")
local pages = sourceConfig.pages
for k, v in pairs(pages) do
    config.pages[k] = UIConfig.UIConfigPage.new(k, v)
end
-------------------------------------------------------
---自定义界面处理
-------------------------------------------------------------
config.mainPage = sourceConfig.mainPage
-----------------------------------------------------------
return config