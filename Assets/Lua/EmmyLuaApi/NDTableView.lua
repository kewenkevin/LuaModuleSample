---@class NDTableView : NDScrollRect
---@field public getCellSizeDelegate GetSizeForCellDelegate
---@field public getCellReuseIdentifierDelegate GetCellReuseIdentifierDelegate
local NDTableView={ }
---@public
---@return void
function NDTableView:CheckLayout() end
ND.UI.NDUI.NDTableView = NDTableView