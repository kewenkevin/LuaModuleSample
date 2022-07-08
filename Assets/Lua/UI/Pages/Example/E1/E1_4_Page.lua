local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E1_4_Page : UIPageBase
local E1_4_Page = class("E1_4_Page", UIPageBase)

--region 页面生命周期
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E1_4_Page:onOpen(options)
    print("onOpen")
    self:displayUI();
end
--endregion

---methods 构造绑定的方法列表， 用来完成UI交互事件响应注册和子模块emit事件响应注册
function E1_4_Page:methods()
    return {
        onChildEvent = self.onChildEvent,
    }
end

function E1_4_Page:onChildEvent(info)
    print("E1_4_Page:onChildEvent " .. info)
end

--region 页面数据辅助

--界面显示
function E1_4_Page:displayUI()
    --这里我们演示如何通过对数据的操作来达到显示列表内容
    self.data = {}
    --这里我们准备了10个数据
    for i = 1, 1000 do
        local tmp = {}
        --这里我们假设第一个数据为 title 标题对象
        if i == 1 or i == 3 or i == 5 then
            tmp.identify = "TableViewTitle"
            tmp.script   = "TableViewTitle"
            tmp.info = "英雄标题"
        else
            --其余的均为HeroInfo对象
            tmp.identify = "TableViewItem"
            tmp.script   = "TableViewItem"
            tmp.info = "Hero No. " .. i - 1;
        end
        
        table.insert(self.data, tmp)
    end
    
    self.comps.E_VerticalYTableView.NDTableView.data = self.data
    self.comps.E_VerticalYTableView.NDTableView:RefreshCells()
end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E1_4_Page:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_CloseButton = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(2),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(3),
        },
        E_VerticalYTableView = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDTableView
            NDTableView = self.uiExpansion:GetBindObject(7),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
    self:BindCustomFunction()
end

function E1_4_Page:BindCommonFunction()
   self.comps.E_CloseButton.NDButton.onClick:AddListener(handler(self,self.OnClick_E_CloseButton))

    --print("BindCommonFunction")
    
end

--endRegionAutoBind

function E1_4_Page:BindCustomFunction()
    self.comps.E_VerticalYTableView.NDTableView = ElementEx(self, self.comps.E_VerticalYTableView.NDTableView, UIExElement.NDTableView)
end

-----OnClick_E_CloseButton()
function E1_4_Page:OnClick_E_CloseButton()
    self:closeMe()
end
return E1_4_Page
