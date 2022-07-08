G_DataStoreModuleBase = require('LuaModules.DataStore.Recommended.DataStoreModuleBase')

-----简略模式
---@type DataStore
---@field player PlayerModule
local Store = require('LuaModules.DataStore.Recommended.DataStore').new()


---定义数据模块
------------------------------------------------------------------------
---@type PlayerModule
Store.player = require('Store.Modules.PlayerModule').new()
---@type AllianceModule
Store.alliance = require('Store.Modules.AllianceModule').new()
Store.checkerLevel = require("Store.Modules.CheckerLevelModule").new()

Store.cipher = require("Store.Modules.CipherModule").new()

------------------------------------------------------------------------

return Store:initialize()



