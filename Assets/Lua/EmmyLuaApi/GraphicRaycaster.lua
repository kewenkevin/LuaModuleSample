---@class GraphicRaycaster : BaseRaycaster
---@field public sortOrderPriority number
---@field public renderOrderPriority number
---@field public ignoreReversedGraphics bool
---@field public blockingObjects number
---@field public blockingMask LayerMask
---@field public eventCamera Camera
local GraphicRaycaster={ }
---@public
---@param eventData PointerEventData
---@param resultAppendList List`1
---@return void
function GraphicRaycaster:Raycast(eventData, resultAppendList) end
UnityEngine.UI.GraphicRaycaster = GraphicRaycaster