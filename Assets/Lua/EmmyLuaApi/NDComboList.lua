---@class NDComboList : NDScrollRect
---@field public getCellSizeDelegate GetSizeForCellDelegate
---@field public getCellReuseIdentifierDelegate GetCellReuseIdentifierDelegate
local NDComboList={ }
---@public
---@return void
function NDComboList:CheckLayout() end
ND.UI.NDUI.NDComboList = NDComboList