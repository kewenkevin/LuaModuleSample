using UnityEngine;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using ND.UI.NDUI.Utility;
using UnityEngine.UI;

namespace ND.UI.NDUI
{
    [AddComponentMenu("NDUI/NDList", 12)]
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    public class NDList : UIBehaviour
    {
        public object bindList;
        public Action<GameObject,int> onAddHandler;
        public Action<GameObject,int> onDeleteHandler;
        //[Tooltip("Total count, negative means INFINITE mode")]
        [SerializeField]
        private int m_totalCount;

        public int totalCount { get => m_totalCount; set { m_totalCount = value; UpdateCurrent(m_totalCount); } }
        public enum Direction
        {
            Horizontal = 0,
            Vertical = 1,
        }
        [SerializeField]
        private Direction m_Direction = Direction.Horizontal;

        [SerializeField]
        public GameObject prefab;
        
        /// <summary>
        /// The direction of the List, from minimum to maximum value.
        /// </summary>
        /// <example>
        /// <code>
        /// using UnityEngine;
        /// using System.Collections;
        /// using UnityEngine.UI; // Required when Using UI elements.
        ///
        /// public class Example : MonoBehaviour
        /// {
        ///     public YList mainList;
        ///
        ///     public void Start()
        ///     {
        ///         //Changes the direction of the slider.
        ///         if (mainList.direction == Slider.Direction.Horizontal)
        ///         {
        ///             mainList.direction = Slider.Direction.Vertical;
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        public Direction direction { get { return m_Direction; } set { if (SetPropertyUtility.SetStruct(ref m_Direction, value)) SetDirection(value); } }
        public void SetDirection(Direction direction)
        {
            if (direction==Direction.Horizontal)
            {
                var hori = gameObject.GetComponent<VerticalLayoutGroup>();
                if (hori!=null)
                {
                    UnityEngine.Object.DestroyImmediate(hori);
                }
                gameObject.AddComponent<HorizontalLayoutGroup>();
            }
            else
            {
                var hori = gameObject.GetComponent<HorizontalLayoutGroup>();
                if (hori!=null)
                {
                    UnityEngine.Object.DestroyImmediate(hori);
                }
                gameObject.AddComponent<VerticalLayoutGroup>();
            }
        }

        public void UpdateCurrent(int curTotal)
        {
            if (Application.isPlaying)
                return;


#if UNITY_EDITOR
            if(prefab == null)
            {
                return;
            }
            int oldCnt = this.transform.childCount;
            if (oldCnt == curTotal)
            {
                return;
            }
            
            int tmp = curTotal - oldCnt;
            if(tmp > 0)
            {
                for(int i = 0;i<tmp;i++)
                {
                    // GameObject obj = GameObject.Instantiate(prefab,this.transform);
                    var obj = UnityEditor.PrefabUtility.InstantiatePrefab(prefab, this.transform);
                }
            }
            else
            {
                tmp = 0 - tmp;
                
                for (int i = 0; i < tmp; i++)
                {
                    GameObject.DestroyImmediate(this.transform.GetChild(0).gameObject);
                }
            }
#endif
        }

        public void ReloadData()
        {
            //已有的显示元素
            var objsLens = transform.childCount;

            var maxCount = Math.Max(totalCount, objsLens);
            
            Stack<Transform> needRemove = new Stack<Transform>();
            
            
            for (int i = 0; i < maxCount; i++)
            {

                if (i <= totalCount)
                {
                    GameObject obj;
                    if (i >= objsLens)
                    {
                        obj = Instantiate(prefab, Vector3.one, Quaternion.identity);
                        obj.transform.SetParent(transform);
                        obj.transform.localPosition = Vector3.zero;
                        obj.transform.localScale = Vector3.one;
                    }
                    else
                    {
                        obj = transform.GetChild(i).gameObject;
                    }
                    onAddHandler?.Invoke(obj,i);
                }
                else
                {
                    var obj = transform.GetChild(i).gameObject;
                    onDeleteHandler?.Invoke(obj, i);
                    UnityEngine.Object.Destroy(obj);

                    needRemove.Push(obj.transform);
                }

            }

            while (needRemove.Count>0)
            {
                needRemove.Pop().SetParent(null);
            }
        }
    }

   
}