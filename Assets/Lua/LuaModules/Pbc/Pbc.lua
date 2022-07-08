local pb = require "LuaModules.Pbc.protobuf"
local pbc = {}
local protoCSCmd
local protoSCCmd
local __G_ERROR_TRACK = __G_ERROR_TRACK

local decodeed = true;
----------------------------------------------------------------------------------------------
local c = require("protobuf.c")
local cext = require("protobuf.c.ext")
local _wmessage_int64 = cext._wmessage_int64
assert(nil ~= _wmessage_int64, "pbc-int64 is missing!!!!")
c._wmessage_int64 = function(t, k, v)
    local vt = type(v)
    if vt == "number" then
        return _wmessage_int64(t, k, v)
    elseif vt == "string" and #v == 8 then--TODO:test
        return _wmessage_int64(t, k, v)
    else
        local v64 = tolua.toint64(v)
        return _wmessage_int64(t, k, v64)
    end
end
c._wmessage_int52 = c._wmessage_int64
c._decode = cext._decode

lltoa = cext.int64ToString
atoll = cext.int64ToNumber
local default_cache_tb = {}
local __decode = pb.decode
local decode = function(typename, buffer, length)
    if nil ~= buffer then
        local ret, err = __decode(typename, buffer, length);
        if false == ret then
            print(string.format("<color = #FF0000>%s</color>",err.."\t"..typename));
            decodeed = false;
            return false;
        end
        return ret
    end
    --default data
    local def = default_cache_tb[typename];
    if nil == def then
        def = pb.default(typename, {});
        default_cache_tb[typename] = def;
    end
    return def;
end

function pbc:initialize(options)
    protoCSCmd = options.CSCmd
    protoSCCmd = options.SCCmd
    pb.register(options.bytes)
end

function pbc:encode( type,msg,data )
    local p = nil
    local msgId = nil
    local errorMsg = nil
    -- local ok,err = xpcall(function()
    --     if type == "cs" then
    --         local name = protoCSCmd.msg[msg]
    --         if name == nil then
    --             error("serialize cmd is null > "..cmd)
    --         else
    --             p =  pb.encode(name,data)
    --             msgId = protoCSCmd.msgToId[msg]
    --         end
    --     elseif type == "sc" then
    --         local name = protoSCCmd.msg[msg]
    --         if name == nil then
    --             error("serialize cmd is null > "..cmd)
    --         else
    --             p = pb.encode(name,data)
    --             msgId = protoCSCmd.msgToId[msg]
    --         end
    --     else
    --         error("serialize type is error > "..type)
    --     end
    -- end,function()
    --     error(debug.traceback())
    -- end,33)
    -- if not ok then
    --     error(err)
    -- else
    --     return p,msgId
    -- end


    if type == "cs" then
        local name = protoCSCmd.msg[msg]
        if name == nil then
            error("serialize cmd is null > "..cmd)
        else
            p, errorMsg = pb.encode(name,data)
            msgId = protoCSCmd.msgToId[msg]
        end
    elseif type == "sc" then
        local name = protoSCCmd.msg[msg]
        if name == nil then
            error("serialize cmd is null > "..cmd)
        else
            p, errorMsg =pb.encode(name,data)
            msgId = protoCSCmd.msgToId[msg]
        end
    else
        error("serialize type is error > "..type)
    end
    if errorMsg then
        error("serialize type is error > "..msg .. "  " .. errorMsg)
    end
    return p,msgId
end

function pbc:decode( type,cmd,data )
    local p = nil
    local ok,err = xpcall(function()
        if type == "cs" then
            local name = protoCSCmd.id[cmd]
            if name == nil then
                error("serialize cmd is null > "..cmd)
            else
                p =  decode(protoCSCmd.id[cmd],data)
            end
        elseif type == "sc" then
            local name = protoSCCmd.id[cmd]
            if name == nil then
                error("serialize cmd is null > "..cmd)
            else
                p =  decode(name,data)
            end
        else
            error("serialize type is error > "..type)
        end
    end,function()
        if __G_ERROR_TRACK then
            __G_ERROR_TRACK(cmd)
        else
            print("ERROR",debug.traceback(cmd))
        end
    end,33)
    if not ok then
        if __G_ERROR_TRACK then
            __G_ERROR_TRACK(cmd)
        else
            print("ERROR",debug.traceback(cmd))
        end
    else
        return p
    end
end

function pbc:getCSCmd( msg )
    return protoCSCmd.msgToId[msg]
end

function pbc:getSCCmd(msg)
    return protoSCCmd.msgToId[msg]
end

return pbc