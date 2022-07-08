--[[package.cpath = package.cpath .. ';C:/Users/Administrator/AppData/Roaming/JetBrains/Rider2020.3/plugins/intellij-emmylua/classes/debugger/emmy/windows/x64/?.dll'
local dbg = require('emmy_core')
dbg.tcpConnect('localhost', 9966)]]

if jit then
	jit.off();-- 关闭
end
require "Alias"
require "Define"
require "LuaModules.index"
require "UI.UIEx.UIExWrapper"

---@type DataStore
Store = require "Store.Store"

---@type UIWidgetFactory
UIWidgetFactory = require "LuaModules.UILib.Base.UIWidgetFactory".new()

---@type UIManager
UIMgr = require "LuaModules.UILib.UIMgr".new()

---@type UIPageBase
G_UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@type UIModuleBase
G_UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")

UIMgrProxy = require 'Api.UIMgrProxy'

Api = require "Api.Api"
isInited = false

UIWidgetFactory:registerAssetHelperClass(require("LuaModules.UILib.Base.UIAssetHelper"))
UIMgr:registerAssetHelperClass(require("LuaModules.UILib.Base.UIAssetHelper"))
UIMgr:setUIRoot(UnityEngine.GameObject.Find("UIRoot"))
UIMgr:initialize(require("UI.ConfigV2"), Store)
UpdateBeat:Add(UIMgr.update,UIMgr)
UIMgr:open(UIMgr:getUIConfig().mainPage,nil,function(result)
	isInited = true
end)

CheckerMgr = require("Api/CheckerMgr")
CheckerMgr:init()

local function _checkProject()
	setmetatable(
			_G,
			{
				__newindex = function(v1, v2, v3)
					if (v2 ~= "i" and v2 ~= "j" and v2 ~= "socket" and v2 ~= "ltn12" and v2~="reload") then
						error("不能添加全局变量 " .. v2, 2)
					else
						rawset(v1, v2, v3)
					end
				end,
				__index = function(t, k)
					if (k ~= "jit") then
						error("未注册的全局变量" .. k, 2)
					end
				end
			}
	)
end

function KeyBoardEscape()
	UIMgrProxy:globalBack()
end

function KeyBoardF1()
	UIMgr:disableTouchable("F1-2")
end

function KeyBoardF2()
	UIMgr:enableTouchable("F1-2")
end

function KeyBoardF3()
	UIMgr:disableTouchable("F3-4")
end

function KeyBoardF4()
	UIMgr:enableTouchable("F3-4")
end
function OnApplicationQuit()

end

_checkProject()