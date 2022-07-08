---@class NDDragButton : NDButton
---@field public onDrag ButtonDragEvent
---@field public onDragOut ButtonDragEvent
---@field public onDragBegin ButtonDragEvent
---@field public onDragEnd ButtonDragEvent
---@field public onDragOutEnd ButtonDragEvent
local NDDragButton={ }
---@public
---@param eventData PointerEventData
---@return void
function NDDragButton:OnDrag(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDDragButton:OnBeginDrag(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDDragButton:OnEndDrag(eventData) end
ND.UI.NDUI.NDDragButton = NDDragButton