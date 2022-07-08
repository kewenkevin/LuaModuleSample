local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E4_2_Page : UIPageBase
local E4_2_Page = class("E4_2_Page", UIPageBase)

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E4_2_Page:onCreated(options)
end
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E4_2_Page:onOpen(options)
    self.process = 0
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E4_2_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E4_2_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E4_2_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E4_2_Page:onUpdate(options)
    if self.process  < 1 then
        self.process = self.process + 0.001
        self:ProcessRefresh()
    else
        self:closeMe()
    end
end
--endregion

function E4_2_Page:ProcessRefresh()
    self.comps.E_Slider.NDSlider.value = self.process
end


--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E4_2_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_btnClose = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(2),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(3),
        },
        E_Slider = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDSlider
            NDSlider = self.uiExpansion:GetBindObject(6),
        },
        E_btnTryOpen = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(7),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(8),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(9),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(10),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function E4_2_Page:BindCommonFunction()
   self.comps.E_btnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnClose))
   self.comps.E_btnTryOpen.NDButton.onClick:AddListener(handler(self,self.OnClick_E_btnTryOpen))
end

--endRegionAutoBind

-----OnClick_E_btnClose()
function E4_2_Page:OnClick_E_btnClose()
    self:closeMe()
end

-----OnClick_E_btnTryOpen()
function E4_2_Page:OnClick_E_btnTryOpen()
    UIMgr:open("E4_2_Page")
end
return E4_2_Page
