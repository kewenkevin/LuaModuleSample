//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

using System;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.WebRequest;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// Web 请求组件。
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("ResourceMgr/Web Request")]
    public sealed class WebRequestComponent : GameFrameworkComponent
    {
        private const int DefaultPriority = 0;

        private IWebRequestManager m_WebRequestManager = null;
        // private EventComponent m_EventComponent = null;

        [SerializeField]
        private Transform m_InstanceRoot = null;

        [SerializeField]
        private string m_WebRequestAgentHelperTypeName = "UnityGameFramework.Runtime.UnityWebRequestAgentHelper";

        [SerializeField]
        private WebRequestAgentHelperBase m_CustomWebRequestAgentHelper = null;

        [SerializeField]
        private int m_WebRequestAgentHelperCount = 1;

        [SerializeField]
        private float m_Timeout = 30f;

        /// <summary>
        /// 获取 Web 请求代理总数量。
        /// </summary>
        public int TotalAgentCount
        {
            get
            {
                return m_WebRequestManager.TotalAgentCount;
            }
        }

        /// <summary>
        /// 获取可用 Web 请求代理数量。
        /// </summary>
        public int FreeAgentCount
        {
            get
            {
                return m_WebRequestManager.FreeAgentCount;
            }
        }

        /// <summary>
        /// 获取工作中 Web 请求代理数量。
        /// </summary>
        public int WorkingAgentCount
        {
            get
            {
                return m_WebRequestManager.WorkingAgentCount;
            }
        }

        /// <summary>
        /// 获取等待 Web 请求数量。
        /// </summary>
        public int WaitingTaskCount
        {
            get
            {
                return m_WebRequestManager.WaitingTaskCount;
            }
        }

        /// <summary>
        /// 获取或设置 Web 请求超时时长，以秒为单位。
        /// </summary>
        public float Timeout
        {
            get
            {
                return m_WebRequestManager.Timeout;
            }
            set
            {
                m_WebRequestManager.Timeout = m_Timeout = value;
            }
        }

        /// <summary>
        /// 游戏框架组件初始化。
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            m_WebRequestManager = GameFrameworkEntry.GetModule<IWebRequestManager>();
            if (m_WebRequestManager == null)
            {
                Log.Fatal("Web request manager is invalid.");
                return;
            }

            m_WebRequestManager.Timeout = m_Timeout;
            m_WebRequestManager.WebRequestStart += OnWebRequestStart;
            m_WebRequestManager.WebRequestSuccess += OnWebRequestSuccess;
            m_WebRequestManager.WebRequestFailure += OnWebRequestFailure;
        }

        private void Start()
        {

            if (m_InstanceRoot == null)
            {
                m_InstanceRoot = new GameObject("Web Request Agent Instances").transform;
                m_InstanceRoot.SetParent(gameObject.transform);
                m_InstanceRoot.localScale = Vector3.one;
            }

            for (int i = 0; i < m_WebRequestAgentHelperCount; i++)
            {
                AddWebRequestAgentHelper(i);
            }
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri)
        {
            return AddWebRequest(webRequestUri, null, null, DefaultPriority, null);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="postData">要发送的数据流。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, byte[] postData)
        {
            return AddWebRequest(webRequestUri, postData, null, DefaultPriority, null);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="wwwForm">WWW 表单。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, WWWForm wwwForm)
        {
            return AddWebRequest(webRequestUri, null, wwwForm, DefaultPriority, null);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="priority">Web 请求任务的优先级。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, int priority)
        {
            return AddWebRequest(webRequestUri, null, null, priority, null);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="postData">要发送的数据流。</param>
        /// <param name="priority">Web 请求任务的优先级。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, byte[] postData, int priority)
        {
            return AddWebRequest(webRequestUri, postData, null, priority, null);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="wwwForm">WWW 表单。</param>
        /// <param name="priority">Web 请求任务的优先级。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, WWWForm wwwForm, int priority)
        {
            return AddWebRequest(webRequestUri, null, wwwForm, priority, null);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, object userData)
        {
            return AddWebRequest(webRequestUri, null, null, DefaultPriority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="postData">要发送的数据流。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, byte[] postData, object userData)
        {
            return AddWebRequest(webRequestUri, postData, null, DefaultPriority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="wwwForm">WWW 表单。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, WWWForm wwwForm, object userData)
        {
            return AddWebRequest(webRequestUri, null, wwwForm, DefaultPriority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="priority">Web 请求任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, int priority, object userData)
        {
            return AddWebRequest(webRequestUri, null, null, priority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="postData">要发送的数据流。</param>
        /// <param name="priority">Web 请求任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, byte[] postData, int priority, object userData)
        {
            return AddWebRequest(webRequestUri, postData, null, priority, userData);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="wwwForm">WWW 表单。</param>
        /// <param name="priority">Web 请求任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        public int AddWebRequest(string webRequestUri, WWWForm wwwForm, int priority, object userData)
        {
            return AddWebRequest(webRequestUri, null, wwwForm, priority, userData);
        }

        /// <summary>
        /// 移除 Web 请求任务。
        /// </summary>
        /// <param name="serialId">要移除 Web 请求任务的序列编号。</param>
        /// <returns>是否移除 Web 请求任务成功。</returns>
        public bool RemoveWebRequest(int serialId)
        {
            return m_WebRequestManager.RemoveWebRequest(serialId);
        }

        /// <summary>
        /// 移除所有 Web 请求任务。
        /// </summary>
        public void RemoveAllWebRequests()
        {
            m_WebRequestManager.RemoveAllWebRequests();
        }

        /// <summary>
        /// 获取所有 Web 请求任务的信息。
        /// </summary>
        /// <returns>所有 Web 请求任务的信息。</returns>
        public TaskInfo[] GetAllWebRequestInfos()
        {
            return m_WebRequestManager.GetAllWebRequestInfos();
        }

        /// <summary>
        /// 增加 Web 请求代理辅助器。
        /// </summary>
        /// <param name="index">Web 请求代理辅助器索引。</param>
        private void AddWebRequestAgentHelper(int index)
        {
            WebRequestAgentHelperBase webRequestAgentHelper = Helper.CreateHelper(m_WebRequestAgentHelperTypeName, m_CustomWebRequestAgentHelper, index);
            if (webRequestAgentHelper == null)
            {
                Log.Error("Can not create web request agent helper.");
                return;
            }

            webRequestAgentHelper.name = Utility.Text.Format("Web Request Agent Helper - {0}", index.ToString());
            Transform transform = webRequestAgentHelper.transform;
            transform.SetParent(m_InstanceRoot);
            transform.localScale = Vector3.one;

            m_WebRequestManager.AddWebRequestAgentHelper(webRequestAgentHelper);
        }

        /// <summary>
        /// 增加 Web 请求任务。
        /// </summary>
        /// <param name="webRequestUri">Web 请求地址。</param>
        /// <param name="postData">要发送的数据流。</param>
        /// <param name="wwwForm">WWW 表单。</param>
        /// <param name="priority">Web 请求任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增 Web 请求任务的序列编号。</returns>
        private int AddWebRequest(string webRequestUri, byte[] postData, WWWForm wwwForm, int priority, object userData)
        {
            return m_WebRequestManager.AddWebRequest(webRequestUri, postData, priority, WWWFormInfo.Create(wwwForm, userData));
        }

        private void OnWebRequestStart(object sender, Framework.WebRequest.WebRequestStartEventArgs e)
        {
            WebRequestStart?.Invoke(this,WebRequestStartEventArgs.Create(e));
        }

        private void OnWebRequestSuccess(object sender, Framework.WebRequest.WebRequestSuccessEventArgs e)
        {
            WebRequestSuccess?.Invoke(this,WebRequestSuccessEventArgs.Create(e));
        }

        private void OnWebRequestFailure(object sender, Framework.WebRequest.WebRequestFailureEventArgs e)
        {
            Log.Warning("Web request failure, web request serial id '{0}', web request uri '{1}', error message '{2}'.", e.SerialId.ToString(), e.WebRequestUri, e.ErrorMessage);
            WebRequestFailure?.Invoke(this,WebRequestFailureEventArgs.Create(e));
        }
        
        
        /// <summary>
        /// Web 请求开始事件。
        /// </summary>
        public event EventHandler<WebRequestStartEventArgs> WebRequestStart;

        /// <summary>
        /// Web 请求成功事件。
        /// </summary>
        public event EventHandler<WebRequestSuccessEventArgs> WebRequestSuccess;

        /// <summary>
        /// Web 请求失败事件。
        /// </summary>
        public event EventHandler<WebRequestFailureEventArgs> WebRequestFailure;
    }
}
