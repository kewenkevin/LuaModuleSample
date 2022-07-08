local UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")

---@class E1_7_ModuleB : UIModuleBase
local E1_7_ModuleB = class("E1_7_ModuleB", UIModuleBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E1_7_ModuleB:observerDataStore()
    return {
    }
end
--endregion

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E1_7_ModuleB:observerDataStore()
    return {
    }
end
--endregion

---methods 构造绑定的方法列表，和子模块emit事件响应注册
function E1_7_ModuleB:methods()
    return {
        onChildEvent = self.onChildEvent,
    }
end

function E1_7_ModuleB:onChildEvent(name)
    print("onChildEvent E1_7_ModuleB " .. name)
    self:emit("onChildEvent", "onChildEvent E1_7_ModuleB " .. name)
end

---@see UIBase#onBinding
---onBinding 在Module数据绑定完成后触发，当Module存在复用时，每次会重新触发
function E1_7_ModuleB:onBinding()
    self.comps.E_Text.NDText.text = ""
end

---@see UIBase#onUnBinding
---onUnBinding 在Module解除绑定后触发，当Module存在复用时，每次会重新触发
function E1_7_ModuleB:onUnBinding()

end


--region 页面数据辅助
--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_7_ModuleB:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_TitleText = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(2),
        },
        E_Text = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(3),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(4),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(5),
        },
        E_E1_7_ModuleC_0 = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(6),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(7),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(8),
        },
        E_E1_7_ModuleC_1 = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(9),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(10),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(11),
        },
        E_E1_7_ModuleC_2 = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(12),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(13),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(14),
        },
    }

    self:BindCommonFunction()
    self:BindCustomModules()
end

function E1_7_ModuleB:BindCommonFunction()
end

--endRegionAutoBind

function E1_7_ModuleB:BindCustomModules()
    ---@type UIE1_7_ModuleC
    self.M_E_E1_7_ModuleC_0 = self:registerModule("M_E_E1_7_ModuleC_0", "E1_7_ModuleC", self.comps.E_E1_7_ModuleC_0.GameObject)
    ---@type UIE1_7_ModuleC
    self.M_E_E1_7_ModuleC_1 = self:registerModule("M_E_E1_7_ModuleC_1", "E1_7_ModuleC", self.comps.E_E1_7_ModuleC_1.GameObject)
    ---@type UIE1_7_ModuleC
    self.M_E_E1_7_ModuleC_2 = self:registerModule("M_E_E1_7_ModuleC_2", "E1_7_ModuleC", self.comps.E_E1_7_ModuleC_2.GameObject)
end

function E1_7_ModuleB:onOpen()
    
end

return E1_7_ModuleB
