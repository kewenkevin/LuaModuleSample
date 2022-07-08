//-----------------------------------------------------------------------
// Created By 甘道夫
// contact E-mail: wwei@ND.com
// Date: 2020-09-11
// 本文件中为资源加载器的对象池
//-----------------------------------------------------------------------
using System.Collections.Generic;

namespace ND.Managers.ResourceMgr.Runtime
{
    /// <summary>
    /// 资源加载器对象池，随资源系统一起创建及清空
    /// </summary>
    public class ResLoaderPool
    {
        /// <summary>
        /// 池中的loader栈
        /// </summary>
        private readonly Stack<ResLoader> m_Loaders = new Stack<ResLoader>();

        /// <summary>
        /// 池中分配一个loader，如果没有cache则直接创建
        /// </summary>
        /// <returns></returns>
        public ResLoader Alloc()
        {
            if (m_Loaders.Count > 0)
            {
                return m_Loaders.Pop();
            }
            else
            {
                return new ResLoader();
            }
        }
        
        /// <summary>
        /// 让一个loader返回池中
        /// </summary>
        /// <param name="resLoader">要回池的loader</param>
        public void Recycle(ResLoader resLoader)
        {
            m_Loaders.Push(resLoader);
            resLoader.OnCacheReset();
        }

        /// <summary>
        /// 清空池，资源系统卸载后调用
        /// </summary>
        public void Clear()
        {
            while (m_Loaders.Count > 0)
            {
                m_Loaders.Pop().Dispose();
            }
        }
    }
}