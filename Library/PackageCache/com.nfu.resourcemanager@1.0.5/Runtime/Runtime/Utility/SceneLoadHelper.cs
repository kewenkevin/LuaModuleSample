

using ND.Managers.ResourceMgr.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneUtility = ND.Managers.ResourceMgr.Runtime.SceneUtility;

namespace ND.ResourceMgr.RunTime
{
    public static class SceneLoadHelper 
    {
        /// <summary>
        /// 通过相对路径加载资源。主要用于适配编辑模式和非编辑器模式的加载。
        /// </summary>
        /// <param name="sceneName">场景的完整相对路径</param>
        /// <param name="loadSceneMode">场景加载模式</param>
        /// <returns></returns>
        public static AsyncOperation LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode)
        {
            if (ResourceEntry.Base.EditorResourceMode)
            {
#if UNITY_EDITOR
                return UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(sceneName,
                    new LoadSceneParameters() { loadSceneMode = loadSceneMode });
#else
                    return null;
#endif
            }
            else
            {
                return SceneManager.LoadSceneAsync(SceneUtility.GetSceneName(sceneName), loadSceneMode);
            }
        }

        public static AsyncOperation UnLoadSceneAsync(string sceneName)
        {
            if (ResourceEntry.Base.EditorResourceMode)
            {
#if UNITY_EDITOR
                return UnityEditor.SceneManagement.EditorSceneManager.UnloadSceneAsync(sceneName);
#else
                return null;
#endif
            }
            else
            {
                return SceneManager.UnloadSceneAsync(SceneUtility.GetSceneName(sceneName));
            }
        
        }
    }
}

