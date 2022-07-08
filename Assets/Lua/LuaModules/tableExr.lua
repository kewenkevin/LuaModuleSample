
---深度克隆一个对象
---@param object table
---@return table
function table.clone(object)
    local lookup_table = {}
    local function _copy(object)
        if type(object) ~= "table" then
            return object
        elseif lookup_table[object] then
            return lookup_table[object]
        end
        local newObject = {}
        lookup_table[object] = newObject
        for key, value in pairs(object) do
            newObject[_copy(key)] = _copy(value)
        end
        return setmetatable(newObject, getmetatable(object))
    end
    return _copy(object)
end

function table.empty(tb)
    return next(tb) == nil
end

---@param str string
function table.totable(str)
    if str == nil or type(str) ~= "string" then
        return
    end
    return loadstring("return " .. str)()
end

local function dumpToList(value, nesting)
    if type(nesting) ~= "number" then nesting = 99999 end
    local lookupTable = {}
    local results = {}
    local levels = {}

    local function _v(v)
        if type(v) == "string" then
            v = "\"" .. v .. "\""
        end
        return tostring(v)
    end

    local function _dump(value, desciption, nest)
        desciption = desciption or "var"
        if type(value) ~= "table" then
            results[#results +1 ] = string.format("[%s] = %s, ", _v(desciption), _v(value))
            levels[#levels + 1] = nest
        elseif lookupTable[value] then
            results[#results +1 ] = string.format("[%s] = *REF*, ", _v(desciption))
            levels[#levels + 1] = nest
        else
            lookupTable[value] = true
            if nest > nesting then
                results[#results + 1] = string.format("[%s] = *MAX NESTING* ,", _v(desciption))
                levels[#levels + 1] = nest
            else
                results[#results +1 ] = string.format("[%s] =", _v(desciption))
                -- result[#result +1 ] = "{"
                levels[#levels + 1] = nest
                local keys = {}
                local values = {}
                for k, v in pairs(value) do
                    keys[#keys + 1] = k
                    local vk = _v(k)
                    values[k] = v
                end
                table.sort(keys, function(a, b)
                    if type(a) == "number" and type(b) == "number" then
                        return a < b
                    else
                        return tostring(a) < tostring(b)
                    end
                end)
                for i, k in ipairs(keys) do
                    _dump(values[k], k, nest + 1)
                end
                -- result[#result +1] = "},"
                -- level[#level + 1] = nest
            end
        end
    end
    _dump(value, nil , 0)
    -- for i, line in ipairs(result) do
    --     print(line[1],line[2])
    -- end
    return levels, results
end

---@param tb table
---@param nesting number
---@param format boolean
function table.tostring(tb, nesting, format)
    local sep = ""
    local indent = ""
    if format == true then
        sep = "\n"
        indent = "    "
    end
    local levels , results = dumpToList(tb,nesting)
    local rt = ""
    local t = #results
    local level = 0
    for i = 1, t do
        if levels[i] > level then
            level = levels[i]
            rt = rt .. string.rep(indent, levels[i-1]) .. "{"
            rt = rt .. sep
        elseif levels[i] < level then
            level = levels[i]
            rt = rt .. string.rep(indent, levels[i-1]-1) .. "},"
            rt = rt .. sep

        end
        rt = rt .. string.rep(indent, level)
        rt = rt .. results[i]
        rt = rt .. sep
    end
    if level > 0 then
        rt = rt .. string.rep(indent, level-1) .. "}"
    end
    return rt
end

---@param input table
---@param nesting number
function table.dump(input, nesting)
    if type(nesting) ~= "number" then nesting = 3 end
    local rt = table.tostring(input, nesting, true)
    local traceback = string.split(debug.traceback("", 2), "\n")
    print("dump from: " .. string.trim(traceback[3]) .. rt)
end

---@param input table
---@param width number
function table.dumptree(input, width)
    if type(width) ~= "number" then width = 2 end
    local end_flag = {}
    local function make_indent(layer, is_end)
        local subIndent = string.rep("  ", width)
        local indent = "";
        end_flag[layer] = is_end;
        local subIndent = string.rep("  ", width)
        for index = 1, layer - 1 do
            if end_flag[index] then
                indent = indent.." "..subIndent
            else
                indent = indent.."|"..subIndent
            end
        end

        if is_end then
            return indent.."└"..string.rep("─", width).." "
        else
            return indent.."├"..string.rep("─", width).." "
        end
        return indent
    end
    local levels , results = dumpToList(input)
    local rt = ""
    local sep = "\n"
    local t = #results
    local level = 0
    for i = 1, t do
        rt = rt .. make_indent(levels[i],levels[i] < level)
        level = levels[i]
        rt = rt .. results[i]
        rt = rt .. sep
    end

    local traceback = string.split(debug.traceback("", 2), "\n")
    print("dump from: " .. string.trim(traceback[3]) .. rt)
end

dump = table.dump
dumptree = table.dumptree