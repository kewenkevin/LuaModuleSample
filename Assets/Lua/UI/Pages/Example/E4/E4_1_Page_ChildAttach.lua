local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E4_1_Page_ChildAttach : UIPageBase
local E4_1_Page_ChildAttach = class("E4_1_Page_ChildAttach", UIPageBase)


---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E4_1_Page_ChildAttach:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E4_1_Page_ChildAttach:onOpen(options)
    UIMgr:attach(self.pageConfig.pageName)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E4_1_Page_ChildAttach:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E4_1_Page_ChildAttach:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E4_1_Page_ChildAttach:onHide(options)
end
--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E4_1_Page_ChildAttach:initAutoBind(gameObject)
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
        E_btnEnableTouch = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(7),
        },
        E_btnDisableTouch = {
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

function E4_1_Page_ChildAttach:BindCommonFunction()
   self.comps.E_btnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnClose))
   self.comps.E_btnEnableTouch.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnEnableTouch))
   self.comps.E_btnDisableTouch.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnDisableTouch))
end

--endRegionAutoBind

-----OnClick_E_btnClose()
function E4_1_Page_ChildAttach:OnClick_E_btnClose()
    self:closeMe()
end

-----OnClick_E_btnEnableTouch()
function E4_1_Page_ChildAttach:OnClick_E_btnEnableTouch()
    UIMgr:enableTouchable("E4_1_Page_ChildAttach")
end

-----OnClick_E_btnDisableTouch()
function E4_1_Page_ChildAttach:OnClick_E_btnDisableTouch()
    UIMgr:disableTouchable("E4_1_Page_ChildAttach")
end
return E4_1_Page_ChildAttach
