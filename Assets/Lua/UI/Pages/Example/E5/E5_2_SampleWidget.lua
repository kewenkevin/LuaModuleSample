local UIWidgetBase = require("LuaModules.UILib.Base.UIWidgetBase")

---@class E5_2_SampleWidget : UIWidgetBase
local E5_2_SampleWidget = class("E5_2_SampleWidget", UIWidgetBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E5_2_SampleWidget:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIWidgetBase#onCreated
---onCreated 界面复写 界面复写 在Panel创建实例时触发，一个实例只会触发一次，这时已完成进行数据绑定
function E5_2_SampleWidget:onCreated(options)
    local grids = {}
    for i = 1, 50 do
        grids[i] = {script = "E5_2_ModuleA",  textVal ="Item"..i}
    end
    
    self.comps.E_Vertical_NDGridView.NDGridView.data = grids
    self.comps.E_Vertical_NDGridView.NDGridView:RefreshCells()
end

---@see UIWidgetBase#onDestroy
---onDestroy 界面复写 在Page实例销毁时触发
function E5_2_SampleWidget:onDestroy(options)
end
--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E5_2_SampleWidget:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_Text = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(2),
        },
        E_Button = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(3),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(4),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
        },
        E_Slider = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(7),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(8),
            ---@type NDSlider
            NDSlider = self.uiExpansion:GetBindObject(9),
        },
        E_Vertical_NDGridView = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(10),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(11),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(12),
            ---@type NDGridView
            NDGridView = self.uiExpansion:GetBindObject(13),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
    self:BindCustomFunction()
end

function E5_2_SampleWidget:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

-----OnClick_E_Button()
function E5_2_SampleWidget:OnClick_E_Button()

end

function E5_2_SampleWidget:BindCustomFunction()
    self.comps.E_Vertical_NDGridView.NDGridView = ElementEx(self, self.comps.E_Vertical_NDGridView.NDGridView, UIExElement.NDGridView)
end

---methods 构造绑定的方法列表， 用来完成UI交互事件响应注册和子模块emit事件响应注册
function E5_2_SampleWidget:methods()
    return {
        onChildEvent = self.onChildEvent,
    }
end

function E5_2_SampleWidget:onChildEvent(info)
    print("E5_2_SampleWidget:onChildEvent " .. info)
end

-----OnClick_E_Button()
function E5_2_SampleWidget:OnClick_E_Button()
    self:destroy()
end

return E5_2_SampleWidget
