---@type UIEnums UI所有枚举
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")


---@class FreedomType:UITypeBase 自由类型是一个基本没有额外规则的类型，基本遵照了UIBase的原始设计，后期迭代时可能会增加特性
local FreedomType = class("FreedomType", require("LuaModules.UILib.UIType.UITypeBase"))

--region 生命周期
function FreedomType:ctor(uiMgr, config)
    FreedomType.super.ctor(self, uiMgr, config)
    self.pageType = UIEnums.PageType.freedom
end
--endregion

return FreedomType
