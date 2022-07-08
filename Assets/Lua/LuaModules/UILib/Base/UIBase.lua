--[[
---@type UIDataStorageFactory
--local UIDataStorageFactory = require("LuaModules.UILib.Base.UIDataStorageFactory")
]]
---@class UIBase
--[[
---@field linkerDatas any[]
---@field moduleDatas any[]
---@field linkerDataDict table<string,any>
]]
---@field isBinded boolean 是否已经处于绑定中
local UIBase = class("UIBase")

local tag = "UIBase"

---@field modules <moduleName, luaScript>
function UIBase:ctor(...)
    self.modules = self.modules or {}
end

--[[
--@type function
function UIBase:_wrapData(uiExpansion, data, methods, modules, observerUIData)
    self.data = UIDataStorageFactory.generate(uiExpansion, data, methods, modules, observerUIData, self)
end
]]

--@field name string
--@field requireName string
--@field obj GameObject
function UIBase:registerModule(name, requireName, obj)
    if name == nil or obj == nil or name == "" then
        error("NFU RegisterModule", "name == nil or moduleobj == nil")
        return
    end

    if self.modules[name] then
        error("NFU RegisterModule", "Not Bind Module Name Of " .. name .. " Two")
        return
    end

    self.modules[name] = {}
    local module = self.modules[name]

    local uiExpansion = nil
    if C_UIExpansion then
        uiExpansion = obj:GetComponent(typeof(C_UIExpansion))
    end

    if uiExpansion == nil then
        error("NFU RegisterModule","Not a Bindable GameObject, Please Add UIExpansion Component First")
    end

    if G_DEFINE_UI_LUA_BIND_MODE == 2 or nil == uiExpansion then
        --强制代码绑定方式 忽略资源绑定
        module = require(self.config.luaUIModulesPathPrefix .. requireName).new()
    elseif G_DEFINE_UI_LUA_BIND_MODE == 0 then
        --强制资源绑定模式
        module = require(uiExpansion.LuaBindPath).new()
    else
        --灵活模式，资源绑定优先，代码次之
        local prefabPath = uiExpansion.LuaBindPath
        if prefabPath ~= nil and prefabPath ~= "" then
            module = require(prefabPath).new()
        else
            module = require(self.config.luaUIModulesPathPrefix .. requireName).new()
        end
    end

    module.setExpansion(module, uiExpansion)
    
    module.created(module, obj, self, self.methods)

    self.modules[name] = module
    
    return module
end

---getModule 获取某个模块
---@param moduleName string
function UIBase:getModule(moduleName)
    local v = self.modules[moduleName]
    if not v then
        error("dont has this moduleName  " .. moduleName)
        return nil;
    end
    
    return v
end

---unRegisterModule 反注册一个名字为modulesName的模块
---@param modulesName string
function UIBase:unRegisterModule(modulesName)
    if type(modulesName) == "string" then
        local v = self.modules[modulesName]
        self:doUnBind(v)
        self.modules[modulesName] = nil
    end
end

---unRegisterGroup 反注册一组Module
---@param modules table<string, table> 反注册的一组，按照table键值反注册
function UIBase:unRegisterModules(modules)
    if type(modules) == "table" then
        for modulesName, v in pairs(modules) do
            self:unRegisterModule(modulesName)
        end
    end
end

function UIBase:created()
    if self.uiExpansion == nil then
        return
    end
    self:bind(self.uiExpansion)
end

---@param uiExpansion UIExpansion
function UIBase:bind(uiExpansion)
    if uiExpansion == nil or self.isBinded then
        return
    end
--[[
    --可绑定的数据
    self.linkerDatas = uiExpansion:GetLinkerDatas()
    --可绑定的UIExpension对应模组
    self.moduleDatas = uiExpansion:GetModuleDatas()
    --可绑定的特殊动态模组
    self.moduleLinkerDatas = uiExpansion:GetModuleContainerLinkerDatas()
    --记录类型，绑定时要用
    self.linkerDataDict = {}
    if self.linkerDatas ~= nil then
        for i = 0, self.linkerDatas.Length - 1 do
            local linker = self.linkerDatas[i]
            self.linkerDataDict[linker.Label] = linker.ValueTypeId
        end
    end

    self.data = self.data or {}
    self.methods = self.methods or {}
    self.observerUIData = self.observerUIData or {}
]]
    self.uiExpansion = uiExpansion
    
--[[
    --组件绑定
    if self.bindComponents then
        self:bindComponents()
    end
]]

    --组件绑定
    if self.initAutoBind then
        self:initAutoBind(self.gameObject)
    end
    if self.bindController then
        self:bindController()
    end
--[[
    --包裹绑定数据到元表
    self:_wrapData(uiExpansion, self.data, self.methods, self.modules, self.observerUIData)
]]

    self.isBinded = true
    self:onBinding()
end

---onBinding 在数据绑定完成后触发，当Module存在复用时，每次会重新触发
function UIBase:onBinding()

end

function UIBase:getDataSore()
    if self.parent then
        return self.parent:getDataSore()
    else
        return nil
    end
end

---@private
function UIBase:doUnBind(v)
    ---todo 需要递归解绑数组
    if v.uiExpansion ~= nil and not v.uiExpansion:Equals(nil) then
        v.uiExpansion:RemoveAllAction()
    end
    if v.unBind and v.isBinded then
        v:unBind()
    end
end

function UIBase:unBind()
    self.isBinded = false
    
    if self.uiExpansion == nil then
        return
    end
    self.uiExpansion = nil
--[[
    local meta = getmetatable(self.data)
    if meta and meta.revertDataToRawTable then
        self.data = meta.revertDataToRawTable(self.data)
    end
]]
    for key, v in pairs(self.modules) do
       --[[
	    if self.linkerDataDict[key] == 11 then
            for _, item in ipairs(v) do
                self:doUnBind(item)
            end
        else
            self:doUnBind(v)
        end
]]
        self:doUnBind(v)
    end
    self.modules = {}
    self:onUnBinding()
end

---onUnBinding 在解除绑定后触发，当Module存在复用时，每次会重新触发
function UIBase:onUnBinding()

end

function UIBase:setExpansion(uiExpansion)
    self.uiExpansion = uiExpansion
--[[
    if self.bindComponents then
        self:bindComponents()
    end 
  ]]
end

--[[
function UIBase:mergeData(newData, defaultData)
    if newData == nil then
        return defaultData
    end

    if type(newData) == "table" then
        --当newData不是被合并过的数据时
        if not rawget(newData, "___isNew") then
            if defaultData == nil then
                defaultData = {}
            end
            local look_up = {}

            for i, v in pairs(defaultData) do
                local old = v
                local new = newData[i]
                if old ~= nil then
                    look_up[i] = true
                    if type(old) == "table" then
                        --【preview】这个if是加入的用来处理数组嵌套的一种特殊情况，老的列表项部分数据被保存下来，没有清除
                        if type(i) == "number" and new == nil then
                            defaultData[i] = nil
                        else
                            defaultData[i] = self:mergeData(new, old)
                        end
                    elseif new ~= nil then
                        defaultData[i] = new;
                    end
                end
            end
            for i, v in pairs(newData) do
                if not look_up[i] then
                    if type(v) == "table" then
                        defaultData[i] = table.clone(v)
                        rawset(defaultData[i], "___isNew", true)
                    elseif v ~= nil then
                        defaultData[i] = v
                    end
                end
            end
            rawset(defaultData, "___isNew", true)
            --当newData是被合并过的数据时,则该数据允许被污染
        else
            if defaultData ~= nil then
                for i, v in pairs(defaultData) do
                    --【preview】and后是加入的用来处理数组嵌套的一种特殊情况，老的列表项部分数据被保存下来，没有清除
                    if newData[i] == nil and not type(i) == "number" then
                        newData[i] = v
                    end
                end
            end
            defaultData = newData
        end
    elseif type(newData) == "function" then
        if defaultData ~= nil then
            local nd = newData()
            if type(defaultData) == "function" then
                defaultData = self:mergeData(nd, defaultData())
            else
                defaultData = self:mergeData(nd, defaultData)
            end
        end
    else
        defaultData = newData
    end

    return defaultData
end
]]
function UIBase:uninjectObservers(observerHandler)
    if self._injectObserverMethod == nil then
        return
    end
    for i, v in ipairs(self._injectObserverMethod) do
        if v == observerHandler then
            table.remove(self._injectObserverMethod, i)
            return
        end
    end
end

function UIBase:injectObservers(observerHandler)
    if self._injectObserverMethod == nil then
        self._injectObserverMethod = { observerHandler }
    else
        table.insert(self._injectObserverMethod, observerHandler)
    end
end

function UIBase:_trigObservers(trigger, key)
    if self._injectObserverMethod then
        for i, v in ipairs(self._injectObserverMethod) do
            v(trigger, key)
        end
    end
    if self.parent then
        self.parent:_trigObservers(trigger, key)
    end
end

function UIBase:uninjectCheck(checkHandler)
    if self._injectCheckMethod == nil then
        return
    end
    for i, v in ipairs(self._injectCheckMethod) do
        if v == checkHandler then
            table.remove(self._injectCheckMethod, i)
            return
        end
    end
end

function UIBase:injectCheck(checkHandler)
    if self._injectCheckMethod == nil then
        self._injectCheckMethod = { checkHandler }
    else
        table.insert(self._injectCheckMethod, checkHandler)
    end
end

function UIBase:_trigCheck(trigger, key, value)
    if self._injectCheckMethod then
        for i, v in ipairs(self._injectCheckMethod) do
            if not v(trigger, key, value) then
                return false
            end
        end
    end
    if self.parent then
        return self.parent:_trigCheck(trigger, key, value)
    end
    return true
end

function UIBase:setController(label, index)
    if not label then
        error("setController must not nil")
    end
    index = index or 0

    self.uiExpansion:SetController(label, index)
end

function UIBase:getControllerSelectedIndex(ctrName)
    local ctrIndex = self.uiExpansion:GetControllerSelectedIndex(ctrName)
    if ctrIndex < 0 then
        error("Controller not exist, ctrName: " .. ctrName)
    else
        return ctrIndex
    end
end

function UIBase:getControllerSelectedPageName(ctrName)
    local pageName = self.uiExpansion:GetControllerSelectedPageName(ctrName)
    if pageName == nil then
        error("Controller not exist, ctrName: " .. ctrName)
    end
    return pageName
end

function UIBase:playTransiton(transitionName, times, delay, onComplete, reverse)
    times = times or 1
    delay = delay or 0
    reverse = reverse or false
    self.uiExpansion:PlayTransition(transitionName, times, delay, onComplete, reverse)
end

function UIBase:pauseTransiton(transitionName)
    self.uiExpansion:PauseTransition(transitionName, true)
end

function UIBase:resumeTransiton(transitionName)
    self.uiExpansion:PauseTransition(transitionName, false)
end

function UIBase:stopTransiton(transitionName)
    self.uiExpansion:StopTransition(transitionName)
end

function UIBase:playAnimation(animName, onComplete)
    self.uiExpansion:PlayAnimation(animName, onComplete)
end

function UIBase:stopAnimation(animName)
    self.uiExpansion:StopAnimation(animName)
end

return UIBase