---@class NDImage : Image
---@field public localizationProvider Action`2
---@field public isStaticFont bool
---@field public originalMaterial Material
---@field public flipVertical bool
---@field public flipHorizontal bool
---@field public cullNoneSprite bool
---@field public spriteName string
---@field public clonedMaterial Material
local NDImage={ }
---@public
---@return void
function NDImage:ResetMaterial() end
---@public
---@return void
function NDImage:UpdateLocalization() end
ND.UI.NDUI.NDImage = NDImage