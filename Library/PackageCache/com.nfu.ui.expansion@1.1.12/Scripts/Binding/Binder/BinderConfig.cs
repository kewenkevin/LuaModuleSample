using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    [Serializable]
    public class BinderConfig 
    {
        [SerializeField]
        public string binderType;

        [SerializeField]
        public ushort[] dataArray;

        public ushort StoredGameObjectIndex
        {
            get { return dataArray[0]; }
        }

        public BinderConfig(string type, List<ushort> dataList)
        {
            binderType = type;
            this.dataArray = dataList.ToArray();
        }
    }
}