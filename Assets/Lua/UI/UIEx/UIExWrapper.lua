-- UGUI控件扩展Lua层封装
local UIExTransFunc = {
    -- 自定义列表
    ["NDTableView"] = require "UI.UIEx.NDTableViewExWrapper",
    ["NDGridView"] = require "UI.UIEx.NDGridViewExWrapper",
}

---@class UIExElement
UIExElement =
{
    NDTableView = "NDTableView",
    NDGridView  = "NDGridView",
}

---@param elementType UIExElement
function ElementEx(elementInView, element, elementType)
    print("ElementEx1") 
    local luaUiExFunc = UIExTransFunc[elementType]
    if luaUiExFunc ~= nil then
        print("ElementEx2")
        return luaUiExFunc(elementInView, element)
    end
end