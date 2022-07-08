using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ND.Managers.ResourceMgr.Editor.Comparer
{
    /// <summary>
    /// 对比状态
    /// </summary>
    public enum ComparableStatus
    {
        None,
        /// <summary>
        /// 新增的
        /// </summary>
        Added = 0x1,
        /// <summary>
        /// 变化的
        /// </summary>
        Changed = 0x2,
        /// <summary>
        /// 移除的
        /// </summary>
        Removed = 0x4,
    }

}
