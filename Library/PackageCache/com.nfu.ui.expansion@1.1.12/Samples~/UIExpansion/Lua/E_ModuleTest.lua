--regionCustomCreator 项目可自定义的ui module代码生成部分
local UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")

---@class E_ModuleTest : UIModuleBase
local E_ModuleTest = class("E_ModuleTest", UIModuleBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E_ModuleTest:observerDataStore()
    return {
    }
end

---methods 构造绑定的方法列表，和子模块emit事件响应注册
function E_ModuleTest:methods()

end

---@see UIBase#onBinding
---onBinding 在Module数据绑定完成后触发，当Module存在复用时，每次会重新触发
function E_ModuleTest:onBinding()
    
end

---@see UIBase#onUnBinding
---onUnBinding 在Module解除绑定后触发，当Module存在复用时，每次会重新触发
function E_ModuleTest:onUnBinding()
    
end

--region 页面生命周期
---@see UIBase#onCreated
---onCreated 
function E_ModuleTest:onCreated(parent)
    
end
--regionEndCustomCreator

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E_ModuleTest:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    --UIExpansion根节点组件
    self.Root = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(0),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(1),
       ---@type UIExpansion
       UIExpansion = self.uiExpansion:GetBindObject(2),
    }
    self.E_Text = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(3),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(4),
       ---@type NDText
       NDText = self.uiExpansion:GetBindObject(5),
    }
    self:BindCustomModules()
    self:BindCommonFunction()
end


---BindCustomModules绑定静态Module方法
function E_ModuleTest:BindCustomModules()
end

---BindCustomFunction绑定通用事件方法
function E_ModuleTest:BindCommonFunction()
   self:BindCustomFunction()
end

--endRegionAutoBind

---BindCustomFunction绑定自定义事件方法
function E_ModuleTest:BindCustomFunction()

end

return E_ModuleTest
