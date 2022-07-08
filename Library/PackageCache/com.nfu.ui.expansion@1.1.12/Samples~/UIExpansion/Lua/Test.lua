--regionCustomCreator 项目可自定义的ui page代码生成部分
local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class Test : UIPageBase
local Test = class("Test", UIPageBase)

---methods 构造绑定的方法列表， 用来完成子模块emit事件响应注册
function Test:methods()

end

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function Test:observerDataStore()
    return {
    }
end
--endregion

---@see UIPageBase#onPreData
---onPreData 在Page创建实例时触发，一个实例只会触发一次，这时还未执行onPreData
function Test:onPreData()
    
end

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function Test:onCreated(options)
end

---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function Test:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function Test:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function Test:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function Test:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function Test:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function Test:onDestroy(options)
end
--endregion

--regionEndCustomCreator

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function Test:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.E_Text = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(0),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(1),
       ---@type NDText
       NDText = self.uiExpansion:GetBindObject(2),
    }
    self.E_Button = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(3),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(4),
       ---@type NDImage
       NDImage = self.uiExpansion:GetBindObject(5),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(6),
    }
    self.E_ModuleTest = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(7),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(8),
       ---@type UIExpansion
       UIExpansion = self.uiExpansion:GetBindObject(9),
    }
    self:BindCustomModules()
    self:BindCommonFunction()
end


---BindCustomModules绑定静态Module方法
function Test:BindCustomModules()
   ---@type ModuleTest
   self.M_E_ModuleTest = self:registerModule("M_E_ModuleTest","ModuleTest",self.E_ModuleTest.GameObject)
end

---BindCustomFunction绑定通用事件方法
function Test:BindCommonFunction()
   self.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
   self:BindCustomFunction()
end

--endRegionAutoBind

---BindCustomFunction绑定自定义事件方法
function Test:BindCustomFunction()

end

-----OnClick_E_Button()
function Test:OnClick_E_Button()

end

return Test
