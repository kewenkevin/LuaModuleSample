---@class Ray : ValueType
---@field public origin Vector3
---@field public direction Vector3
local Ray={ }
---@public
---@param distance number
---@return Vector3
function Ray:GetPoint(distance) end
---@public
---@return string
function Ray:ToString() end
---@public
---@param format string
---@return string
function Ray:ToString(format) end
---@public
---@param format string
---@param formatProvider IFormatProvider
---@return string
function Ray:ToString(format, formatProvider) end
UnityEngine.Ray = Ray