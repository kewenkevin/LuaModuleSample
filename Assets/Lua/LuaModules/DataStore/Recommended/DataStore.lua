--- 面向对象数据仓库类
--- Generated by EmmyLua(https://github.com/EmmyLua)

--- Example ： 
---------------------------------------------------------------------------------------------
--- ---初始化后直接定义模块
--- local DataStore = require('LuaModules.DataStore.Recommended.DataStore').new()
---
--- DataStore.player = require('Store.PlayerModule').new()
--- DataStore.player2 = require('Store.PlayerModule2').new()
---
--- ---注意模块加入后一定要进行初始化
--- return DataStore:initialize()  
----------------------------------------------------------------------------------------------
--- ---初始化后增加一个模块
--- DataStore.tests = require('Store.TestModule').new()
--- --- 需要手动调用WrapData接口，创建内部结构，保证数据绑定正常工作
--- DataStore.tests:wrapData()
----------------------------------------------------------------------------------------------

---@class DataStore
local DataStore = class("DataStore")

function DataStore:ctor(...)

end

---@return DataStore
function DataStore:initialize()
    for moduleName, module in pairs(self) do
        if type(module) == "table" and type(module.wrapData) == "function" then
            module:wrapData()
        end
    end
    return self
end

---_toMapState
---@private
---@param mapState table
---@param moduleName string 模块名
---@param maps table|function
function DataStore:_toMapState(mapState, moduleName, maps)
    if not mapState then
        return
    end
    if type(maps) == "table" then
        for i, v in pairs(maps) do
            mapState[i] = mapState[i] or {}
            self:_toMapState(mapState[i], i, v)
        end
    elseif type(maps) == "function" then
        table.insert(mapState, 1, maps)
    else
        error("mapStates error map target must table or function: " .. moduleName)
    end
end

---_toUnmapState
---@private
---@param mapState table
---@param moduleName string
---@param maps table | function
function DataStore:_toUnmapState(mapState, moduleName, maps)
    if not mapState then
        return
    end
    if type(maps) == "table" then
        for i, v in pairs(maps) do
            mapState[i] = mapState[i] or {}
            self:_toUnmapState(mapState[i], i, v)
        end
    elseif type(maps) == "function" then
        for i = 1, #mapState do
            if mapState[i] == maps then
                table.remove(mapState, i)
                break
            end
        end
    else
        error("mapStates error map target must table or function: " .. moduleName)
    end
end

---mapStates 按table结构映射一个方法图和数据
---@param moduleName table
---@param maps table<string,function>
function DataStore:mapStates(moduleName, maps)
    if not self[moduleName] then
        return
    end
    self:_toMapState(self[moduleName]._statesObservers, moduleName, maps)
end

---unmapStates 按table结构取消映射一个方法图和数据
---@param moduleName string
---@param maps table<string,function>
function DataStore:unmapStates(moduleName, maps)
    if self[moduleName] then
        self:_toUnmapState(self[moduleName]._statesObservers, moduleName, maps)
    end
end

---mapAllStates 按table结构映射方法图和数据
---@param maps table<string,table>
function DataStore:mapAllStates(maps)
    if maps then
        for key, value in pairs(maps) do
            self:mapStates(key, value)
        end
    end
end

---unmapAllStates 按table结构取消映射方法图和数据
---@param maps table<string,table>
function DataStore:unmapAllStates(maps)
    if maps then
        for key, value in pairs(maps) do
            self:unmapStates(key, value)
        end
    end
end

---listenCall
---@param moduleName table
---@param maps table
---@param mapsFunc table
function DataStore:listenCall(moduleName, maps, mapsFunc)
    if maps == nil then
        error("error:dont listenCall this maps params is nil: " .. moduleName)
        return ;
    end
    local v = self[moduleName]
    if not v then
        error("error:dont hash this moduleName: " .. moduleName)
        return ;
    end
    if not v._eventListener then
        v._eventListener = {}
    end
    if type(maps) == "table" then
        for k, f in pairs(maps) do
            v._eventListener[k] = v._eventListener[k] or {}
            table.insert(v._eventListener[k], f)
        end
    elseif type(maps) == "string" and mapsFunc ~= nil then
        v._eventListener[maps] = v._eventListener[maps] or {}
        table.insert(v._eventListener[maps], mapsFunc)
    else
        error("error:dont support this type: " .. type(maps))
    end
end

---unListenCall
---@param moduleName table
---@param maps table
---@param mapFunc table
function DataStore:unListenCall(moduleName, maps, mapFunc)
    local v = self[moduleName]
    if not v then
        error("error:dont hash this moduleName: " .. moduleName)
        return ;
    end
    if not v._eventListener then
        return ;
    end
    if type(maps) == "table" then
        for k, f in pairs(maps) do
            if v._eventListener[k] then
                for i, x in ipairs(v._eventListener[k]) do
                    if x == f then
                        table.remove(v._eventListener[k], i)
                    end
                end
            end
        end
    elseif type(maps) == "string" and mapFunc ~= nil then
        if v._eventListener[maps] then
            for i, x in ipairs(v._eventListener[maps]) do
                if x == mapFunc then
                    table.remove(v._eventListener[maps], i)
                end
            end
        end
    else
        error("error:dont support this type: " .. type(maps))
    end
end

---listenAllCall 增加一组监听
---@param maps table<string,<string,function>> 第一个string是模块名，第二个string是监听事件名
function DataStore:listenAllCall(maps)
    if type(maps) == "table" then
        for k, v in pairs(maps) do
            self:listenCall(k, v)
        end
    else
        error("error:listenAllCall must a table in  this moduleName")
    end
end

---unListenAllCall 取消一组监听
---@param maps table<string,<string,function>> 第一个string是模块名，第二个string是监听事件名
function DataStore:unListenAllCall(maps)
    if type(maps) == "table" then
        for k, v in pairs(maps) do
            self:unListenCall(k, v)
        end
    else
        error("error:listenAllCall must a table in  this moduleName")
    end
end

-------------------------------------------------------------------------------------------
--- 过期方法，之后移除
-------------------------------------------------------------------------------------------

---dispatch 触发某模块的manager中的某个方法
---@param moduleName string 模块名
---@param managerFuncName string 要出发的manger的方法名称
---@param params table 触发的manager方法的参数列表
function DataStore:dispatch(moduleName, managerFuncName, params)

    local v = self[moduleName]
    if not v then
        error("dont has this moduleName dispatch  " .. moduleName)
        return nil;
    end
    return v:dispatch(managerFuncName, params)
end

---commit 提交数据到某个模块的某个字段
---@param moduleName string
---@param propName string 参数名
---@param params table setter参数列表
function DataStore:commit(moduleName, propName, params)
    local v = self[moduleName]
    if not v then
        error("dont has this moduleName commit" .. moduleName)
        return nil;
    end
    return v:commit(propName, params)
end

---getter 调用某个模块的getter中propName的方法，返回数据
---@param moduleName string
---@param propName string
---@return any
function DataStore:getter(moduleName, propName)
    local v = self[moduleName]
    if not v then
        error("error:dont hash this moduleName" .. moduleName)
        return nil;
    end
    return v:getter(propName)
end
-------------------------------------------------------------------------------------------


return DataStore;