---@class MaskableGraphic : Graphic
---@field public onCullStateChanged CullStateChangedEvent
---@field public maskable bool
---@field public isMaskingGraphic bool
local MaskableGraphic={ }
---@public
---@param baseMaterial Material
---@return Material
function MaskableGraphic:GetModifiedMaterial(baseMaterial) end
---@public
---@param clipRect Rect
---@param validRect bool
---@return void
function MaskableGraphic:Cull(clipRect, validRect) end
---@public
---@param clipRect Rect
---@param validRect bool
---@return void
function MaskableGraphic:SetClipRect(clipRect, validRect) end
---@public
---@param clipSoftness Vector2
---@return void
function MaskableGraphic:SetClipSoftness(clipSoftness) end
---@public
---@return void
function MaskableGraphic:ParentMaskStateChanged() end
---@public
---@return void
function MaskableGraphic:RecalculateClipping() end
---@public
---@return void
function MaskableGraphic:RecalculateMasking() end
UnityEngine.UI.MaskableGraphic = MaskableGraphic