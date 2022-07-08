using System;
using System.Collections.Generic;

namespace ND.Managers.ResourceMgr.Editor.ResourceTools
{
    /// <summary>
    /// 资源分组
    /// </summary>
    [Serializable]
    public class ResourceGroup
    {

        public List<ResourcePreprocessBuild> PreprocessBuilds { get; set; } = new List<ResourcePreprocessBuild>();

        public List<ResourceRule> Rules { get; set; } = new List<ResourceRule>();


        public ResourceRule FindRule(string assetPath)
        {
            for(int i = Rules.Count - 1; i >= 0; i--)
            {
                var rule = Rules[i];
                //if (EditorUtilityx.IncludeExclude(assetPath, rule.include, rule.exclude))
                if(rule.IsMatch(assetPath))
                    return rule;
            }
            return null;
        }

    }
}