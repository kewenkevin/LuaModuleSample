---@class NDTableViewCell : MonoBehaviour
---@field public rectTransform RectTransform
---@field public id number
---@field public reuseIdentifier string
local NDTableViewCell={ }
---@public
---@return void
function NDTableViewCell:UpdateCellSize() end
---@public
---@param tableView NDScrollRect
---@return Vector2
function NDTableViewCell:GetSize(tableView) end
---@public
---@param tableView NDScrollRect
---@param id number
---@return void
function NDTableViewCell:SetData(tableView, id) end
---@public
---@param tableView NDScrollRect
---@return void
function NDTableViewCell:Unused(tableView) end
ND.UI.NDUI.NDTableViewCell = NDTableViewCell