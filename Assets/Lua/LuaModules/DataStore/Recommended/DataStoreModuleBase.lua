--- 面向对象数据对象基类
--- Generated by EmmyLua(https://github.com/EmmyLua)

--- Example ： 
---------------------------------------------------------------------------------------------
--- local PlayerModule = class("Store.PlayerModule", require('LuaModules.DataStore.Recommended.DataStoreModuleBase'))
---
--- ---在这里定义数据初始值，注意此时数据data还未包裹warp，这是在构造时，GenerateData之前调用
--- function PlayerModule:initialize()
---     self._data.tag = "player"
---     self._data.playerId = 0
---     self._data.rank = 100
---     self._data.playerName = ""
---     self._data.cardList = {
---         type = "array"
---     }
---     self._data.race = ""
---     self._data.level = 0
---     self._data.exp = 0
---     self._data.courage_trial = 0
---     self._data.fight_value = 0
---     self._data.actor_limit = 0
---     self._data.vipLevel = 0
---     self._data.vipPoints = 0
---     self._data.mapId = 0
---     self._data.birthday = ""
---     self._data.account = ""
---     self._data.charge_sn = 0
---     self._data.staking_sn = 0
---     self._data.id_account = ""
--- end
----------------------------------------------------------------------------------------------

local DataStoreModuleStorageFactory = require("LuaModules.DataStore.Core.DataStoreModuleStorageFactory")

---@class DataStoreModuleBase
---@field protected data table 原始状态（实际存储）
---@field protected _statesObservers table<string,function[]> 原始状态（实际存储）
---@field private _eventListener table<string,function[]> 原始状态（实际存储）
local DataStoreModuleBase = class("DataStoreModuleBase")

---ctor
function DataStoreModuleBase:ctor(...)
    self.moduleName = self.class.__cname
    self._statesObservers = {}
    self._eventListener = {}
    self._data = {}
    self:initialize(...)
end

---initialize 初始化方法，用于给data赋值等初始化
function DataStoreModuleBase:initialize(...)

end

---BindData 进行数据绑定
function DataStoreModuleBase:wrapData()
    local initData = self._data
    self._data = {}
    self._data = self:_generateInternalStorage(self._statesObservers, initData)
    self:afterWrap()
end


---afterWrap
function DataStoreModuleBase:afterWrap()
    
end

---GenerateInternalStorage 生成一个内部存储区
---@private
---@param moduleName string
---@param mappedStates table
---@param data table
function DataStoreModuleBase:_generateInternalStorage(mappedStates, data, keyName)
    if data == nil then
        return nil
    end
    -- 建立模块内部存储区域 并 写入内部存储区
    return DataStoreModuleStorageFactory.generate(self, mappedStates, data, keyName)
end


---dispatchEvent 发出事件 
---protected
---@param eventName string
---@param ... any[]
function DataStoreModuleBase:dispatchEvent(eventName, ...)
    self:fireEvent(eventName, ...)
end

---fireEvent 发出事件 
---protected
---@param eventName string
---@param ... any[]
function DataStoreModuleBase:fireEvent(eventName, ...)
    if not self._eventListener then
        self._eventListener = {}
    end
    local listeners = self._eventListener[eventName]
    if not listeners then
        listeners = {}
        self._eventListener[eventName] = listeners
    end
    if type(listeners) ~= "table" then
        error("error:call this callback must be an array table> " .. self.moduleName .. ":" .. eventName)
        return
    end
    for i, v in ipairs(listeners) do
        listeners[i](...)
    end
end


-------------------------------------------------------------------------------------------------------
--- 过期方法，之后移除
-------------------------------------------------------------------------------------------------------


---dispatch
---@param functionName string
---@param params table
function DataStoreModuleBase:dispatch(functionName, params)
    warn("warning! this is an expired function! please use recommended function like 'Store.[moduleName].[functionName]'")

    local act = self[functionName]
    if not act then
        error("error:DataStoreModule:dispatch dont has managers > " .. self.moduleName .. ":" .. functionName)
        return nil
    end
    local result = act(self, params);

    return result
end

---commit
---@param propName string
---@param params table
---@return any
function DataStoreModuleBase:commit(propName, params)
    warn("warning! this is an expired function! please use recommended function like 'Store.[moduleName].data.[propertyName] = [newvalue]'")
    local setter = self[propName];
    if not setter then
        error("error: DataStoreModule:commit dont has setter > " .. self.moduleName .. ":" .. propName)
        return nil
    end
    local result = setter(self, params)
    return result
end

---getter
---@param propName string
---@return any
function DataStoreModuleBase:getter(propName)
    warn("warning! this is an expired function! please use recommended function like 'local value = Store.[moduleName].data.[propertyName]'")

    local getter = self[propName];
    if not getter then
        error("error:DataStoreModule:getter dont has getter > " .. self.moduleName .. propName)
        return nil
    end
    return getter(self)
end
-------------------------------------------------------------------------------------------



---SnapShot 快照
function DataStoreModuleBase:SnapShotData()
    local snapShot = self:_snapShot(self._data)
    return snapShot;
end


function DataStoreModuleBase:_snapShot(data)
    if type(data) == "table" then
        local targetTable = {}
        for key, value in pairsUMT(data) do
            local snap = self:_snapShot(value)
            if snap~=nil then
                targetTable[key] = snap
            end
        end
        return targetTable
    elseif type(data) == "function" then
        return nil
    else 
        return data
    end
end


return DataStoreModuleBase