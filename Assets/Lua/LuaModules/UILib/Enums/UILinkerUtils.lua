--- todo 最好是生成
local UILinkerUtils = class("UILinkerUtils")

---@class EnumLinkerTypeId 修改请见UIExpansionUtility.cs中枚举LinkerValueState
local EnumLinkerTypeId = {
    Vector2 = 0,
    Vector3 = 1,
    Quaternion = 2,
    Boolean = 3,
    Int32 = 4,
    String = 5,
    Single = 6,
    Color = 7,
    Sprite = 8,
    Char = 9,
    Rect = 10,
    SystemObject = 11,

    UnityEvent = 100,
    UnityEventBoolean = 101,
    UnityEventSingle = 102,
    UnityEventInt32 = 103,
    UnityEventString = 104,
    UnityEventVector2 = 105,

    UnityEvent2 = 130,
    UnityEvent2Boolean = 131,
    UnityEvent2Single = 132,
    UnityEvent2Int32 = 133,
    UnityEvent2String = 134,
    UnityEvent2Vector2 = 135,

    SystemEventActionIntAndObject = 201,
    SystemEventActionIntAndBool = 202,
    SystemEventActionObject = 203,
    DelegateInt = 301,
}

UILinkerUtils.EnumLinkerTypeId = EnumLinkerTypeId
---BindValue
---@param target UIBase
---@param metaTable table
---@param key string
---@param typeId number
---@param val any
function UILinkerUtils.setLinkerValue(target, metaTable, key, typeId, val)
    if typeId == EnumLinkerTypeId.Vector2 then
        metaTable.uiExpansion:LinkerSetVector2(key, val);
    elseif typeId == EnumLinkerTypeId.Vector3 then
        metaTable.uiExpansion:LinkerSetVector3(key, val);
    elseif typeId == EnumLinkerTypeId.Quaternion then
        metaTable.uiExpansion:LinkerSetQuaternion(key, val);
    elseif typeId == EnumLinkerTypeId.Boolean then
        local typeVal = type(val)
        if typeVal == "number" then
            val = (val ~= 0)
        elseif typeVal == "string" then
            val = (val == "true" or val == "True")
        elseif typeVal == "boolean" then
            val = val
        else
            val = (val~=nil)
        end
        metaTable.uiExpansion:LinkerSetBoolean(key, val);
    elseif typeId == EnumLinkerTypeId.Int32 then
        local typeVal = type(val)
        if typeVal == "string" then
            --判断是string类型的话，转为number
            val= tonumber(val)
        elseif typeVal == "boolean" then
            val = (val and 1 or 0)
        elseif typeVal ~= "number" then
            error("data value must number:" .. key .. " val: " .. val)
        end
        metaTable.uiExpansion:LinkerSetInt32(key, val);
    elseif typeId == EnumLinkerTypeId.String then
        if type(val) == "string" then
            val = val
        else
            val = tostring(val)
        end
        metaTable.uiExpansion:LinkerSetString(key, val);
    elseif typeId == EnumLinkerTypeId.Single then
        local typeVal = type(val)
        if typeVal == "string" then
            --判断是string类型的话，转为number
            val= tonumber(val)
        elseif typeVal == "boolean" then
            val = (val and 1 or 0)
        elseif typeVal ~= "number" then
            error("data value must number:" .. key .. " val: " .. val)
        end
        metaTable.uiExpansion:LinkerSetSingle(key, val);
    elseif typeId == EnumLinkerTypeId.Color then
        metaTable.uiExpansion:LinkerSetColor(key, val);
    elseif typeId == EnumLinkerTypeId.Sprite then
        metaTable.uiExpansion:LinkerSetSprite(key, val);
    elseif typeId == EnumLinkerTypeId.Char then
        metaTable.uiExpansion:LinkerSetChar(key, val);
    elseif typeId == EnumLinkerTypeId.Rect then
        metaTable.uiExpansion:LinkerSetRect(key, val);
    else
        local tag = target.tag
        if tag then
            error(target.tag, "_setValue dont has this type >" .. typeId)
        else
            error("UIStorageFactory _setValue dont has this type >" .. typeId)
        end
        return
    end
end

---BindFunction
---@param target UIBase
---@param metaTable table
---@param key string
---@param typeId number
function UILinkerUtils.setLinkerFunction(target, metaTable, key, typeId)
    if typeId == EnumLinkerTypeId.UnityEvent then
        metaTable.uiExpansion:LinkerSetAction(key, function()
            metaTable._methods[key](target)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEventBoolean then
        metaTable.uiExpansion:LinkerSetActionBoolean(key, function(booleanValue)
            metaTable._methods[key](target, booleanValue)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEventSingle then
        metaTable.uiExpansion:LinkerSetActionSingle(key, function(str)
            metaTable._methods[key](target, str)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEventInt32 then
        metaTable.uiExpansion:LinkerSetActionInt32(key, function(str)
            metaTable._methods[key](target, str)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEventString then
        metaTable.uiExpansion:LinkerSetActionString(key, function(str)
            metaTable._methods[key](target, str)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEventVector2 then
        metaTable.uiExpansion:LinkerSetActionVector2(key, function(vec2)
            metaTable._methods[key](target, vec2)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEvent2 then
        metaTable.uiExpansion:LinkerSetAction2(key, function()
            metaTable._methods[key](target)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEvent2Boolean then
        metaTable.uiExpansion:LinkerSetAction2Boolean(key, function(bool)
            metaTable._methods[key](target, bool)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEvent2Single then
        metaTable.uiExpansion:LinkerSetAction2Single(key, function(single)
            metaTable._methods[key](target, single)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEvent2Int32 then
        metaTable.uiExpansion:LinkerSetAction2Int32(key, function(bool)
            metaTable._methods[key](target, bool)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEvent2String then
        metaTable.uiExpansion:LinkerSetAction2String(key, function(str)
            metaTable._methods[key](target, str)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.UnityEvent2Vector2 then
        metaTable.uiExpansion:LinkerSetAction2Vector2(key, function(vec2)
            metaTable._methods[key](target, vec2)
            --metaTable._toRmMethod(key)
        end);
    elseif typeId == EnumLinkerTypeId.SystemEventActionIntAndObject then
        metaTable.uiExpansion:LinkerSetSystemActionIntAndObject(key, function(single, item)
            metaTable._methods[key](target, single, item)
        end);
    elseif typeId == EnumLinkerTypeId.SystemEventActionIntAndBool then
        metaTable.uiExpansion:LinkerSetSystemActionIntAndBool(key, function(single, bool)
            metaTable._methods[key](target, single, bool)
        end);
    elseif typeId == EnumLinkerTypeId.SystemEventActionObject then
        metaTable.uiExpansion:LinkerSetSystemActionObject(key, function(object)
            metaTable._methods[key](target, object)
        end);
    elseif typeId == EnumLinkerTypeId.DelegateInt then
        metaTable.uiExpansion:LinkerSetDelegateInt(key, function(single)
            return metaTable._methods[key](target, single)
        end);
    else
        error("dont has this type:" .. typeId)
    end
end

return UILinkerUtils