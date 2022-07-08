--regionCustomCreator 项目可自定义的ui page代码生成部分
local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class ExampleEnterPage : UIPageBase
local ExampleEnterPage = class("ExampleEnterPage", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function ExampleEnterPage:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function ExampleEnterPage:onCreated(options)
end
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function ExampleEnterPage:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function ExampleEnterPage:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function ExampleEnterPage:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function ExampleEnterPage:onHide(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function ExampleEnterPage:onClose(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function ExampleEnterPage:onDestroy(options)
end
--endregion
--regionEndCustomCreator



-----OnClick_E_E113Click()
function ExampleEnterPage:OnClick_E_E113Click()
    UIMgr:open("E1_13_Page", {withAnimation = true})
end



-----OnClick_E_E201Click()
function ExampleEnterPage:OnClick_E_E201Click()
    UIMgr:open("E2_1_Page")
end


-----OnClick_E_E202Click()
function ExampleEnterPage:OnClick_E_E202Click()
    UIMgr:open("E2_2_Page")
end


-----OnClick_E_E203Click()
function ExampleEnterPage:OnClick_E_E203Click()
    UIMgr:open("E2_3_Page")
end

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function ExampleEnterPage:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.E_E102Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(0),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(1),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(2),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(3),
    }
    self.E_E104Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(4),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(5),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(6),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(7),
    }
    self.E_E105Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(8),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(9),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(10),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(11),
    }
    self.E_E107Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(12),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(13),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(14),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(15),
    }
    self.E_E112Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(16),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(17),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(18),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(19),
    }
    self.E_E201Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(20),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(21),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(22),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(23),
    }
    self.E_E202Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(24),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(25),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(26),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(27),
    }
    self.E_E203Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(28),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(29),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(30),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(31),
    }
    self.E_E301Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(32),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(33),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(34),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(35),
    }
    self.E_E401Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(36),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(37),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(38),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(39),
    }
    self.E_E402Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(40),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(41),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(42),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(43),
    }
    self.E_E403Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(44),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(45),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(46),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(47),
    }
    self.E_E404Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(48),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(49),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(50),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(51),
    }
    self.E_E501Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(52),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(53),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(54),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(55),
    }
    self.E_E502Click = {
       ---@type GameObject
       GameObject = self.uiExpansion:GetBindObject(56),
       ---@type RectTransform
       RectTransform = self.uiExpansion:GetBindObject(57),
       ---@type Image
       Image = self.uiExpansion:GetBindObject(58),
       ---@type NDButton
       NDButton = self.uiExpansion:GetBindObject(59),
    }
    self:BindCustomModules()
    self:BindCommonFunction()
end


---BindCustomModules绑定静态Module方法
function ExampleEnterPage:BindCustomModules()
end

---BindCustomFunction绑定通用事件方法
function ExampleEnterPage:BindCommonFunction()
   self.E_E102Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E102Click))
   self.E_E104Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E104Click))
   self.E_E105Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E105Click))
   self.E_E107Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E107Click))
   self.E_E112Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E112Click))
   self.E_E201Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E201Click))
   self.E_E202Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E202Click))
   self.E_E203Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E203Click))
   self.E_E301Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E301Click))
   self.E_E401Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E401Click))
   self.E_E402Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E402Click))
   self.E_E403Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E403Click))
   self.E_E404Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E404Click))
   self.E_E501Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E501Click))
   self.E_E502Click.NDButton.onClick:AddListener(handler(self,self.OnClick_E_E502Click))
   self:BindCustomFunction()
end

--endRegionAutoBind

---BindCustomFunction绑定自定义事件方法
function ExampleEnterPage:BindCustomFunction()

end

-----OnClick_E_E301Click()
function ExampleEnterPage:OnClick_E_E301Click()
    UIMgr:open("E3_1_Page",nil, nil)
end

-----OnClick_E_E401Click()
function ExampleEnterPage:OnClick_E_E401Click()
    UIMgr:open("E4_1_Page")
end

-----OnClick_E_E402Click()
function ExampleEnterPage:OnClick_E_E402Click()
    UIMgr:open("E4_2_Page")
end

-----OnClick_E_E403Click()
function ExampleEnterPage:OnClick_E_E403Click()
    local msgQueue = {}
    for i=1, 3 do
        local msg = {}
        msg.title = "消息"..i
        msg.content = ("消息%d的主题内容").format(i)
        table.insert(msgQueue, msg)
    end
    for i, v in ipairs(msgQueue) do
        UIMgr:open("E4_3_Page", v)
    end
end

-----OnClick_E_E404Click()
function ExampleEnterPage:OnClick_E_E404Click()
    UIMgr:open("E4_4_Page")
end

-----OnClick_E_E501Click()
function ExampleEnterPage:OnClick_E_E501Click()
    UIMgr:open("E5_1_Page")
end

-----OnClick_E_E502Click()
function ExampleEnterPage:OnClick_E_E502Click()
    UIMgr:open("E5_2_Page")
end

-----OnClick_E_E102Click()
function ExampleEnterPage:OnClick_E_E102Click()
    UIMgr:open("E1_2_Page",nil, nil)
end

-----OnClick_E_E104Click()
function ExampleEnterPage:OnClick_E_E104Click()
    UIMgr:open("E1_4_Page",nil, nil)
end

-----OnClick_E_E105Click()
function ExampleEnterPage:OnClick_E_E105Click()
    UIMgr:open("E1_5_Page",nil, nil)
end

-----OnClick_E_E107Click()
function ExampleEnterPage:OnClick_E_E107Click()
    UIMgr:open("E1_7_Page",nil, nil)
end

-----OnClick_E_E112Click()
function ExampleEnterPage:OnClick_E_E112Click()
    UIMgr:open("E1_12_Page", {withAnimation = true})
end

return ExampleEnterPage
