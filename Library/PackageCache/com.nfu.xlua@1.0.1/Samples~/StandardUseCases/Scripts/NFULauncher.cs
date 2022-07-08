using UnityEngine;
using System.Collections;
using XLua;

namespace ND.Framework.XLua.Example
{

    /// <summary>
    /// NFU的默认启动流程，项目研发中不允许直接修改，可以在GameSetup中继承NFULauncher进行接口复写
    /// </summary>
    public class NFULauncher : MonoBehaviour
    {
        #region public member
        public string luaMainFunc = "main";
        /// <summary>
        /// 从ab加载lua
        /// </summary>
        public bool luaAB;
        #endregion

        #region protected member
        protected LuaManager m_luaManager;
        #endregion

        #region unity methods
        void Awake()
        {
            NFUAwake();
        }
        IEnumerator Start()
        {
            yield return NFUStart();
        }

        void Update()
        {
            NFUUpdate();
        }
        void OnDestroy()
        {
            NFUDestroy();
        }

        private void OnApplicationQuit()
        {
            NFUOnApplicationQuit();
        }

        #endregion

        #region for product custom
        /// <summary>
        /// 需要在Scene中的ResourceUpdate上指定OnUpdateSuccess处理函数为OnUpdateComplete
        /// </summary>
        public virtual void OnUpdateComplete()
        {
            Debug.Log("模拟热更结束后，LuaManager初始化操作");
            m_luaManager.abMode = luaAB;
            m_luaManager.Initialize();
            m_luaManager.Start(luaMainFunc);
            
            Debug.Log("模拟向Lua注册Unity Tick");
            //gameObject.AddComponent<LuaLooper>().luaState=LuaManager.mainState;
            
            
            //PlayerPrefs.SetString("version", $"1.0.{ResourceEntry.Resource.InternalResourceVersion.ToString()}");
        }
        
        public virtual void NFUAwake()
        {
            m_luaManager = LuaManager.GetInstance();
#if !UNITY_EDITOR
            luaAB = true;
#endif
        }
        public virtual IEnumerator NFUStart()
        {
            yield return new WaitForEndOfFrame();
            //NFUResource.Initialize();
            //TODO: 用于设置变体

            //NFUResource.HotUpdateStart();
            
            // yield return new WaitForEndOfFrame();
            //测试临时开启，项目方不要模仿
            OnUpdateComplete();
        }
        public virtual void NFUUpdate()
        {
            if (m_luaManager != null)
            {
                m_luaManager.Update();
            }
        }
        public virtual void NFUDestroy()
        {
            
        }

        public virtual void NFUOnApplicationQuit()
        {
            m_luaManager.Dispose();
        }
        #endregion
    }
}