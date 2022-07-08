---@class NFUResource : Object
---@field public Resource ResourceComponent
---@field public WebRequest WebRequestComponent
---@field public Initializated bool
---@field public isEditorMode bool
---@field public currentVariant string
local NFUResource={ }
---@public
---@return void
function NFUResource.Initialize() end
---@public
---@return void
function NFUResource.HotUpdateStart() end
---@public
---@param type number
---@return void
function NFUResource.Shutdown(type) end
---@public
---@param assetName string
---@return number
function NFUResource.HasAsset(assetName) end
---@public
---@param assetName string
---@return string
function NFUResource.GetResourceName(assetName) end
---@public
---@param assetName string
---@return string
function NFUResource.GetResourceFullName(assetName) end
---@public
---@param assetName string
---@return string
function NFUResource.GetResourceFileName(assetName) end
---@public
---@param assetName string
---@return string
function NFUResource.GetResourcePath(assetName) end
---@public
---@param assetName string
---@param loadAssetSuccessCallback LoadAssetSuccessCallback
---@param loadAssetFailureCallback LoadAssetFailureCallback
---@return ICancelable
function NFUResource.InstantiateAsync(assetName, loadAssetSuccessCallback, loadAssetFailureCallback) end
---@public
---@param assetName string
---@param assetType Type
---@param loadAssetSuccessCallback LoadAssetSuccessCallback
---@param loadAssetFailureCallback LoadAssetFailureCallback
---@param priority number
---@return ICancelable
function NFUResource.LoadAssetAsync(assetName, assetType, loadAssetSuccessCallback, loadAssetFailureCallback, priority) end
---@public
---@param assetName string
---@param assetType Type
---@return Object
function NFUResource.LoadAssetImmediate(assetName, assetType) end
---@public
---@param asset Object
---@return void
function NFUResource.Release(asset) end
---@public
---@param assetName string
---@param assetType Type
---@return void
function NFUResource.Release(assetName, assetType) end
---@public
---@param binaryAssetName string
---@return Byte[]
function NFUResource.LoadBinaryFromFileSystem(binaryAssetName) end
---@public
---@param sceneAssetName string
---@param loadSceneCallbacks LoadSceneCallbacks
---@param priority number
---@param userData Object
---@return void
function NFUResource.LoadSceneAsset(sceneAssetName, loadSceneCallbacks, priority, userData) end
---@public
---@param sceneAssetName string
---@param unloadSceneCallbacks UnloadSceneCallbacks
---@return void
function NFUResource.UnloadSceneAsset(sceneAssetName, unloadSceneCallbacks) end
---@public
---@param sceneAssetName string
---@param unloadSceneCallbacks UnloadSceneCallbacks
---@param userData Object
---@return void
function NFUResource.UnloadScene(sceneAssetName, unloadSceneCallbacks, userData) end
---@public
---@param binaryAssetName string
---@return string
function NFUResource.GetBinaryPath(binaryAssetName) end
---@public
---@param binaryAssetName string
---@param storageInReadOnly Boolean&
---@param storageInFileSystem Boolean&
---@param relativePath String&
---@param fileName String&
---@return bool
function NFUResource.GetBinaryPath(binaryAssetName, storageInReadOnly, storageInFileSystem, relativePath, fileName) end
---@public
---@param binaryAssetName string
---@return number
function NFUResource.GetBinaryLength(binaryAssetName) end
---@public
---@param binaryAssetName string
---@param loadBinaryCallbacks LoadBinaryCallbacks
---@return void
function NFUResource.LoadBinary(binaryAssetName, loadBinaryCallbacks) end
---@public
---@param binaryAssetName string
---@param loadBinaryCallbacks LoadBinaryCallbacks
---@param userData Object
---@return void
function NFUResource.LoadBinary(binaryAssetName, loadBinaryCallbacks, userData) end
---@public
---@param resourceName string
---@return String[]
function NFUResource.GetResourceAllAssets(resourceName) end
---@public
---@param rootAsset Object
---@return Object[]
function NFUResource.GetAllSubAssets(rootAsset) end
---@public
---@param rootAsset Object
---@param type Type
---@return List`1
function NFUResource.GetSubAssets(rootAsset, type) end
ND.Managers.ResourceMgr.Runtime.NFUResource = NFUResource