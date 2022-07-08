
---@param obj table 对象
---@param method function 方法
---@return function
function handler(obj,method)
    return function(...)
        return method(obj,...)
    end
end