---@type UIEnums UI所有枚举
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")


---@class UniqueType:UITypeBase
local UniqueType = class("UniqueType", require("LuaModules.UILib.UIType.UITypeBase"))


--region 生命周期
function UniqueType:ctor(uiMgr, config)
    UniqueType.super.ctor(self, uiMgr, config)
    self.pageType = UIEnums.PageType.unique
end

---open 
---@param pageName string
---@param options table
---@param callback function
function UniqueType:open(pageName, options, callback)
    local success = self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            return true
        end
    end)

    if success then
        if callback ~= nil then
            callback(false)
        end
        warn("uniquePage is dont nil:" .. pageName .. " must close")
        return
    end
    
    UniqueType.super.open(self, pageName, options, callback)
end
--endregion


return UniqueType