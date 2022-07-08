---@class NDList : UIBehaviour
---@field public bindList Object
---@field public onAddHandler Action`2
---@field public onDeleteHandler Action`2
---@field public prefab GameObject
---@field public totalCount number
---@field public direction number
local NDList={ }
---@public
---@param direction number
---@return void
function NDList:SetDirection(direction) end
---@public
---@param curTotal number
---@return void
function NDList:UpdateCurrent(curTotal) end
---@public
---@return void
function NDList:ReloadData() end
ND.UI.NDUI.NDList = NDList