
__G_ERROR_TRACK = function(err)
    C_LuaUtility.ErrorTrack(debug.traceback(err,2))
end

__G_UNITY_EDITOR = false