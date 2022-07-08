---@type UIEnums UI所有枚举
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")


---@class OrderType:UITypeBase
local OrderType = class("OrderType", require("LuaModules.UILib.UIType.UITypeBase"))

--region 生命周期
function OrderType:ctor(uiMgr, config)
    OrderType.super.ctor(self, uiMgr, config)
    self.pageType = UIEnums.PageType.order
    ---@field orderIndexes table<string,table> 当前管理中的pageHandlers，结构{pageName，options}
    self.orderIndexes = {}
    ------@field hasPageShowing boolean 当前是否有实例显示中
    self.hasPageShowing = false
end

---open 
---@param pageName string
---@param options table
---@param callback function
function OrderType:open(pageName, options, callback)
    local orderIndexes = self.orderIndexes
    if self.hasPageShowing then
        -- pos不写默认尾插，缓存打开请求
        table.insert(orderIndexes,  {
            pageName = pageName,
            options = options
        })
        return
    end

    OrderType.super.open(self, pageName, options, callback)
    self.hasPageShowing = true
end

---close 
---@param pageName string
---@param options table
function OrderType:close(pageName, options)
    ---@type table<options>
    local orderIndexes = self.orderIndexes

    OrderType.super.close(self, pageName, options)
    self.hasPageShowing = false
    
    if #orderIndexes > 0 then
        local topPageParams = orderIndexes[1]
        table.remove(orderIndexes, 1)
        self:open(topPageParams.pageName, topPageParams.options)
    end
end

---closeAllExclude 覆盖原接口，不支持直接关闭全部，避免需要告知用户的消息丢失
---@param excludePageNames string[]
function OrderType:closeAllExclude(excludePageNames)
    
end
--endregion

return OrderType