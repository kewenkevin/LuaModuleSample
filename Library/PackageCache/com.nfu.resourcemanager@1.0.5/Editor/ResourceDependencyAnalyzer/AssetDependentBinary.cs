using System.Collections.Generic;

namespace ND.Managers.ResourceMgr.Editor.ResourceDependencyAnalyzer
{
    
    [System.Serializable]
    public class StringInt
    {
        public string key;
        public int value;

        public static bool TryGetValue(List<StringInt> list, string str, out int index)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == str)
                {
                    index = list[i].value;
                    return true;
                }
            }
            index = 0;
            return false;
        }

        public static int GetValue(List<StringInt> list, string str)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == str)
                {
                    return list[i].value;
                }
            }
            return 0;
        }

        public static StringInt Create(string key, int index)
        {
            StringInt stringInt = new StringInt();
            stringInt.key = key;
            stringInt.value = index;
            return stringInt;
        }
    }
    [System.Serializable]
    public class StringListInt
    {
        public string key;
        public List<int> value;

        public static bool TryGetValue(List<StringListInt> list, string str, out List<int> value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == str)
                {
                    value = list[i].value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        public static StringListInt Create(string key, List<int> index)
        {
            StringListInt stringInt = new StringListInt();
            stringInt.key = key;
            stringInt.value = index;
            return stringInt;
        }
    }

    [System.Serializable]
    public class StringListString
    {
        public string key;
        public List<string> value;

        public static bool TryGetValue(List<StringListString> list, string str, out List<string> value)
        {
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].key == str)
                {
                    value = list[i].value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        public static StringListString Create(string key, List<string> index)
        {
            StringListString stringInt = new StringListString();
            stringInt.key = key;
            stringInt.value = index;
            return stringInt;
        }
    }

    [System.Serializable]
    public class AssetDependenciesTreeBinary
    {
        /// <summary>
        /// bundle依赖资源ID信息记录
        /// </summary>
        public List<Dependencies> dependentTree=new List<Dependencies>();

        /// <summary>
        /// 记录asetBundle索引
        /// </summary>
        public List<StringInt> treeNodeIndex=new List<StringInt>();

        /// <summary>
        /// 根据资源ID 记录资源名和资源BundleName的索引
        /// </summary>
        public List<StringInt> resourceIndex=new List<StringInt>();

        /// <summary>
        /// 记录所有打包资源并且记录完整路径
        /// </summary>
        [System.NonSerialized]
        public AllAssetDependenciesTreeRecord loaclRecord=new AllAssetDependenciesTreeRecord();
        /// <summary>
        /// 记录所有打包资源并且记录完整路径
        /// </summary>
        [System.NonSerialized]
        public List<StringInt> loaclResourceMap=new List<StringInt>();

        public List<string> GetAllDependenciesByIndex(int resourcesIndex)
        {
            //因为设计问题需要进行排重
            var resourceIndexSet = new HashSet<int>();
            GetAllDependencies(resourcesIndex, resourceIndexSet);
            var dependenciesPaths = new List<string>(resourceIndexSet.Count);
            Dependencies single;
            foreach (var resourceIndex in resourceIndexSet)
            {
                single = dependentTree[resourceIndex];
                if (!string.IsNullOrEmpty(single.bundleName))
                {
                    dependenciesPaths.Add(single.bundleName);
                }
            }
            return dependenciesPaths;
        }
        private void GetAllDependencies(int resourcesIndex, HashSet<int> indexSet)
        {
            Dependencies single = dependentTree[resourcesIndex];
            ///防止循环依赖
            if (!indexSet.Add(resourcesIndex))
            {
                return;
            };

            if (single.dependenciesHashArray == null)
            {
                return;
            }
            foreach (var index in single.dependenciesHashArray)
            {
                if (resourcesIndex == index)
                {
                    continue;
                }
                GetAllDependencies(index, indexSet);
            }
        }
    }

    [System.Serializable]
    public class Dependencies
    {
        public string bundleName;
        public List<int> dependenciesHashArray=new List<int>();
    }

    [System.Serializable]
    public class AssetNameBundleIndex
    {
        public string assetName;
        public int bundleIndex;
    }

    [System.Serializable]
    public class AllAssetDependenciesTreeRecord
    {
        public List<StringInt> allTreeNodeIndex=new List<StringInt>();
        public List<StringInt> bundleIndex=new List<StringInt>();
        public List<StringListString> bundleToAssetPaths=new List<StringListString>();
    }
}