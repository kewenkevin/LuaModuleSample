local UIEnums = require("LuaModules.UILib.Enums.UIEnums")
local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E4_4_Page : UIPageBase
local E4_4_Page = class("E4_4_Page", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E4_4_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E4_4_Page:onCreated(options)
end
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E4_4_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E4_4_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E4_4_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E4_4_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E4_4_Page:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function E4_4_Page:onDestroy(options)
end
--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E4_4_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_btnHome = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(2),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(3),
        },
        E_btnGlobalBack = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(7),
        },
        E_btnBack = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(8),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(9),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(10),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(11),
        },
        E_btnClose = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(12),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(13),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(14),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(15),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E4_4_Page:BindCommonFunction()
   self.comps.E_btnHome.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnHome))
   self.comps.E_btnGlobalBack.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnGlobalBack))
   self.comps.E_btnBack.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnBack))
   self.comps.E_btnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnClose))
end

--endRegionAutoBind

-----OnClick_E_btnHome()
function E4_4_Page:OnClick_E_btnHome()
    UIMgr:closeAllExclude({"UIE4_4_Page", "ExampleEnterPage"})
end

-----OnClick_E_btnGlobalBack()
function E4_4_Page:OnClick_E_btnGlobalBack()
    UIMgr:globalBack() 
end

-----OnClick_E_btnBack()
function E4_4_Page:OnClick_E_btnBack()
    UIMgr:back()
end

-----OnClick_E_btnClose()
function E4_4_Page:OnClick_E_btnClose()
    self:closeMe()
end
return E4_4_Page
