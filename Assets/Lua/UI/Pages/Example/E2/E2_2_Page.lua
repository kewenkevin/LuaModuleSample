--regionCustomCreator 项目可自定义的ui page代码生成部分
local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E2_2_Page : UIPageBase
local E2_2_Page = class("E2_2_Page", UIPageBase)


---region 页面数据组织
function E2_2_Page:data()
    local E2_2_PageData = 
    {
        msgText = "",
        inputPasswordText = "",
    }
    return E2_2_PageData
end


---methods 构造绑定的方法列表， 用来完成子模块emit事件响应注册
function E2_2_Page:methods()

end

---Listen 监控模块数仓变化，这里注册事件接口
function E2_2_Page:Listen()
	return{
	}
end

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E2_2_Page:observerDataStore()
    return {
        cipher = {
            password = function (oldVal,newVal)
               self:onCliperPasswordChanged(oldVal,newVal)
            end
        }
    }
end

function E2_2_Page:onCliperPasswordChanged(oldVal,newVal)
    self.data.msgText = newVal
    self.E_TextAction.NDText.text  =  self.data.msgText
end
--endregion

---@see UIPageBase#onPreData
---onPreData 在Page创建实例时触发，一个实例只会触发一次，这时还未执行onPreData
function E2_2_Page:onPreData()
    
end

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E2_2_Page:onCreated(options)
    self.E_InputName.NDInputField.onValueChanged:AddListener(handler(self, self.onInputNameValueChanged))
    self.E_InputName.NDInputField.onValueChanged:AddListener(handler(self, self.onInputNameValueChanged))
    self.data = self:data()
end

---onInputNameValueChanged
---@param text string
function E2_2_Page:onInputNameValueChanged(text)
    self.data.inputPasswordText = text
end


---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E2_2_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E2_2_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E2_2_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E2_2_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E2_2_Page:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function E2_2_Page:onDestroy(options)
end
--endregion

--regionEndCustomCreator


-----OnClick_E_BtnClose()
function E2_2_Page:OnClick_E_BtnClose()
    self:closeMe()
end

-----OnClick_E_BtnSubmit()
function E2_2_Page:OnClick_E_BtnSubmit()
    Store.cipher:setPassword(self.data.inputPasswordText)
end

---BindCustomFunction绑定自定义事件方法
function E2_2_Page:BindCustomFunction()

end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E2_2_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.E_BtnClose = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(0),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(1),
       ---@type NDImage
       NDImage = self.uiExpansion:GetBindObject(2),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(3),
    }
    self.E_BtnSubmit = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(4),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(5),
       ---@type NDImage
       NDImage = self.uiExpansion:GetBindObject(6),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(7),
    }
    self.E_InputName = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(8),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(9),
       ---@type NDImage
       NDImage = self.uiExpansion:GetBindObject(10),
       ---@type NDInputField
       NDInputField = self.uiExpansion:GetBindObject(11),
    }
    self.E_TextAction = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(12),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(13),
       ---@type NDText
       NDText = self.uiExpansion:GetBindObject(14),
    }
    self:BindCustomModules()
    self:BindCommonFunction()
end


---BindCustomModules绑定静态Module方法
function E2_2_Page:BindCustomModules()
end

---BindCustomFunction绑定通用事件方法
function E2_2_Page:BindCommonFunction()
   self.E_BtnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnClose))
   self.E_BtnSubmit.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnSubmit))
   self:BindCustomFunction()
end

--endRegionAutoBind

return E2_2_Page
