using System;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI.Core.Model
{
    [Serializable]
    public class GearConfig
    {
        [SerializeField]
        public GearTypeState gearType;

        [SerializeField] 
        public GearPriority priority = GearPriority.Normal;

        [SerializeField]
        public ushort[] dataArray;

        public ushort StoredGameObjectIndex
        {
            get { return dataArray[0]; }
        }

        public GearConfig(GearTypeState type, List<ushort> datas)
        {
            gearType = type;
            dataArray = datas.ToArray();
        }
    }
}