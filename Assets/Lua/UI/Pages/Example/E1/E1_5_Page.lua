local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E1_5_Page : UIPageBase
local E1_5_Page = class("E1_5_Page", UIPageBase)
--endregion

--region 页面生命周期

---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E1_5_Page:onOpen(options)
    self:displayUIByDataTable();
end

--region 页面数据辅助

---methods 构造绑定的方法列表， 用来完成UI交互事件响应注册和子模块emit事件响应注册
function E1_5_Page:methods()
    return {
        onChildEvent = self.onChildEvent,
    }
end

function E1_5_Page:onChildEvent(info)
    print("UIE1_5_Page:onChildEvent " .. info)
end

function E1_5_Page:displayUIByDataTable()
    --这里我们演示如何通过对数据的操作来达到显示列表内容
    local data = {}
    --这里我们准备了10个数据
    for i = 1, 100 do
        local tmp = {}
        --这里我们假设第一个数据为 title 标题对象
        tmp.script = "TableViewItem"
        tmp.info = "英雄No. " .. i;
        table.insert(data, tmp)
    end

    self.comps.E_Horizontal_NDGridView.NDGridView.data = data
    self.comps.E_Horizontal_NDGridView.NDGridView:RefreshCells()
end

function E1_5_Page:onCloseClick()
    self:closeMe()
end

--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_5_Page:initAutoBind(gameObject)
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
        E_Horizontal_NDGridView = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDGridView
            NDGridView = self.uiExpansion:GetBindObject(7),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
    self:BindCustomFunction()
end

function E1_5_Page:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

function E1_5_Page:BindCustomFunction()
    self.comps.E_Horizontal_NDGridView.NDGridView = ElementEx(self, self.comps.E_Horizontal_NDGridView.NDGridView, UIExElement.NDGridView)
end

-----OnClick_E_Button()
function E1_5_Page:OnClick_E_Button()
    self:closeMe()
end
return E1_5_Page
