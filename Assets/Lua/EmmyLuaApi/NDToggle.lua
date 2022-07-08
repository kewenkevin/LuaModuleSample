---@class NDToggle : Selectable
---@field public graphic GameObject
---@field public inversGraphic GameObject
---@field public onValueChanged ToggleEvent
---@field public on UnityEvent2
---@field public off UnityEvent2
---@field public group NDToggleGroup
---@field public isOn bool
local NDToggle={ }
---@public
---@param executing number
---@return void
function NDToggle:Rebuild(executing) end
---@public
---@return void
function NDToggle:LayoutComplete() end
---@public
---@return void
function NDToggle:GraphicUpdateComplete() end
---@public
---@param value bool
---@return void
function NDToggle:SetIsOnWithoutNotify(value) end
---@public
---@param value bool
---@param sendCallback bool
---@return void
function NDToggle:Set(value, sendCallback) end
---@public
---@return void
function NDToggle:UpdateStatus() end
---@public
---@param eventData PointerEventData
---@return void
function NDToggle:OnPointerClick(eventData) end
---@public
---@param eventData BaseEventData
---@return void
function NDToggle:OnSubmit(eventData) end
ND.UI.NDUI.NDToggle = NDToggle