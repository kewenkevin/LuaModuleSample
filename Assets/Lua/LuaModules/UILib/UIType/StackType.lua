---@class StackType:UITypeBase
local StackType = class("StackType", require("LuaModules.UILib.UIType.UITypeBase"))

---@type UIEnums UI所有枚举
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")
---@type UIPageHandler
local UIPageHandler = require("LuaModules.UILib.Base.UIPageHandler")

--region 生命周期
function StackType:ctor(uiMgr, config)
    StackType.super.ctor(self, uiMgr, config or {})
    self.pageType = UIEnums.PageType.stack
end

---attach 将pageName指定的页面附加到当前栈顶的页面
---当被附加界面状态改变时，附加界面也会一同改变
---附加界面不能是栈类型界面
---@param pageName string
function StackType:attach(pageName)
    local targetPageConfig = self.uiMgr:getPageConfig(pageName)
    if targetPageConfig.uiType == UIEnums.PageType.stack then
        warn("attach failed: page cannot be a stack type.")
        return
    end
    ---@type UIPageHandler
    local topPageHandler = self:getGroupPageHandler()
    topPageHandler.attached = topPageHandler.attached or {}
    table.insert(topPageHandler.attached, pageName)
end

---close 导航栈回退(back)
---@param options table
function StackType:close(pageName, options)
    if nil == pageName then
        warn("stack close failed: pageName must not be nil.")
        return
    end
    if not self.uiMgr:isStackTop(pageName) then
        warn("stack close failed: " .. pageName .. " not top page.")
        return
    end
    self:back(options)
end

---back 导航栈回退
---@param options table
function StackType:back(options)
    self:popTopUI(options)
    local nextPageHandler = self:getGroupPageHandler()
    if nextPageHandler then
        nextPageHandler:setPageActive(true)
        nextPageHandler:show()
        nextPageHandler:changePageStatus(UIEnums.PageStatus.shown)
        
        if nextPageHandler.mode == UIEnums.PageMode.coexist then
            self:toShowCoexist(2)
        end
    end
end

---closeAllExclude 清空栈，遇到任意一个需要排除的UI则停止退栈
---@param excludePageNames string[]
function StackType:closeAllExclude(excludePageNames)
    if excludePageNames == nil then
        error("StackType:closeAllExclude failed, excludePageNames is nil")
        return
    end
    local backCount = 0
    self:traversalGroupPageHandler(function (pageHandler)
        if self:inArray(pageHandler.pageConfigName, excludePageNames) then
            return true
        end
        backCount = backCount + 1
    end)
    for i = 1, backCount, 1 do
        self:back()
    end
end

---destroy 销毁pageName指定最接近栈顶的page，并从栈上移除该page记录，其附加界面(栈中如果只有一个page则无法销毁)
---@param pageName string
---@param options table
function StackType:destroy(pageName, options)
    local success = self:traversalGroupPageHandler(function (pageHandler)
        if pageHandler.pageConfigName == pageName then
            self:destroyPage(pageHandler, options)
            return true
        end
    end)

    if not success then
        warn("destroy failed, target page not exist: " .. pageName)
    end
end
--endregion

--region 内部API
---@private
---destroyPage 关闭指定的page，并关闭其附加界面
---@param pageHandler UIPageHandler
---@param options table
function StackType:destroyPage(pageHandler, options)
    pageHandler:destroy(options)
end

---@private
---toShowCoexist 显示和索引位置界面共存的栈中其它界面
---@param index number 导航栈中的界面索引
function StackType:toShowCoexist(index)
    local pageHandler = self:getGroupPageHandler(index)
    if pageHandler and pageHandler.status ~= UIEnums.PageStatus.shown then
        pageHandler:setPageActive(true)
        pageHandler:show()
        pageHandler:changePageStatus(UIEnums.PageStatus.shown)
        if pageHandler.mode == UIEnums.PageMode.coexist then
            self:toShowCoexist(index + 1)
        end
    end
end

---@private
---toHideCoexist 隐藏和当前索引位置界面共存的栈中其它界面
---@param index number 导航栈中的界面索引
function StackType:toHideCoexist(index)
    local pageHandler = self:getGroupPageHandler(index)
    if pageHandler and pageHandler.status ~= UIEnums.PageStatus.hided then
        pageHandler:setPageActive(false)
        pageHandler:hide()
        pageHandler:changePageStatus(UIEnums.PageStatus.hided)
        if pageHandler.mode == UIEnums.PageMode.coexist then
            self:toHideCoexist(index + 1)
        end
    end
end

---@private
---popCloseStackTopUI 弹出关闭导航栈顶的UI
---@param options table
function StackType:popTopUI(options)
    local pageHandler = self:getGroupPageHandler()
    if pageHandler == nil then
        warn("popTopUI failed, no page exist")
        return false
    end
    pageHandler:close(options)
    pageHandler:setPageActive(false)
    pageHandler:changePageStatus(UIEnums.PageStatus.closed)

    self:destroyPage(pageHandler)
    return true
end

---@private
---栈顶是否可关闭
function StackType:canBack()
    local pageHandler = self:getGroupPageHandler()
    if pageHandler == nil then
        warn("popTopUI failed, no page exist")
        return true
    end

    return pageHandler:canClose()
end
---open 
---@param pageName string
---@param options table
---@param callback function
function StackType:open(pageName, options, callback)
    local targetPageHandler = UIPageHandler.new(pageName, self.uiMgr)
    self:handlerStackTopPage(targetPageHandler, options)
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

---@private
---handlerStackTopPage 执行导航栈互斥等操作并将page插入栈顶
---@param pageHandler UIPageHandler
---@param options table
function StackType:handlerStackTopPage(pageHandler, options)
    local topPageHandler = self:getGroupPageHandler()
    if topPageHandler ~= nil then
        local op = nil
        if options and options.mode then
            op = options
        else
            if pageHandler.config.mode then
                op = pageHandler.config
            end
        end
        if op == nil then
            op = op or {}
            op.mode = UIEnums.PageMode.exclusive
        end
        if op.mode == nil then
            op.mode = UIEnums.PageMode.exclusive
        end
        pageHandler.mode = op.mode
        if op.mode == UIEnums.PageMode.replace then
            -- 替换
            self:popTopUI(options)
        elseif op.mode == UIEnums.PageMode.coexist then
            -- 共存
        elseif op.mode == UIEnums.PageMode.exclusive then
            -- 互斥
            topPageHandler:hide()
            topPageHandler:changePageStatus(UIEnums.PageStatus.hided)
            topPageHandler:setPageActive(false)
            if topPageHandler.mode == UIEnums.PageMode.coexist then
                --栈顶的下一个的下一个开始隐藏
                self:toHideCoexist(2)
            end
        else
            error("dont support mode: " .. op.mode)
        end 
    end
end
--endregion

return StackType