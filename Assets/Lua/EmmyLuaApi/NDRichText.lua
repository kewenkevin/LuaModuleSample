---@class NDRichText : NDText
---@field public OnGetPrefab GetPrefab
---@field public OnReleasePrefab ReleasePrefab
---@field public onClick RichTextHrefEvent
---@field public lineAlignment number
---@field public richText string
local NDRichText={ }
---@public
---@return void
function NDRichText:DrawTag() end
---@public
---@param eventData PointerEventData
---@return void
function NDRichText:OnPointerClick(eventData) end
ND.UI.NDUI.NDRichText = NDRichText