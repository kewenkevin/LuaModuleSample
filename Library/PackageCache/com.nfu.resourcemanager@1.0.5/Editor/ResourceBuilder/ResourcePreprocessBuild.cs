using System;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{

    /// <summary>
    /// 预生成资源配置
    /// </summary>
    public class ResourcePreprocessBuild
    {
        [SerializeField]
        public string preprocessBuildTypeName;

        [SerializeField]
        public string preprocessBuildData;

        private IResourcePreprocessBuild preprocessBuild;


        public IResourcePreprocessBuild PreprocessBuild { get => preprocessBuild; set => preprocessBuild = value; }

        public void OnAfterDeserialize()
        {
            preprocessBuild = null;
            if (!string.IsNullOrEmpty(preprocessBuildTypeName))
            {
                System.Type  type= System.Type.GetType(preprocessBuildTypeName);
                if (type != null)
                {
                     preprocessBuild= Activator.CreateInstance(type) as IResourcePreprocessBuild;
                }
                if (preprocessBuild != null)
                {
                    if (!string.IsNullOrEmpty(preprocessBuildData))
                    {
                        JsonUtility.FromJsonOverwrite(preprocessBuildData, preprocessBuild);
                    }
                }
            }
        }

        public void OnBeforeSerialize()
        {
            preprocessBuildTypeName = null;
            if (preprocessBuild != null)
            {
                preprocessBuildTypeName = preprocessBuild.GetType().AssemblyQualifiedName;
                preprocessBuildData = JsonUtility.ToJson(preprocessBuild);
            }
        }

    }
}