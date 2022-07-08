---@type UIEnums UI所有枚举
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")
---@type UIPageHandler
local UIPageHandler = require("LuaModules.UILib.Base.UIPageHandler")

---@class UITypeBase
local UITypeBase = class("UITypeBase")

--region 生命周期
---@param uiMgr UIManager
function UITypeBase:ctor(uiMgr, config)
    ---@field pageType PageType
    self.pageType = nil
    ---@field uiMgr UIManager
    self.uiMgr = uiMgr
    ---@field config UIConfigBasic
    self.config = config
end

---open 
---@param pageName string
---@param options table
---@param callback function
function UITypeBase:open(pageName, options, callback)
    local targetPageHandler = UIPageHandler.new(pageName, self.uiMgr)
    targetPageHandler:addPageHandlerToGroup()
    targetPageHandler:instantiate(handler(self, function(self, newPageHandler)
        if newPageHandler.status == UIEnums.PageStatus.closed then
            warn("uiMgr:newPage failed:" .. pageName .. "  page is closed")
            newPageHandler:removePageHandlerFromGroup()
            if callback ~= nil then
                callback(false)
            end
        else
            self:toOpen(newPageHandler, options)
            if callback ~= nil then
                callback(true)
            end
        end
    end),options)
end

---show
---@param pageName string
---@param options table
function UITypeBase:show(pageName, options)
    local success = self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            pageHandler:show(options)
            pageHandler:setPageActive(true)
            return true
        end
    end)

    if not success then
        warn("show failed, target page not exist: " .. pageName .. "  please try open first!!") 
    end
end

---attach 将page附加到当前策略管理中，不同策略下实现不同
---@param pageName string
function UITypeBase:attach(pageName)
end

---hide
---@param pageName string
---@param options table
function UITypeBase:hide(pageName, options)
    local success = self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            pageHandler:hide(options)
            pageHandler:setPageActive(false)
            return true
        end
    end)

    if not success then
        warn("hide failed, target page not exist: " .. pageName)
    end
end

function UITypeBase:close(pageName, options)
    local success = self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            if pageHandler.status == UIEnums.PageStatus.closed then
                print(tag, "page is dont opened:", pageName)
                return true
            end
            pageHandler:close(options)
            pageHandler:changePageStatus(UIEnums.PageStatus.closed)
            pageHandler:setPageActive(false)
            pageHandler:destroy(options)
            return true
        end
    end)

    if not success then
        warn("close failed, target page not exist: " .. pageName) 
    end
end

---closeAllExclude 关闭group内的全部UI除了指定的Page例外
---@param excludePageNames string[]
function UITypeBase:closeAllExclude(excludePageNames)
    self:traversalGroupPageHandler(function (pageHandler)
        if nil ~= pageHandler and not self:inArray(pageHandler.pageConfigName, excludePageNames) then
            self:close(pageHandler.pageConfigName)
        end
    end)
end

---isShowing 检查当前显示UI是否有pageName
---@param pageName string
---@return boolean
function UITypeBase:isShowing(pageName)
    local showState = false
    local success = self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            showState = pageHandler.status == UIEnums.PageStatus.shown or pageHandler.status == UIEnums.PageStatus.opened
            return true
        end
    end)
    
    return showState
end

function UITypeBase:destroy(pageName, options)
    local success = self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            pageHandler:destroy(options)
            return true
        end
    end)

    if not success then
        warn("destroy failed, target page not exist: " .. pageName) 
    end
end
--endregion

--region 策略类内部API
---@protected
function UITypeBase:inArray(targetItem, targetArray)
    local isIn = false
    if targetArray ~= nil then
        for i, arrayItem in ipairs(targetArray) do
            if arrayItem == targetItem then
                isIn = true
                break
            end
        end
    end
    return isIn
end

---@protected
---@param pageHandler UIPageHandler
function UITypeBase:toOpen(pageHandler, options)
    pageHandler:setPageActive(true)
    pageHandler:open(options)
    pageHandler:changePageStatus(UIEnums.PageStatus.opened)

    pageHandler:afterOpen(options)
end

---pageInfo  获取界面的状态
---@param group string 组名
---@param pageName string 界面名
---@return UIEnums.PageStatus
function UITypeBase:getPageState(pageName)
    local status = UIEnums.PageStatus.notExist
    self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            status = pageHandler.status
            return true
        end
    end)
    
    return status
end

---getGroupPageHandler 获取对应策略组中的pageHandler
---@param invertedIndex number 倒序索引，1是倒数第一个,缺省为1
function UITypeBase:getGroupPageHandler(invertedIndex)
    return self.uiMgr:_getGroupPageHandler(self.pageType, invertedIndex)
end

---traversalGroupPageHandler 倒序遍历策略组的全部pageHandler，如果遍历结束lookfunc没有返回true，则整体返回false
---@param lookFunc function function(UIPageHandler):boolean 检查pageHandler的函数，返回true则中止遍历，否则继续遍历
function UITypeBase:traversalGroupPageHandler(lookFunc)
    return self.uiMgr:_traversalGroupPageHandler(self.pageType, lookFunc)
end

---getGroupPageHandlerCount 获取对应策略组中的页面数量
function UITypeBase:getGroupPageHandlerCount()
    return self.uiMgr:_getGroupPageHandlerCount(self.pageType)
end
--endregion

return UITypeBase