---@class UIModuleBase : UIBase
local moduleBase = class("moduleBase",require("LuaModules.UILib.Base.UIBase"))
local tag = "moduleBase"

function moduleBase:created(--[[newData,emitMethods,]] obj, parent, emitMethods)
--[[
    if self.data then
        self.data = self:data()
    end
    if self.methods then
        self.methods = self:methods()
    end
    
    if self.observerUIData then
        self.observerUIData = self:observerUIData()
    end
    
    self.emitMethods = emitMethods
    self.data = self:mergeData(newData,self.data)
]]

    if self.methods then
        self.methods = self:methods()
    end
  
    self.emitMethods = emitMethods
    
	self.gameObject = obj
    self.parent = parent
--[[
    if self.onPreData then
        self:onPreData()
    end
]]

    moduleBase.super.created(self --[[,self.uiExpansion]])

    self:onCreated(--[[newData,emitMethods,]] parent)
end

---onCreated 一个实例只会触发一次，这时还未进行数据绑定
--[[
---@param parent UnityEngine.GameObject
---@param config UIConfigPage
---@param binderData table
]]
function moduleBase:onCreated(--[[newData,emitMethods,]] parent)
    
end


--[[function moduleBase:mergeData(newData,data)
    --if newData == nil or table.empty(newData) then
    --    return data
    --end
    --self:onMergeData(newData,data)
    return  moduleBase.super.mergeData(self, newData, data)
end

-----onMergeData 绑定，模块可以选择自己复写mergeData的实现，处理业务数据结构向ViewModel的转换
--function moduleBase:onMergeData(newData,data)
--    moduleBase.super.mergeData(self, newData, data)
--end
]]

function moduleBase:bind(uiExpansion)
    if self.isBinded then
        return
    end
    
    moduleBase.super.bind(self,uiExpansion)
    self.mappedDataStore = self:getDataSore()
    if self.mappedDataStore then
        if self.observerDataStore then
            self.observerDataStoreCache = self:observerDataStore()
            self.mappedDataStore:mapAllStates(self.observerDataStoreCache)
        end
        if self.listen then
            self.listenCache = self:listen()
            self.mappedDataStore:listenAllCall(self.listenCache)
        end
    end
end

function moduleBase:unBind()
    if self.mappedDataStore then
        if self.observerDataStoreCache then
            self.mappedDataStore:unmapAllStates(self.observerDataStoreCache)
        end
        if self.listenCache then
            self.mappedDataStore:unListenAllCall(self.listenCache)
        end
    end
    moduleBase.super.unBind(self)
end

function moduleBase:emit(eventName,...)
    if self.emitMethods~=nil and self.emitMethods[eventName] then
        self.emitMethods[eventName](self.parent,...)
    end
end

return moduleBase