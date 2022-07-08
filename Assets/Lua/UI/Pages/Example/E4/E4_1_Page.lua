local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E4_1_Page : UIPageBase
local E4_1_Page = class("E4_1_Page", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E4_1_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E4_1_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E4_1_Page:onClose(options)
end
--endregion


--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E4_1_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_btnClose = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(2),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(3),
        },
        E_btnOpenChild = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(7),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E4_1_Page:BindCommonFunction()
   self.comps.E_btnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnClose))
   self.comps.E_btnOpenChild.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnOpenChild))
end

--endRegionAutoBind

-----OnClick_E_btnClose()
function E4_1_Page:OnClick_E_btnClose()
    self:closeMe()
end

-----OnClick_E_btnOpenChild()
function E4_1_Page:OnClick_E_btnOpenChild()
    UIMgr:open("E4_1_Page_Child")
end
return E4_1_Page
