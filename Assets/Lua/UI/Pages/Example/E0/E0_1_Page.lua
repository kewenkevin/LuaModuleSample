--regionCustomCreator 项目可自定义的ui page代码生成部分
local UIPageBase = require("LuaModules.UILib.Base.PageBase")

---@class E0_1_Page : UIPageBase
local E0_1_Page = class("E0_1_Page", UIPageBase)

---methods 构造绑定的方法列表， 用来完成子模块emit事件响应注册
function E0_1_Page:methods()

end

---Listen 监控模块数仓变化，这里注册事件接口
function E0_1_Page:listen()
	return{
	}
end

---@see UIPageBase#observerDataStore
---observerDataStore 监控模块数仓数据变化，这里注册响应接口
function E0_1_Page:observerDataStore()
    return {
    }
end
--endregion

---@see UIPageBase#onPreData
---onPreData 在Page创建实例时触发，一个实例只会触发一次，这时还未执行onPreData
function E0_1_Page:onPreData()
    
end

--region 页面生命周期
---@see UIPageBase#onCreated
---onCreated 在Page创建实例时触发，一个实例只会触发一次，这时还未进行数据绑定
function E0_1_Page:onCreated(options)
end

---@see UIPageBase#onOpen
---onOpen 在Page创建实例后触发，一个实例只会触发一次，若需要onOpen前做打开条件检查，可以实现toCheck接口
function E0_1_Page:onOpen(options)
end

---@see UIPageBase#onClose
---onClose 在Page关闭时触发，一个实例只会触发一次
function E0_1_Page:onClose(options)
end

---@see UIPageBase#onShow
---onShow 在Page显示时触发，除第一次显示外，每次从Hide切到Show状态都会触发
function E0_1_Page:onShow(options)
end

---@see UIPageBase#onHide
---onHide 在Page隐藏时触发，每次切到Hide状态都会触发
function E0_1_Page:onHide(options)
end

---onUpdate 逐帧触发，可选的，不实现便不会调用
function E0_1_Page:onUpdate(options)
end

---@see UIPageBase#onDestroy
---onDestroy 在Page实例销毁时触发
function E0_1_Page:onDestroy(options)
end
--endregion

--regionEndCustomCreator


---BindCustomFunction绑定自定义事件方法
function E0_1_Page:BindCustomFunction()

end

return E0_1_Page
