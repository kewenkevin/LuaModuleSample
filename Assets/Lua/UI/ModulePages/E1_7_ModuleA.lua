local UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")

---@class E1_7_ModuleA : UIModuleBase
local E1_7_ModuleA = class("E1_7_ModuleA", UIModuleBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E1_7_ModuleA:observerDataStore()
    return {
    }
end
--endregion

---@see UIBase#onBinding
---onBinding 在Module数据绑定完成后触发，当Module存在复用时，每次会重新触发
function E1_7_ModuleA:onBinding()
    self.comps.E_Text_1.NDText.text = ""
end

---@see UIBase#onUnBinding
---onUnBinding 在Module解除绑定后触发，当Module存在复用时，每次会重新触发
function E1_7_ModuleA:onUnBinding()
    
end

---methods 构造绑定的方法列表，和子模块emit事件响应注册
function E1_7_ModuleA:methods()
    return {
        onChildEvent = self.onChildEvent,
    }
end

function E1_7_ModuleA:onChildEvent(name)
    print("onChildEvent E1_7_ModuleA " .. name)
    
    self:emit("onChildEvent", "onChildEvent E1_7_ModuleA " .. name)
end

function E1_7_ModuleA:DoSomething()
    self.M_E_E1_7_ModuleB1.comps.E_Text.NDText.text = "KKKKKKKK"
end


--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_7_ModuleA:initAutoBind(gameObject)
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
        E_Text_1 = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(3),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(4),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(5),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(6),
        },
        E_E1_7_ModuleB = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(7),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(8),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(9),
        },
        E_E1_7_ModuleB1 = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(10),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(11),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(12),
        },
    }

    self:BindCommonFunction()
    self:BindCustomModules()
end

function E1_7_ModuleA:BindCommonFunction()
   self.comps.E_Text_1.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Text_1))
end

--endRegionAutoBind

function E1_7_ModuleA:BindCustomModules()
    ---@type UIE1_7_ModuleB
    self.M_E_E1_7_ModuleB = self:registerModule("M_E_E1_7_ModuleB", "E1_7_ModuleB", self.comps.E_E1_7_ModuleB.GameObject)
    ---@type UIE1_7_ModuleB
    self.M_E_E1_7_ModuleB1 = self:registerModule("M_E_E1_7_ModuleB1", "E1_7_ModuleB", self.comps.E_E1_7_ModuleB1.GameObject)
end

-----OnClick_E_Text_1()
function E1_7_ModuleA:OnClick_E_Text_1()

end
return E1_7_ModuleA
