if LUA_XLua then
    ND = CS.ND
end

C_Net = ND.Framework.Managers.NetworkManager
C_UIExpansion = ND.UI.UIExpansion
C_ResFunc = ND.Managers.ResourceMgr.Runtime.NFUResource
C_ResLoader = ND.Managers.ResourceMgr.Runtime.ResLoader
C_LuaUtility = ND.Gameplay.Utilities.LuaUtility
D_ResLoadCallBack = ND.Managers.ResourceMgr.Runtime.Res.ResLoadCompleteCallBack

if LUA_XLua then
    UnityEngine = CS.UnityEngine
    Vector2 = UnityEngine.Vector2
    Vector3 = UnityEngine.Vector3
    Vector4 = UnityEngine.Vector4
    Rect = UnityEngine.Rect
    Ray = UnityEngine.Ray
    RaycastHit = UnityEngine.RaycastHit
    Quaternion = UnityEngine.Quaternion
    Color = UnityEngine.Color
    Bounds = UnityEngine.Bounds

    Time = require("ToLua.UnityEngine.Time")
    list = require "list"
    require "event"
    --require "typeof"
    require "System.Timer"
end 