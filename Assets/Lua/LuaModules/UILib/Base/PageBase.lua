--[[
UI生命周期可参见：
onPreData  //只调用一次
onCreated  //只调用一次
autoBind   //UI按钮绑定
onBinding  //在数据绑定完成后触发，当Module存在复用时，每次会重新触发
onOpen
onShow     //入场动画播放完触发
onFinished //入场动画播放结束回调
onHide+
onunBinding//在数据解绑的时候触发
onDestroy //删除界面对象回资源池中
]]

---@type UIEnums UI所有枚举
local UIEnums = require("LuaModules.UILib.Enums.UIEnums")

---@class UIPageBase : UIBase
---@field private mappedDataStore DataStore
local page = class("pageBase",require("LuaModules.UILib.Base.UIBase"))
local tag = "pageBase"


---created 管理器调用，界面不要复写
---@param obj UnityEngine.GameObject
---@param config UIConfigPage
---@param binderData table
---@param uiMgr UIManager
function page:created(obj,config, --[[binderData,]] uiMgr,options)
    self.gameObject = obj
    self.gameObject.name = config.pageName
    self._uiMgr = uiMgr
    ---@type UIConfigPage
    self.pageConfig = config
--[[
    if self.data then
        self.data = self:data()
    end

    if binderData~=nil and type(binderData) == "function" then
        self.data = self:mergeData(self.data,binderData())
    end

    if self.methods then
        self.methods = self:methods()
    end
    if self.observerUIData then
        self.observerUIData = self:observerUIData()
    end
]]
    if self.methods then
        self.methods = self:methods()
    end
    
    if self.onPreData then
        self:onPreData()
    end
    page.super.created(self--[[,self.uiExpansion]])

    self:onCreated(obj,config,--[[binderData,]]options)
end

---onCreated 界面复写 界面复写 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
---@param obj UnityEngine.GameObject
---@param config UIConfigPage
---@param binderData table
function page:onCreated(obj,config,--[[binderData,]]options)
end

---open 管理器调用，界面不要复写
---@param options table
---@param uiMgr UIManager
function page:open(options, uiMgr)
    self._uiMgr = uiMgr
    self.mappedDataStore = self:getDataSore()
    if self.mappedDataStore then
        if self.observerDataStore then
            self.observerDataStore = self:observerDataStore()
            self.mappedDataStore:mapAllStates(self.observerDataStore)
        end
        if self.listen then
            self.listen = self:listen()
            self.mappedDataStore:listenAllCall(self.listen)
        end
    end
    self:onOpen(options)
end


function page:getDataSore()
    if self._uiMgr then
        return self._uiMgr:getDataSore()
    else
        return nil
    end
end

---onOpen  在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
---@param options table
function page:onOpen(options)
end

---close 管理器调用，界面不要复写
---@param options table
function page:close(options)
    if options and options.withAnimation == true then
        self:playAnimation("close", function()
            self:doClose(options)
        end)
    else
        self:doClose(options)
    end
end

---doclose 关闭界面的清理工作
---@param options table
function page:doClose(options)
    if self.mappedDataStore then
        if self.observerDataStore then
            self.mappedDataStore:unmapAllStates(self.observerDataStore)
        end
        if self.listen then
            self.mappedDataStore:unListenAllCall(self.listen)
        end
    end

    self:onClose(options)
end
---onClose 界面复写 在Page关闭时触发，一个实例只会触发一次
---@param options table
function page:onClose(options)
end

---afterOpen 在open执行后触发，处理打开显示前的动作
---@param withAnimation boolean
function page:afterOpen(withAnimation, onFinished, options)
    if withAnimation then
        self:playAnimation("open", function ()
            self:onShow()
            if onFinished then
                onFinished()
            end
        end)
    else
        self:onShow(options)
        if onFinished then
            onFinished()
        end
    end
end

---show 管理器调用，界面不要复写
function page:show(options)
    self:onShow(options)
end

---onShow 界面复写 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function page:onShow(options)
end

---hide 管理器调用，界面不要复写
function page:hide(options)
    self:onHide(options)
end

---onHide 界面复写 在Page隐藏时触发，每次切到Hide状态都会触发
function page:onHide(options)
end

---destroy 管理器调用，界面不要复写
---@param options table
function page:destroy(options)
    self:unBind()
    self:onDestroy(options)
end

---onDestroy 界面复写 在Page实例销毁时触发
---@param options table
function page:onDestroy(options)
end

---closeMe 关闭自身页面
---如果是栈类型界面，只有当自身页面是栈顶页面时生效
function page:closeMe(options)
    if self._uiMgr == nil then
        return
    end
    if self.pageConfig.uiType == UIEnums.PageType.stack then
        if self._uiMgr:isStackTop(self.pageConfig.pageName) then
            self._uiMgr:back(options)
        else
            error("this page in stack, isStackTop check failed: "..self.pageConfig.pageName)
        end
    else
        self._uiMgr:close(self.pageConfig.pageName, options)
    end
end

return page