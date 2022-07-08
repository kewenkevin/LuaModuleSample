using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ND.UI.NDUI
{
    public class GameObjectPool
    {
        // public class Item
        // {
        //     public GameObject gameObject;
        //     // public object widget;
        // }
        
        public delegate object CreatedGO(GameObject obj);
        public delegate void RemoveGO(GameObject obj);

        public CreatedGO onCreatedGO;

        public RemoveGO onRemoveGO;

        Stack<GameObject> m_pool;

        Dictionary<string, Stack<GameObject>> m_poolDic = new Dictionary<string, Stack<GameObject>>();

        GameObject m_prefab;

        Dictionary<string, GameObject> m_prefabDic = new Dictionary<string, GameObject>();

        Transform m_root;

        // public GameObjectPool(Transform trans, GameObject prefab)
        // {
        //     m_root = trans;
        //     m_prefab = prefab;
        //     m_pool = new Queue<GameObject>();
        // }
        //
        // public GameObjectPool(Transform trans, GameObject prefab, string identifyName)
        // {
        //     m_root = trans;
        //     m_prefabDic.Add(identifyName,prefab);
        //     m_poolDic.Add(identifyName,new Queue<GameObject>());
        // }

        public void CreatePool(Transform trans, GameObject prefab)
        {
            m_root = trans;
            m_prefab = prefab;
            if (m_pool == null)
            {
                m_pool = new Stack<GameObject>();
            }
        }

        public void CreatePool(Transform trans, GameObject prefab, string identifyName)
        {
            //防止List复用时候重复创建池子
            if (m_pool == null)
            {
                m_pool = new Stack<GameObject>();
            }
            m_root = trans;
            if (!m_poolDic.ContainsKey(identifyName))
            {
                m_poolDic.Add(identifyName, new Stack<GameObject>());
            }
            if (!m_prefabDic.ContainsKey(identifyName))
            {
                m_prefabDic.Add(identifyName, prefab);
            }
            //TODO 删了
            // GameObject go = Object.Instantiate(prefab);
            // onCreatedGO?.Invoke(go);
            //
            // go.name = temp.ToString();
            // temp++;
            // Put(identifyName,go);
            
            // Item item = new Item();
            // item.gameObject = go;
            // item.widget = widget;

            // m_prefabDic.Add(identifyName, prefab);
            // m_poolDic.Add(identifyName, new Queue<GameObject>());
        }

        public GameObject Get()
        {
            if (m_pool.Count > 0)
                return m_pool.Pop();

            GameObject go = Object.Instantiate(m_prefab);
            // Item item = new Item();
            // item.gameObject = go;
            return go;
        }

        private int temp = 0;
        public GameObject Get(string identifyName)//TODO 对齐FGUI
        {
            if (m_poolDic.TryGetValue(identifyName, out var queue))
            {
                if (queue.Count > 0)
                {
                    return queue.Pop();
                }

                if (m_prefabDic.TryGetValue(identifyName, out var prefab))
                {
                    GameObject go = Object.Instantiate(prefab.gameObject);
                    onCreatedGO?.Invoke(go);
                    go.name = temp.ToString();
                    temp++;
                    // Item item = new Item();
                    // item.gameObject = go;
                    // item.widget = widget;
                    return go;
                }
            }
            throw new Exception($"Can not Found {identifyName} Queue, Pls Check Default Prefab Setting");
        }

        public void Put(string identifyName, GameObject item)
        {
            if (item == null) return;
            item.gameObject.transform.SetParent(m_root);
            item.gameObject.SetActive(false);
            if (!m_prefabDic.ContainsKey(identifyName))
            {
                m_prefabDic.Add(identifyName, item);
            }

            if (m_poolDic.ContainsKey(identifyName))
            {
                m_poolDic[identifyName].Push(item);
            }
            else
            {
                m_poolDic.Add(identifyName, new Stack<GameObject>());
                m_poolDic[identifyName].Push(item);
            }
        }

        public void Put(GameObject item)
        {
            if (item == null) return;
            item.gameObject.transform.SetParent(m_root);
            item.gameObject.SetActive(false);
            m_pool.Push(item);
        }

        public void Clear()
        {
            if (m_pool == null || m_pool.Count == 0)
            {
                return;
            }
            
            foreach (var item in m_pool)
            {
                onRemoveGO?.Invoke(item.gameObject);
                GameObject.Destroy(item.gameObject);
                // item.widget = null;
            }

            foreach (var VARIABLE in m_poolDic)
            {
                foreach (var ob in VARIABLE.Value)
                {
                    onRemoveGO?.Invoke(ob.gameObject);
                    GameObject.Destroy(ob.gameObject);
                    // ob.widget = null;
                }
            }

            foreach (var VARIABLE in m_prefabDic)
            {
                onRemoveGO?.Invoke(VARIABLE.Value.gameObject);
                GameObject.Destroy(VARIABLE.Value.gameObject);
                // VARIABLE.Value.widget = null;
            }

            m_prefabDic.Clear();
            m_poolDic.Clear();
            m_pool.Clear();
        }
    }
}