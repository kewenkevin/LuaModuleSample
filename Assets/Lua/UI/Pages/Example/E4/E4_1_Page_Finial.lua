local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E4_1_Page_Finial : UIPageBase
local E4_1_Page_Finial = class("E4_1_Page_Finial", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E4_1_Page_Finial:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E4_1_Page_Finial:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E4_1_Page_Finial:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E4_1_Page_Finial:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E4_1_Page_Finial:onHide(options)
end
--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E4_1_Page_Finial:initAutoBind(gameObject)
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
        E_btnBackTo = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(7),
        },
        E_btnOpenOther = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(8),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(9),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(10),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(11),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E4_1_Page_Finial:BindCommonFunction()
   self.comps.E_btnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnClose))
   self.comps.E_btnBackTo.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnBackTo))
   self.comps.E_btnOpenOther.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnOpenOther))
end

--endRegionAutoBind

-----OnClick_E_btnClose()
function E4_1_Page_Finial:OnClick_E_btnClose()
    self:closeMe()
end

-----OnClick_E_btnBackTo()
function E4_1_Page_Finial:OnClick_E_btnBackTo()
    UIMgr:backTo("E4_1_Page")
end

-----OnClick_E_btnOpenOther()
function E4_1_Page_Finial:OnClick_E_btnOpenOther()
    UIMgr:open("E4_1_Page_Parallel")
end

return E4_1_Page_Finial
