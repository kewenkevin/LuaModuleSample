using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    [System.Serializable]
    public class LinkerConfig
    {
        [SerializeField]
        public ushort[] dataArray;

        /// <summary>
        /// 绑定对象的GameObject ID
        /// </summary>
        public ushort StoredGameObjectIndex
        {
            get { return dataArray[0]; }
        }

        /// <summary>
        /// 绑定对象的Component ID
        /// </summary>
        public ushort BinderTypeIndex
        {
            get { return dataArray[1]; }
        }

        /// <summary>
        /// 绑定对象的属性 ID
        /// </summary>
        public ushort LinkerTypeIndex
        {
            get { return dataArray[2]; }
        }

        /// <summary>
        /// 绑定对象的标签 ID
        /// </summary>
        public ushort LabelIndex
        {
            get { return dataArray[3]; }
        }

        /// <summary>
        /// 绑定对象的数据类型
        /// </summary>
        public ushort ValueTypeIndex
        {
            get { return dataArray[4]; }
        }

        // public ushort PrefabPathIndex
        // {
        //     get { return dataArray[4]; }
        // }

        public LinkerConfig(List<ushort> datas)
        {
            dataArray = datas.ToArray();
        }
    }
}