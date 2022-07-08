local UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")

---@class E5_2_ModuleA : UIModuleBase
local E5_2_ModuleA = class("E5_2_ModuleA", UIModuleBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E5_2_ModuleA:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIBase#onBinding
---onBinding 在Module数据绑定完成后触发，当Module存在复用时，每次会重新触发
function E5_2_ModuleA:onBinding()
end

---@see UIBase#onUnBinding
---onUnBinding 在Module解除绑定后触发，当Module存在复用时，每次会重新触发
function E5_2_ModuleA:onUnBinding()
end
--endregion

--region 页面数据辅助
--endregion

function E5_2_ModuleA:Refresh(data)
    self.data = data
    self.comps.E_Text.NDText.text = data.textVal
end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E5_2_ModuleA:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_Button = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(2),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(3),
        },
        E_Text = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(6),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E5_2_ModuleA:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

-----OnClick_E_Button()
function E5_2_ModuleA:OnClick_E_Button()
    self:emit("onChildEvent", "E5_2_ModuleA " .. self.data.textVal)
end
return E5_2_ModuleA
