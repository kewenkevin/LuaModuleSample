local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---测试用常量id
local TEST_ALLIANCE_ID = 300001

---@class E3_1_Page : UIPageBase
local E3_1_Page = class("E3_1_Page", UIPageBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E3_1_Page:observerDataStore()
    return {
    }
end
--endregion

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E3_1_Page:onCreated(options)
    
end
---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E3_1_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E3_1_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E3_1_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E3_1_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E3_1_Page:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function E3_1_Page:onDestroy(options)
end
--endregion


--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E3_1_Page:initAutoBind(gameObject)
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
        E_BtnTransData = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(4),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(5),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(6),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(7),
        },
        E_BtnChangeStore = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(8),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(9),
            ---@type NDImage
            NDImage = self.uiExpansion:GetBindObject(10),
            ---@type NDButton
            NDButton = self.uiExpansion:GetBindObject(11),
        },
        E_E3_1_Module = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(12),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(13),
            ---@type UIExpansion
            UIExpansion = self.uiExpansion:GetBindObject(14),
        },
    }
    
--[[    self.modules = {
          Module_E_E3_1_Module = require("UI.ModulePages.E3_1_Module").new(self.comps.E_E3_1_Module.GameObject),
    }]]
    
    --self:BindModules("E3_1_Module", "E3_1_Module", self.comps.E_E3_1_Module.GameObject)
    self:BindCommonFunction()
    self:BindCustomModules()
end

function E3_1_Page:BindCommonFunction()
   self.comps.E_BtnClose.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnClose))
   self.comps.E_BtnTransData.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnTransData))
   self.comps.E_BtnChangeStore.NDButton.onClick:AddListener(handler(self,self.OnClick_E_BtnChangeStore))
end

--endRegionAutoBind

function E3_1_Page:BindCustomModules()
    ---@type UIE1_7_ModuleA
    self.M_E3_1_Module = self:registerModule("M_E3_1_Module", "E3_1_Module", self.comps.E_E3_1_Module.GameObject)
end

-----OnClick_E_BtnClose()
function E3_1_Page:OnClick_E_BtnClose()
    self:closeMe()
end

-----OnClick_E_BtnTransData() 透传数据到module
function E3_1_Page:OnClick_E_BtnTransData()
    Store.alliance._data.allianceInfo.id = TEST_ALLIANCE_ID
    
    local allianceInfo = {}
    allianceInfo.name = "alliance100"
    allianceInfo.level = 100
    Store.alliance._data.allianceList1.insert(allianceInfo)
    Store.alliance._data.allianceList1.remove(1)

    Store.alliance._data.allianceList1 = 
    {
        {
            name = "alliance1000",
            level = 12000,
        },
        {
            name = "alliance2000",
            level = 13000,
        },
    }
end

-----OnClick_E_BtnChangeStore() 修改数仓
function E3_1_Page:OnClick_E_BtnChangeStore()
    Api.alliance:reqAllianceLevelUp(TEST_ALLIANCE_ID, 1)
end
return E3_1_Page
