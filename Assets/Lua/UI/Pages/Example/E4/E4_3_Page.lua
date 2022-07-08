local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E4_3_Page : UIPageBase
local E4_3_Page = class("E4_3_Page", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E4_3_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期

---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E4_3_Page:onOpen(options)
    self.comps.E_title.NDText.text = options.title
    self.comps.E_content.NDText.text = options.content
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E4_3_Page:onClose(options)
end
--endregion

--region 页面数据辅助

---onCloseClick
function E4_3_Page:onCloseClick(value)
    self:closeMe()
end
--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E4_3_Page:initAutoBind(gameObject)
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
        E_title = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(6),
        },
        E_content = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(7),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(8),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(9),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E4_3_Page:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

-----OnClick_E_Button()
function E4_3_Page:OnClick_E_Button()
    self:closeMe()
end

return E4_3_Page
