---@class NDSlider : Selectable
---@field public tween bool
---@field public tweenDuration number
---@field public onTweenComplete TweenCompleteEvent
---@field public fillRect RectTransform
---@field public handleRect RectTransform
---@field public text Text
---@field public direction number
---@field public minValue number
---@field public maxValue number
---@field public wholeNumbers bool
---@field public value number
---@field public normalizedValue number
---@field public onValueChanged SliderEvent
local NDSlider={ }
---@public
---@param input number
---@return void
function NDSlider:SetValueWithoutNotify(input) end
---@public
---@param executing number
---@return void
function NDSlider:Rebuild(executing) end
---@public
---@return void
function NDSlider:LayoutComplete() end
---@public
---@return void
function NDSlider:GraphicUpdateComplete() end
---@public
---@param eventData PointerEventData
---@return void
function NDSlider:OnPointerDown(eventData) end
---@public
---@param eventData PointerEventData
---@return void
function NDSlider:OnDrag(eventData) end
---@public
---@param eventData AxisEventData
---@return void
function NDSlider:OnMove(eventData) end
---@public
---@return Selectable
function NDSlider:FindSelectableOnLeft() end
---@public
---@return Selectable
function NDSlider:FindSelectableOnRight() end
---@public
---@return Selectable
function NDSlider:FindSelectableOnUp() end
---@public
---@return Selectable
function NDSlider:FindSelectableOnDown() end
---@public
---@param eventData PointerEventData
---@return void
function NDSlider:OnInitializePotentialDrag(eventData) end
---@public
---@param direction number
---@param includeRectLayouts bool
---@return void
function NDSlider:SetDirection(direction, includeRectLayouts) end
ND.UI.NDUI.NDSlider = NDSlider