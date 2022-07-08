//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------

namespace ND.Managers.ResourceMgr.Framework.Resource
{
    /// <summary>
    /// 加载资源代理辅助器异步读取资源二进制流完成事件。
    /// </summary>
    public sealed class LoadResourceAgentHelperReadBytesCompleteEventArgs : GameFrameworkEventArgs
    {
        private byte[] m_Bytes;

        private bool m_rapidly;

        /// <summary>
        /// 初始化加载资源代理辅助器异步读取资源二进制流完成事件的新实例。
        /// </summary>
        public LoadResourceAgentHelperReadBytesCompleteEventArgs()
        {
            m_Bytes = null;
            m_rapidly = false;
        }

        /// <summary>
        /// 创建加载资源代理辅助器异步读取资源二进制流完成事件。
        /// </summary>
        /// <param name="bytes">资源的二进制流。</param>
        /// <returns>创建的加载资源代理辅助器异步读取资源二进制流完成事件。</returns>
        public static LoadResourceAgentHelperReadBytesCompleteEventArgs Create(byte[] bytes, bool rapidly)
        {
            LoadResourceAgentHelperReadBytesCompleteEventArgs loadResourceAgentHelperReadBytesCompleteEventArgs = ReferencePool.Acquire<LoadResourceAgentHelperReadBytesCompleteEventArgs>();
            loadResourceAgentHelperReadBytesCompleteEventArgs.m_Bytes = bytes;
            loadResourceAgentHelperReadBytesCompleteEventArgs.m_rapidly = rapidly;
            return loadResourceAgentHelperReadBytesCompleteEventArgs;
        }

        /// <summary>
        /// 清理加载资源代理辅助器异步读取资源二进制流完成事件。
        /// </summary>
        public override void Clear()
        {
            m_Bytes = null;
        }

        /// <summary>
        /// 获取资源的二进制流。
        /// </summary>
        /// <returns>资源的二进制流。</returns>
        public byte[] GetBytes()
        {
            return m_Bytes;
        }


        public bool IsRapidly
        {
            get
            {
                return m_rapidly;
            }
        }
    }
}
