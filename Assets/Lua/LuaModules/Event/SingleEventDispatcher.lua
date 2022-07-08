---@class SingleEventDispatcher
local SingleEventDispatcher = class("SingleEventDispatcher")

--- 初始化
function SingleEventDispatcher:ctor()
    self.listeners = {}
    self.dirtyFlags = false --只有增加会影响排序 标记需要重新排序
    self.opList = {}
    self.lock = false
end

--- 添加监听器
---@overload fun(listener:function)
---@param listener function object
---@param context table 上下文，一般为调用者的self对象
---@param level number 优先级，默认为 -255
---@return boolean 添加结果
function SingleEventDispatcher:Add(listener,context, level)
    if type(listener) ~= "function" then
        return false
    end

    ---防止同一事件注册不同优先级
    if self:Has(listener, nil) then
        self:Remove(listener)
    end

    if context~=nil and type(context) ~= "table" then
        level = tonumber(context) or -255
        context = nil
    else
        level = tonumber(level) or -255
    end
    
    

    self.listeners = self.listeners or {}

    local events = self.listeners

    local func = function()
        table.insert(events, { callback = listener, this = context,level = level })
        self.dirtyFlags = true
    end

    if self.lock then
        table.insert(self.opList, func)
    else
        func()
    end

    return true
end

--- 检查是否已存在事件监听器
---@overload fun (listener:function)
---@param listener function 监听器
---@param level table
---@return boolean 是否存在
function SingleEventDispatcher:Has(listener, level)
    
    local events = self.listeners
    if events == nil or #events == 0 then
        return false
    end

    for _, val in ipairs(events) do
        if (listener == nil or val.callback == listener) and (level == nil or val.level == level) then
            return true
        end
    end

    return false
end

--- 删除监听器
---@param listener function 处理函数
---@return boolean 删除状态
function SingleEventDispatcher:Remove(listener)
    local events = self.listeners
    if events == nil or #events == 0 then
        return false
    end

    local removed = false

    if self.lock then
        table.insert(self.opList, function()
            if events then
                for i = #events, 1, -1 do
                    if listener == nil or events[i].callback == listener then
                        table.remove(events, i)
                    end
                end
            end
        end)
        removed = true
    else
        for i = #events, 1, -1 do
            if listener == nil or events[i].callback == listener then
                table.remove(events, i)
                removed = true
            end
        end
    end
    return removed
end

--- 根据名称删除事件下的所有监听器
--- 如果name为空，则删除所有事件的监听器
---@return boolean 是否删除成功
function SingleEventDispatcher:removeAll()
    ---这个方法不用锁
    
        self.listeners = {}
        self.dirtyFlags = false
        return true
end

--- 执行事件
--- 监听器返回true的时候，将停止后续监听器的执行
function SingleEventDispatcher:Dispatch(...)
    
    local events = self.listeners

    if events == nil or #events == 0 then
        return
    end

    if self.dirtyFlags then
        -- 排序
        local function levelSort(a, b)
            return tonumber(a.level) > tonumber(b.level)
        end
        table.sort(events, levelSort)
    end

    self.dirtyFlags = false

    local stop = false

    --锁定列表 防止在回调里改变事件列表
    self.lock = true
    for _, val in ipairs(events) do
        if val.callback ~= nil then
            if val.this ~= nil then
                stop = val.callback(val.this,...)
            else
                stop = val.callback(...)
            end
            if stop == true then
                break
            end
        end
    end

    local opList = self.opList
    self.lock = false
    --解锁列表 处理在事件回调过程中造成的事件列表改变操作
    for i, op in ipairs(opList) do
        op()
        opList[i] = nil
    end
end

----- 打印所有已注册的事件监听器
-----@private
--function SingleEventDispatcher:printListeners()
--
--    local listeners = self.listeners
--    if listeners == nil then
--        return
--    end
--    dump(listeners)
--end

return SingleEventDispatcher