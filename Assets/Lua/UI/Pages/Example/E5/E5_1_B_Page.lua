local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class UIE5_1_B_Page : UIPageBase
local E5_1_B_Page = class("E5_1_B_Page", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E5_1_B_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E5_1_B_Page:onCreated(options)
end
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E5_1_B_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E5_1_B_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E5_1_B_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E5_1_B_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E5_1_B_Page:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function E5_1_B_Page:onDestroy(options)
end
--endregion


--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E5_1_B_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_Button = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(2),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(3),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E5_1_B_Page:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

-----OnClick_E_Button()
function E5_1_B_Page:OnClick_E_Button()
    self:closeMe()
end
return E5_1_B_Page
