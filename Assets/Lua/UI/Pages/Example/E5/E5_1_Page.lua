local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E5_1_Page : UIPageBase
local E5_1_Page = class("E5_1_Page", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E5_1_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E5_1_Page:onCreated(options)
end

---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E5_1_Page:onOpen(options)
    self.comps.E_NDInputField.NDInputField.onValueChanged:AddListener(handler(self, self.onInputFieldChange))
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E5_1_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E5_1_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E5_1_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E5_1_Page:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function E5_1_Page:onDestroy(options)
end
--endregion

function E5_1_Page:onInputFieldChange(text)
    self:levelChange(text)
end

---@param string string
function E5_1_Page:levelChange(value)
    if type(value) == "string" then
        Api.checkerLevel:SetCheckerLevel(tonumber(value))
    else
        Api.checkerLevel:SetCheckerLevel(value)
    end
end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E5_1_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_BtnClose = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(2),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(3),
        },
        E_BtnPageA = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(7),
        },
        E_BtnPageB = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(8),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(9),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(10),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(11),
        },
        E_NDInputField = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(12),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(13),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(14),
            ---@type NDInputField
            NDInputField = self.uiExpansion:GetBindObject(15),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E5_1_Page:BindCommonFunction()
   self.comps.E_BtnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnClose))
   self.comps.E_BtnPageA.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnPageA))
   self.comps.E_BtnPageB.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnPageB))
end

--endRegionAutoBind

-----OnClick_E_BtnClose()
function E5_1_Page:OnClick_E_BtnClose()
    self:closeMe()
end

-----OnClick_E_BtnPageA()
function E5_1_Page:OnClick_E_BtnPageA()
    UIMgr:open("E5_1_A_Page", nil, nil)
end

-----OnClick_E_BtnPageB()
function E5_1_Page:OnClick_E_BtnPageB()
    UIMgr:open("E5_1_B_Page", nil, nil)
end


return E5_1_Page
