local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E1_7_Page : UIPageBase
local E1_7_Page = class("E1_7_Page", UIPageBase)

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E1_7_Page:onCreated(options)
end
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E1_7_Page:onOpen(options)
    self.M_E_E1_7_ModuleA.M_E_E1_7_ModuleB1.M_E_E1_7_ModuleC_1.comps.E_Text.NDText.text = "Hello world"
    self.M_E_E1_7_ModuleA:DoSomething()
end
--endregion

---methods 构造绑定的方法列表，和子模块emit事件响应注册
function E1_7_Page:methods()
    return {
        onChildEvent = self.onChildEvent,
    }
end

function E1_7_Page:onChildEvent(name)
    print("onChildEvent: E1_7_Page " .. name)
end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_7_Page:initAutoBind(gameObject)
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
        E_E1_7_ModuleA = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(6),
        },
    }

    self:BindCommonFunction()
    self:BindCustomModules()
end

function E1_7_Page:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

function E1_7_Page:BindCustomModules()
    ---@type UIE1_7_ModuleA
    self.M_E_E1_7_ModuleA = self:registerModule("M_E_E1_7_ModuleA", "E1_7_ModuleA", self.comps.E_E1_7_ModuleA.GameObject)
end

-----OnClick_E_Button()
function E1_7_Page:OnClick_E_Button()
    self:closeMe()
end
return E1_7_Page
