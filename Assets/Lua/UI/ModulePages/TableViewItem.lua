--regionCustomCreator 项目可自定义的ui module代码生成部分
--自定义module模板代码
--regionEndCustomCreator

local UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")
local TableViewItem = class("TableViewItem", UIModuleBase)


--regionAutoBind 导出控件代码自动生成，请勿手动修改
function TableViewItem:initAutoBind(gameObject)
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
        E_Text = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(6),
        },
    }
    self.modules = {
    }
    self:BindCommonFunction()
end

function TableViewItem:BindCommonFunction()
   self.comps.E_Button.NDButton.onClick:AddListener(handler(self,self.OnClick_E_Button))
end

--endRegionAutoBind

-----OnClick_E_Button()
function TableViewItem:OnClick_E_Button()
    print(self.data.info)
    self:emit("onChildEvent", self.data.info)
end

function TableViewItem:Refresh(data)
    self.data = data
    self.comps.E_Text.NDText.text = data.info
end

return TableViewItem
