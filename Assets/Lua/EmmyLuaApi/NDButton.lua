---@class NDButton : Selectable
---@field public onClick ButtonClickedEvent
---@field public onDoubleClick ButtonDoubleClickedEvent
---@field public onLongClick ButtonLongClickedEvent
---@field public onLongPress ButtonLongPressedEvent
---@field public onDown ButtonDownEvent
---@field public onUp ButtonUpEvent
---@field public onInteractiveChange ButtonInteractiveChangeEvent
---@field public onPointerExit ButtonPointerExitEvent
---@field public onPointerEnter ButtonPointerEnterEvent
local NDButton={ }
---@public
---@return bool
function NDButton:IsInteractable() end
---@public
---@param eventData PointerEventData
---@return void
function NDButton:OnPointerEnter(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDButton:OnPointerExit(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDButton:OnPointerDown(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDButton:OnPointerUp(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDButton:OnPointerClick(eventData) end
---@public
---@param eventData BaseEventData
---@return void
function NDButton:OnSubmit(eventData) end
ND.UI.NDUI.NDButton = NDButton