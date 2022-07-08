local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E5_2_Page : UIPageBase
local E5_2_Page = class("E5_2_Page", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E5_2_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E5_2_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E5_2_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E5_2_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E5_2_Page:onHide(options)
end
--endregion

--region 页面数据辅助

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E5_2_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_BtnLoad = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(2),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(3),
        },
        E_BtnCreate = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(7),
        },
        E_Widget = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(8),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(9),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(10),
        },
        E_BtnClose = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(11),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(12),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(13),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(14),
        },
    }
    self.modules = {
          Module_E_Widget = require("UI.Pages.Example.E5.E5_2_SampleWidget").new(self.comps.E_Widget.GameObject),
    }
    self:BindCommonFunction()
end

function E5_2_Page:BindCommonFunction()
   self.comps.E_BtnLoad.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnLoad))
   self.comps.E_BtnCreate.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnCreate))
   self.comps.E_BtnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnClose))
end

--endRegionAutoBind

-----OnClick_E_BtnLoad()
function E5_2_Page:OnClick_E_BtnLoad()
    local this = self
    UIWidgetFactory:LoadAndCreatePanelAsync("Assets/Sample/ResourceManager/ResourcesAssets/UI/GUI/Pages/E5_2_FlexPanel/E5_2_SampleWidget.prefab",function(widget)
        widget.gameObject.transform:SetParent(this.gameObject.transform,false)
    end)
end

-----OnClick_E_BtnCreate()
function E5_2_Page:OnClick_E_BtnCreate()
    ---@type UIE5_2_SampleWidget
    local widget = UIWidgetFactory:BindPanelWithGameObject(self.comps.E_Widget.GameObject,"UI.Pages.Example.E5.E5_2_SampleWidget")
    widget:destroy()
end

-----OnClick_E_BtnClose()
function E5_2_Page:OnClick_E_BtnClose()
    self:closeMe()
end

return E5_2_Page
