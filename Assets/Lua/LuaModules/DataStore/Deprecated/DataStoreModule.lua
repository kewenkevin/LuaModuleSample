--- 数据仓库的一个数据模块，每个数据模块会创建一个新的类，这个其实是类的模板。每个类的元表中存储各种数据段
--- Generated by EmmyLua(https://github.com/EmmyLua)


local DataStoreModuleStorageFactory = require("LuaModules.DataStore.Core.DataStoreModuleStorageFactory")

---@class DataStoreModule
---@field private moduleName string 模块名称
---@field private parent DataStoreDeprecated|DataStoreModule 父模块
---@field private mappedStates table
local DataStoreModule = class("DataStoreModule")

---ctor 构造函数
---@param moduleName string 当前模块名称
---@param req DataStoreModuleOption 定义的请求表
---@param parent table
---@param mappedStates table 注释
function DataStoreModule:ctor(moduleName, request, parent, mappedStates)
    self.moduleName = moduleName
    self.parent = parent
    self.mappedStates = mappedStates

    if type(request.beforeWarp) == "function" then
        request.beforeWarp(request)
    end
    
    self.class.data = self:_generateInternalStorage(self.mappedStates, request.data)

    self.class.managers = request.managers or {}

    self.class.getters = request.getters or {}
    self.class.setters = request.setters or {}

    self.class.callback = request.callback or {}

    if type(request.afterWarp) == "function" then
        request.afterWarp(self.class)
    end
    
    self.class.managers.context = self
end

---GenerateInternalStorage 生成一个内部存储区
---@param moduleName string
---@param mappedStates table
---@param data table
function DataStoreModule:_generateInternalStorage(mappedStates, data , keyName)
    if data == nil then
        return nil
    end
    -- 建立模块内部存储区域 并 写入内部存储区
    return DataStoreModuleStorageFactory.generate(self, mappedStates, data , keyName)
end

-----manager 获取方法管理器
function DataStoreModule:manager()
    local manager = self.class.managers;
    if not manager then
        error("error:DataStoreModule dont has manager > " .. self.moduleName)
        return nil
    end
    return manager
end

---dispatch
---@param functionName string
---@param params table
function DataStoreModule:dispatch(functionName, params)
    local mgr = self.class.managers
    if not mgr then
        error("error:DataStoreModule dont has manager > " .. self.moduleName)
        return nil
    end

    local act = mgr[functionName]
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
function DataStoreModule:commit(propName, params)
    local setters = self.class.setters;
    if not setters then
        error("error: DataStoreModule:commit dont has setters > " .. self.moduleName)
        return nil
    end
    local setter = setters[propName];
    if not setter then
        error("error: DataStoreModule:commit dont has setter > " .. self.moduleName .. ":" .. propName)
        return nil
    end
    local result = setter(self.class.data, params)
    return result
end

---getter
---@param propName string
---@return any
function DataStoreModule:getter(propName)
    local getters = self.class.getters;
    if not getters then
        error("error:DataStoreModule:getter dont has getters > " .. self.moduleName)
        return nil
    end
    local getter = getters[propName];
    if not getter then
        error("error:DataStoreModule:getter dont has getter > " .. self.moduleName .. propName)
        return nil
    end
    return getter(self.class.data)
end

---rootCommit
---@param module string
---@param funcName string
---@param params table
function DataStoreModule:rootCommit(module, funcName, params)
    self.parent:commit(module, funcName, params)
end

---call
---@param funcName string
---@param params table
function DataStoreModule:call(funcName, params)
    if not self.class.callback then
        self.class.callback = {}
    end
    local call = self.class.callback[funcName]
    if not call then
        call = {}
        callback[funcName] = call
    end

    if type(call) ~= "table" then
        error("error:call this callback must be an array table> " .. self.moduleName .. ":" .. funcName)
        return
    end

    for i, v in ipairs(call) do
        call[i](params)
    end
end

return DataStoreModule



