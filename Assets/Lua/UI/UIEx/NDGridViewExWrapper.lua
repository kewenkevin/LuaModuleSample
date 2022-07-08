
-- 自定义列表Lua层封装
---@param elementInViewEx UIBase
---@param csNDTableViewEx NDGridView
local NDGridViewEx = function(elementInViewEx, csNDGridViewEx)
    --print("ScrollListEx")
    local csNDGridView = csNDGridViewEx
    local elementInView = elementInViewEx

    local tLuaListEx = {}
    -- 存储列表数据
    tLuaListEx._data = {}
    
    tLuaListEx.RefreshCells = function(listSelf)
        --print("RefreshCells")
        csNDGridView:RefreshCells()
    end
    
    setmetatable(tLuaListEx, {
        __index = function(tb, key)
            if key == "data" then
                --print("ScrollListEx")
                -- 获取列表数据
                return tb._data
            end
        end,
        __newindex = function(tb, key, value)
            --print("ScrollListEx __newindex " .. type(value) .. #value)
            if key == "data" and type(value) == "table" then
                tb._data = value

                print("__newindex _data " .. #value)
                -- 通过设置数据直接触发列表刷新
                csNDGridView.totalCount = #value
            end
        end
    })


    ---@type table<GameObject, UIModuleBase>
    local scriptUse = {}
    ---@type table<GameObject, UIModuleBase>
    local scriptUnUse = {}

    ---@param cell NDGridView
    csNDGridView.cellOnUseDelegate = function(cell)
        --print("cellOnUseDelegate " .. #tLuaListEx._data)
        local index = cell.id + 1
        if tLuaListEx._data ~= nil and type(tLuaListEx._data) == "table" and #tLuaListEx._data >= index then

            --print("cellOnUseDelegate1")
            local itemData = tLuaListEx._data[index]
            if itemData.script ~= nil then
                
                --print("cellOnUseDelegate2")
                local scriptModule = scriptUnUse[cell.gameObject]
                if scriptModule == nil then
                    --print("cellOnUseDelegate3")
                    scriptModule = elementInView:registerModule(itemData.script .. index, itemData.script, cell.gameObject)
                else
                    scriptUnUse[cell.gameObject] = nil
                    scriptModule:onBinding()
                end
                
                if scriptModule.Refresh then
                    --print("cellOnUseDelegate4")
                    scriptModule:Refresh(itemData)
                end

                scriptUse[cell.gameObject] = scriptModule
            else 
                error("index " .. index .. " of cell data can not find script info. ")
            end
        end
    end

    ---@param cell NDGridView
    csNDGridView.cellOnUnuseDelegate = function(cell)
        --print("cellOnUnuseDelegate")
        local scriptModule = scriptUse[cell.gameObject]
        if scriptModule == nil then
            error(cell.gameObject.name .. "can not find script")
        else
            scriptModule:onUnBinding()
            scriptUnUse[cell.gameObject] = scriptModule
        end

        scriptUse[cell.gameObject] = nil
    end
    
    return tLuaListEx
end

return NDGridViewEx

