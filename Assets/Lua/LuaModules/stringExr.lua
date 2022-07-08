---@param input string
function string.ltrim(input)
    return string.gsub(input, "^[ \t\n\r]+", "")
end

---@param input string
function string.rtrim(input)
    return string.gsub(input, "[ \t\n\r]+$", "")
end

---@param input string
function string.trim(input)
    input = string.gsub(input, "^[ \t\n\r]+", "")
    return string.gsub(input, "[ \t\n\r]+$", "")
end

---@param input string
---@param delimiter string
---@param cast function
function string.split(input, delimiter, cast)
    input = tostring(input)
    delimiter = tostring(delimiter)
    if type(cast) ~= "function" then
        cast = nil
    end
    if (delimiter == '') then
        return false
    end
    local pos, arr = 0, {}
    -- for each divider found
    for st, sp in function()
        return string.find(input, delimiter, pos, true)
    end do
        if cast ~= nil then
            table.insert(arr, cast(string.sub(input, pos, st - 1)))
        else
            table.insert(arr, string.sub(input, pos, st - 1))
        end
        pos = sp + 1
    end
    if cast ~= nil then
        table.insert(arr, cast(string.sub(input, pos)))
    else
        table.insert(arr, string.sub(input, pos))
    end

    return arr
end