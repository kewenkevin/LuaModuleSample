using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 准备资源，生成图集和Lua
    /// </summary>
    public interface IResourcePreprocessBuild
    {
        /// <summary>
        /// 初始化，可以存在一个或多个实例，该方法在<see cref="PreprocessBuild"/>被调用前只执行一次
        /// </summary>
        void PreprocessBuildInitialize();
        
        /// <summary>
        /// 生成资源包之前准备资源
        /// </summary>
        void PreprocessBuild();

        /// <summary>
        /// 释放资源，与 <see cref="BeginPreprocessBuild"/> 对应
        /// </summary>
        void PreprocessBuildCleanup();
    }
}
