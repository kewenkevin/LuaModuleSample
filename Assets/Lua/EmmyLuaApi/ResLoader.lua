---@class ResLoader : CustomYieldInstruction
---@field public Id number
---@field public loadingCount number
---@field public MainAsset Object
---@field public allRes Res[]
---@field public keepWaiting bool
local ResLoader={ }
---@public
---@return ResLoader
function ResLoader.Alloc() end
---@public
---@param assetNamePrefix string
---@return ResLoader
function ResLoader.Alloc(assetNamePrefix) end
---@public
---@param name string
---@param listener ResLoadCompleteCallBack
---@return ResLoader
function ResLoader:Add2Load(name, listener) end
---@public
---@param name string
---@param type Type
---@param listener ResLoadCompleteCallBack
---@return ResLoader
function ResLoader:Add2Load(name, type, listener) end
---@public
---@param listener Action
---@return void
function ResLoader:Load(listener) end
---@public
---@param assetName string
---@param assetType Type
---@return Object
function ResLoader:LoadAssetImmediate(assetName, assetType) end
---@public
---@param name string
---@param assetType Type
---@return void
function ResLoader:ReleaseRes(name, assetType) end
---@public
---@return void
function ResLoader:ReleaseAllRes() end
---@public
---@param assetName string
---@param assetType Type
---@return Object
function ResLoader:GetAsset(assetName, assetType) end
---@public
---@return void
function ResLoader:Recycle2Cache() end
---@public
---@return void
function ResLoader:OnCacheReset() end
---@public
---@return void
function ResLoader:Dispose() end
---@public
---@return void
function ResLoader:DisposeOnGC() end
ND.Managers.ResourceMgr.Runtime.ResLoader = ResLoader