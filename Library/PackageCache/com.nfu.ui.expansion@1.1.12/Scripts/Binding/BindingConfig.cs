using System;
using System.Collections.Generic;
using ND.UI;
using UnityEngine;

namespace ND.UI
{
    [Serializable]
    public class BindingConfig
    {
        [SerializeField]
        public LinkerConfig[] linkerConfigs;

        public LinkerData[] GetLinkerDataArray(UIExpansion owner)
        {
            if(linkerConfigs == null || linkerConfigs.Length == 0)
            {
                return null;
            }
            // LinkerData[] linkerDataArray = new LinkerData[linkerConfigs.Length];
            List<LinkerData> linkerDataList = new List<LinkerData>();
            for (int i = 0;i < linkerConfigs.Length; i++)
            {
                var s = owner.StoredStrings[linkerConfigs[i].BinderTypeIndex];
               if( s != "UIExpansion")
                {
                    linkerDataList.Add(new LinkerData(owner, linkerConfigs[i]));
                }
            }
            return linkerDataList.ToArray();
        }

        public ModuleData[] GetModuleDataArray(UIExpansion owner)
        {
            if (linkerConfigs == null || linkerConfigs.Length == 0)
            {
                return null;
            }
            List<ModuleData> moduleDataList = new List<ModuleData>();
            for (int i = 0; i < linkerConfigs.Length; i++)
            {
                var s = owner.StoredStrings[linkerConfigs[i].BinderTypeIndex];
                if (s == "UIExpansion")
                {
                    moduleDataList.Add(new ModuleData(owner, linkerConfigs[i]));
                }
            }
            return moduleDataList.ToArray();
        }
        
        public LinkerData[] GetModuleContainerDataArray(UIExpansion owner)
        {
            if (linkerConfigs == null || linkerConfigs.Length == 0)
            {
                return null;
            }
            List<LinkerData> linkerDataList = new List<LinkerData>();
            for (int i = 0; i < linkerConfigs.Length; i++)
            {
                if (owner.StoredStrings[linkerConfigs[i].BinderTypeIndex] == "NDModuleContainer"
                && owner.StoredStrings[linkerConfigs[i].LinkerTypeIndex] == "ModuleName")
                {
                    linkerDataList.Add(new LinkerData(owner, linkerConfigs[i]));
                }
            }
            return linkerDataList.ToArray();
        }
    }
}