local UIModuleBase = require("LuaModules.UILib.Base.ModuleBase")

---@class E3_1_Module : UIModuleBase
local E3_1_Module = class("E3_1_Module", UIModuleBase)

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E3_1_Module:observerDataStore()
   return {
        alliance = {
            allianceInfo =
            {
                id = handler(self, self.initData),
            },
            allianceList1_insert = handler(self, self.allianceListInsert),
            allianceList1_remove = handler(self, self.allianceListRemove),
            allianceList1 = handler(self, self.allianceListChange),
        }
    }
end
--endregion

function E3_1_Module:allianceListInsert(array, addItem)
    print("allianceListInsert" .. addItem.name)
end

function E3_1_Module:allianceListRemove(array, removeIndex, removeItem)
    print("allianceListRemove " .. removeItem.name .. " removeIndex = " .. removeIndex)
end

function E3_1_Module:allianceListChange(oldArray, newArray)
    print("allianceListChange " .. " oldArray = " .. oldArray[1].name .. " newArray = " .. newArray[1].name)
end

--region 页面数据辅助
---initData 初始化数据
---@param oldVal number
---@param newVal number
function E3_1_Module:initData(oldVal, newVal)
    print("initData oldValue=" .. oldVal .. " newValue=" .. newVal)
    local info = Store.alliance:getAllianceInfoById(newVal)

    if nil == info then
        return
    end

    self.comps.E_txtName.NDText.text = info.name
end

---@see UIPageBase#listen
---listen 模块数仓数据变化发送时间，这里注册响应接口
function E3_1_Module:listen()
    return{
        alliance = {
            onAllianceLevelUp = handler(self, self.refreshLevel)
        }
    }
end

---refreshLevel
---@param id number
function E3_1_Module:refreshLevel(id)
    local data = Store.alliance:getAllianceInfoById(id)
    self.comps.E_txtLevel.NDText.text = data.level
end
--endregion

--regionAutoBind 导出控件代码自动生成，请勿手动修改
function E3_1_Module:initAutoBind(gameObject)
    ---@type UIExpansion
    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))
    self.comps = {
        E_txtName = {
            ---@type GameObject
            GameObject = self.uiExpansion:GetBindObject(0),
            ---@type RectTransform
            RectTransform = self.uiExpansion:GetBindObject(1),
            ---@type NDText
            NDText = self.uiExpansion:GetBindObject(2),
        },
        E_txtLevel = {
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

function E3_1_Module:BindCommonFunction()
end

--endRegionAutoBind
return E3_1_Module
