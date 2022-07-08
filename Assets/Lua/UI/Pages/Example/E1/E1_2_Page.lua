local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E1_2_Page : UIPageBase
local E1_2_Page = class("E1_2_Page", UIPageBase)

---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E1_2_Page:onCreated(options)
    --self.comps.E_InputName..AddListener:AddListener(handler(self,self.OnClick_E_BtnClose))
    self.comps.E_InputName.NDInputField.onValueChanged:AddListener(handler(self, self.onInputNameValueChanged))
end


---onInputNameValueChanged
---@param text string
function E1_2_Page:onInputNameValueChanged(text)
    self.comps.E_TextAction.NDText.text = text
end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_2_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_BtnClose = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(2),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(3),
        },
        E_TextAction = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(6),
        },
        E_TextTarget = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(7),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(8),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(9),
        },
        E_InputName = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(10),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(11),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(12),
            ---@type NDInputField
            NDInputField = self.uiExpansion:GetBindObject(13),
        },
        E_BtnSubmit = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(14),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(15),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(16),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(17),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E1_2_Page:BindCommonFunction()
   self.comps.E_BtnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnClose))
   self.comps.E_BtnSubmit.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnSubmit))
end

--endRegionAutoBind

-----OnClick_E_BtnClose()
function E1_2_Page:OnClick_E_BtnClose()
    self:closeMe()
end

-----OnClick_E_BtnSubmit()
function E1_2_Page:OnClick_E_BtnSubmit()
    self.comps.E_TextTarget.NDText.text = self.comps.E_TextAction.NDText.text
end
return E1_2_Page
