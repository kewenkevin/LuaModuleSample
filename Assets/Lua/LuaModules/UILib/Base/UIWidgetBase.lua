--[[
UI生命周期：
]]

---@type UIEnums UI所有枚举
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")

local EventDispatcher = require("LuaModules.Event.EventDispatcher")

---@class UIWidgetBase : UIBase
---@field private mappedDataStore DataStore
---@field public eventBus EventDispatcher
local UIWidgetBase = class("UIWidgetBase",require("LuaModules.UILib.Base.UIBase"))
local tag = "panelBase"

--region LifeCycle Methods

---onCreated 界面复写 界面复写 在Panel创建实例时触发，一个实例只会触发一次，这时已完成进行数据绑定
---@param obj UnityEngine.GameObject
---@param config UIConfigPage
function UIWidgetBase:onCreated(obj,options)
end

---onDestroy 界面复写 在Page实例销毁时触发
---@param options table
function UIWidgetBase:onDestroy(options)
end

--endregion


function UIWidgetBase:ctor(...)
    self.destroyed = false
    self.eventBus = EventDispatcher.new()
    self.modules = self.modules or {}

--[[
    if self.data then
        self.data = self:data()
    end

    if self.observerUIData then
        self.observerUIData = self:observerUIData()
    end
]]

    if self.methods then
        self.methods = self:methods()
    end
    
    if self.onPreData then
        self:onPreData()
    end
end


---created 管理器调用，界面不要复写
---@param obj UnityEngine.GameObject
function UIWidgetBase:created(obj,options)
    self.gameObject = obj
    UIWidgetBase.super.created(self,self.uiExpansion)
    self:onCreated(obj,options)
end


---destroy 管理器调用，界面不要复写
---@param options table
function UIWidgetBase:destroy(options)
    self.destroyed = true
    
    self:unBindDataStore()
    self:unBind()

    self.eventBus:dispatch("UIPanelDestroyed")
    self.eventBus:removeAllListeners()
    self:onDestroy(options)
end


--region DataStore Support

function UIWidgetBase:BindDataStore(dataStore)
    self:unBindDataStore()
    self.mappedDataStore = dataStore
    if self.mappedDataStore then
        if self.observerDataStore then
            self.observerDataStore = self:observerDataStore()
            self.mappedDataStore:mapAllStates(self.observerDataStore)
        end
        if self.listen then
            self.listen = self:listen()
            self.mappedDataStore:listenAllCall(self.listen)
        end
    end
end

function UIWidgetBase:unBindDataStore(dataStore)
    if self.mappedDataStore then
        if self.observerDataStore then
            self.mappedDataStore:unmapAllStates(self.observerDataStore)
        end
        if self.listen then
            self.mappedDataStore:unListenAllCall(self.listen)
        end
    end
end

--endregion

return UIWidgetBase