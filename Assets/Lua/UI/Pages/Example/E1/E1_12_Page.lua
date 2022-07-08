local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E1_12_Page : UIPageBase
local E1_12_Page = class("E1_12_Page", UIPageBase)

---methods 构造绑定的方法列表， 用来完成子模块emit事件响应注册
function E1_12_Page:methods()

end

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E1_12_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E1_12_Page:onCreated(options)
end

---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E1_12_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E1_12_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E1_12_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E1_12_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E1_12_Page:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function E1_12_Page:onDestroy(options)
end
--endregion


---BindCustomFunction绑定自定义事件方法
function E1_12_Page:BindCustomFunction()

end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_12_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.E_Button = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(0),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(1),
       ---@type NDImage
       NDImage = self.uiExpansion:GetBindObject(2),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(3),
    }
    self:BindCustomModules()
    self:BindCommonFunction()
end


---BindCustomModules绑定静态Module方法
function E1_12_Page:BindCustomModules()
end

---BindCustomFunction绑定通用事件方法
function E1_12_Page:BindCommonFunction()
   self.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
   self:BindCustomFunction()
end

--endRegionAutoBind

-----OnClick_E_Button()
function E1_12_Page:OnClick_E_Button()
    self:closeMe({withAnimation = true})
end
return E1_12_Page
