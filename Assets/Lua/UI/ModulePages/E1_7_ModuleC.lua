local UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")

---@class E1_7_ModuleC : UIModuleBase
local E1_7_ModuleC = class("E1_7_ModuleC", UIModuleBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E1_7_ModuleC:observerDataStore()
    return {
    }
end
--endregion

function E1_7_ModuleC:onBinding()
    self.comps.E_Text.NDText.text = ""
end

function E1_7_ModuleC:onUnBinding()
    
end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_7_ModuleC:initAutoBind(gameObject)
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
        E_Button = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(6),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(7),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(8),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(9),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E1_7_ModuleC:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

-----OnClick_E_Button()
function E1_7_ModuleC:OnClick_E_Button()
    self:emit("onChildEvent", "E1_7_ModuleC")
end
return E1_7_ModuleC
