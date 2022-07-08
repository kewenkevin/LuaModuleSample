---@class LinkerData : Object
---@field public Label string
---@field public ValueTypeId number
local LinkerData={ }
---@public
---@param owner UIExpansion
---@param config LinkerConfig
---@return bool
function LinkerData:Init(owner, config) end
ND.UI.LinkerData = LinkerData