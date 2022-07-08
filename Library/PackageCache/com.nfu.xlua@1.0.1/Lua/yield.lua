local util = require 'xlua.util'
local monoBehaviour = __UNITY_MONOBEHAVIOUR
local waitForFixedUpdate = __UNITY_WAITFORFIXEDUPDATE
local waitForEndOfFrame = CS.UnityEngine.WaitForEndOfFrame()

local YIELD = {
    StartCoroutine = function(...)
        return monoBehaviour:StartCoroutine(util.cs_generator(...))
    end,
    StopCoroutine = function(co)
        monoBehaviour:StopCoroutine(co)
    end,
    WaitForEndOfFrame = function(count)
        if count ~= nil then
            for i = 1, count, 1 do
                coroutine.yield(waitForEndOfFrame)
            end
        else
            coroutine.yield(waitForEndOfFrame)
        end
    end,
    WaitForSeconds = function(seconds)
        coroutine.yield(CS.UnityEngine.WaitForSeconds(seconds))
    end,
    WaitForSecondsRealtime = function(seconds)
        coroutine.yield(CS.UnityEngine.WaitForSecondsRealtime(seconds))
    end,
    WaitForFixedUpdate = function(count)
        if count ~= nil then
            for i = 1, count, 1 do
                coroutine.yield(waitForFixedUpdate)
            end
        else
            coroutine.yield(waitForFixedUpdate)
        end
    end,
    WaitWhile = function(predicateFunc)
        coroutine.yield(CS.UnityEngine.WaitWhile(predicateFunc))
    end,
    WaitUntil = function(predicateFunc)
        coroutine.yield(CS.UnityEngine.WaitUntil(predicateFunc))
    end,
    Wait = function(obj, isDoneField)
        local t = type(obj)
        if t == "function" then
            coroutine.yield(CS.UnityEngine.WaitUntil(obj))
        elseif t == 'userdata' then
            coroutine.yield(obj)
        elseif t == "table" then
            if not isDoneField then
                isDoneField = "isDone"
            end
            while not obj[isDoneField] do
                coroutine.yield(waitForEndOfFrame)
            end
        end
    end
}

return YIELD