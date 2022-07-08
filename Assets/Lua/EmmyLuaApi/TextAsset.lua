---@class TextAsset : Object
---@field public bytes Byte[]
---@field public text string
local TextAsset={ }
---@public
---@return string
function TextAsset:ToString() end
UnityEngine.TextAsset = TextAsset