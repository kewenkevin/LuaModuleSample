---@class NDText : Text
---@field public localizationProvider Func`2
---@field public m_LetterSpacing number
---@field public LocalizationKey string
---@field public LocalizationGearType number
---@field public IsLocalized bool
---@field public text string
---@field public letterSpacing number
---@field public style TextStyleBase
---@field public colorStyle ColorStyleBase
local NDText={ }
---@public
---@return void
function NDText:UpdateLocalization() end
ND.UI.NDUI.NDText = NDText