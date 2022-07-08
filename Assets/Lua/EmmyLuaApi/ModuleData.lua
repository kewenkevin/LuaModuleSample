---@class ModuleData : Object
---@field public Label string
---@field public PrefabPath string
---@field public UI UIExpansion
local ModuleData={ }
---@public
---@param owner UIExpansion
---@param config LinkerConfig
---@return bool
function ModuleData:Init(owner, config) end
ND.UI.ModuleData = ModuleData