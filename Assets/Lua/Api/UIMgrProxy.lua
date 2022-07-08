local UIMgrProxy = {}

function UIMgrProxy:back(options)
    local result = UIMgr:canBack()
    if result == nil then
        UIMgr:back(options)
--[[    else
        result.confirm = function()
            UIMgr:back(options)
        end
        UIMgr:open("Confirm", result)]]
    end
end

function UIMgrProxy:globalBack(options)
    local result = UIMgr:canGlobalBack()
    if result == nil then
        UIMgr:globalBack(options)
--[[    else
        result.confirm = function()
            UIMgr:globalBack(options)
        end
        UIMgr:open("Confirm", result)]]
    end
end

return UIMgrProxy