---@class NDScrollRect : UIBehaviour
---@field public cellOnUseDelegate CellOnUseDelegate
---@field public cellOnUnuseDelegate CellOnUnUseDelegate
---@field public direction number
---@field public totalCount number
---@field public reverseDirection bool
---@field public rubberScale number
---@field public content RectTransform
---@field public movementType number
---@field public elasticity number
---@field public canScroll bool
---@field public inertia bool
---@field public snap bool
---@field public decelerationRate number
---@field public scrollSensitivity number
---@field public viewport RectTransform
---@field public scrollbar NDScrollbar
---@field public scrollbarVisibility number
---@field public onValueChanged ScrollRectEvent
---@field public velocity Vector2
---@field public normalizedPosition Vector2
---@field public horizontalNormalizedPosition number
---@field public verticalNormalizedPosition number
---@field public scrollingNeeded bool
local NDScrollRect={ }
---@public
---@return void
function NDScrollRect:CheckLayout() end
---@public
---@return void
function NDScrollRect:ClearCells() end
---@public
---@param index number
---@param speed number
---@return void
function NDScrollRect:ScrollToCell(index, speed) end
---@public
---@return void
function NDScrollRect:RefreshCells() end
---@public
---@param offset number
---@param fillViewRect bool
---@return void
function NDScrollRect:ReloadData(offset, fillViewRect) end
---@public
---@param reuseIdentifier string
---@return NDTableViewCell
function NDScrollRect:GetReusableCell(reuseIdentifier) end
---@public
---@param executing number
---@return void
function NDScrollRect:Rebuild(executing) end
---@public
---@return void
function NDScrollRect:LayoutComplete() end
---@public
---@return void
function NDScrollRect:GraphicUpdateComplete() end
---@public
---@return bool
function NDScrollRect:IsActive() end
---@public
---@return void
function NDScrollRect:StopMovement() end
---@public
---@param data PointerEventData
---@return void
function NDScrollRect:OnScroll(data) end
---@public
---@param eventData PointerEventData
---@return void
function NDScrollRect:OnInitializePotentialDrag(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDScrollRect:OnBeginDrag(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDScrollRect:OnEndDrag(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDScrollRect:OnDrag(eventData) end
---@public
---@return void
function NDScrollRect:EditorUpdate() end
---@public
---@return void
function NDScrollRect:SetLayoutHorizontal() end
---@public
---@return void
function NDScrollRect:SetLayoutVertical() end
ND.UI.NDUI.NDScrollRect = NDScrollRect