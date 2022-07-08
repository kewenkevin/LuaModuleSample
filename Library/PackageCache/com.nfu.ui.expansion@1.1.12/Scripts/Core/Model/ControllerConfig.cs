using System;
using System.Linq;
using UnityEngine;

namespace ND.UI.Core.Model
{
    [Serializable]
    public class ControllerConfig
    {
        [SerializeField]
        public string name;

        [SerializeField]
        public int selectedIndex;

        [SerializeField]
        public string[] pageNames;

        [SerializeField]
        public GearConfig[] gearConfigs;

        [SerializeField] 
        public string[] pageTips;


        public void SortGears()
        {
            if (gearConfigs.Length < 2) return;
            
            var list = gearConfigs.ToList();
            list.Sort((config, gearConfig) =>
            {
                if(config.priority == gearConfig.priority)
                    return 0;
                else if (config.priority > gearConfig.priority)
                    return -1;
                else
                    return 1;
            });
            gearConfigs = list.ToArray();
        }
    }
}