//------------------------------------------------------------
// Game Framework
// Copyright © 2013-2020 Jiang Yin. All rights reserved.
// Homepage: https://gameframework.cn/
// Feedback: mailto:ellan@gameframework.cn
//------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using ND.Managers.ResourceMgr.Runtime;

namespace ND.Managers.ResourceMgr.Framework.ObjectPool
{
    public sealed partial class ObjectPoolManager : GameFrameworkModule, IObjectPoolManager
    {
        /// <summary>
        /// 对象池。
        /// </summary>
        /// <typeparam name="T">对象类型。</typeparam>
        private sealed class ObjectPool<T> : ObjectPoolBase, IObjectPool<T> where T : ObjectBase
        {
            private readonly GameFrameworkMultiDictionary<string, Object<T>> m_Objects;
            private readonly Dictionary<object, Object<T>> m_ObjectMap;
            private readonly ReleaseObjectFilterCallback<T> m_DefaultReleaseObjectFilterCallback;
            private readonly List<T> m_CachedCanReleaseObjects;
            private readonly List<T> m_CachedToReleaseObjects;
            private readonly bool m_AllowMultiSpawn;
            private float m_AutoReleaseInterval;
            private int m_Capacity;
            private float m_ExpireTime;
            private int m_Priority;
            private float m_AutoReleaseTime;
            //循环依赖检测
            private HashSet<T> checkCircleUnspawn = new HashSet<T>();
            private int unspawnDepth;
            /// <summary>
            /// 释放循环引用检查
            /// </summary>
            private List<Object<T>> releaseCircleDepAllZeroRef = new List<Object<T>>();
            private HashSet<Object<T>> releaseCircleDepGroup = new HashSet<Object<T>>();

            /// <summary>
            /// 初始化对象池的新实例。
            /// </summary>
            /// <param name="name">对象池名称。</param>
            /// <param name="allowMultiSpawn">是否允许对象被多次获取。</param>
            /// <param name="autoReleaseInterval">对象池自动释放可释放对象的间隔秒数。</param>
            /// <param name="capacity">对象池的容量。</param>
            /// <param name="expireTime">对象池对象过期秒数。</param>
            /// <param name="priority">对象池的优先级。</param>
            public ObjectPool(string name, bool allowMultiSpawn, float autoReleaseInterval, int capacity, float expireTime, int priority)
                : base(name)
            {
                m_Objects = new GameFrameworkMultiDictionary<string, Object<T>>();
                m_ObjectMap = new Dictionary<object, Object<T>>();
                m_DefaultReleaseObjectFilterCallback = DefaultReleaseObjectFilterCallback;
                m_CachedCanReleaseObjects = new List<T>();
                m_CachedToReleaseObjects = new List<T>();
                m_AllowMultiSpawn = allowMultiSpawn;
                m_AutoReleaseInterval = autoReleaseInterval;
                Capacity = capacity;
                ExpireTime = expireTime;
                m_Priority = priority;
                m_AutoReleaseTime = 0f;
            }


            public GameFrameworkMultiDictionary<string, ObjectPoolManager.Object<T>> GetAll
            {
                get { return m_Objects; }
            }

            /// <summary>
            /// 获取对象池对象类型。
            /// </summary>
            public override Type ObjectType
            {
                get
                {
                    return typeof(T);
                }
            }

            /// <summary>
            /// 获取对象池中对象的数量。
            /// </summary>
            public override int Count
            {
                get
                {
                    return Mathf.Max(m_Objects.Count, m_ObjectMap.Count);
                }
            }

            /// <summary>
            /// 获取对象池中能被释放的对象的数量。
            /// </summary>
            public override int CanReleaseCount
            {
                get
                {
                    GetCanReleaseObjects(m_CachedCanReleaseObjects);
                    return m_CachedCanReleaseObjects.Count;
                }
            }

            /// <summary>
            /// 获取是否允许对象被多次获取。
            /// </summary>
            public override bool AllowMultiSpawn
            {
                get
                {
                    return m_AllowMultiSpawn;
                }
            }

            /// <summary>
            /// 获取或设置对象池自动释放可释放对象的间隔秒数。
            /// </summary>
            public override float AutoReleaseInterval
            {
                get
                {
                    return m_AutoReleaseInterval;
                }
                set
                {
                    m_AutoReleaseInterval = value;
                }
            }

            /// <summary>
            /// 获取或设置对象池的容量。
            /// </summary>
            public override int Capacity
            {
                get
                {
                    return m_Capacity;
                }
                set
                {
                    if (value < 0)
                    {
                        throw new GameFrameworkException("Capacity is invalid.");
                    }

                    if (m_Capacity == value)
                    {
                        return;
                    }

                    m_Capacity = value;
                    Release();
                }
            }

            /// <summary>
            /// 获取或设置对象池对象过期秒数。
            /// </summary>
            public override float ExpireTime
            {
                get
                {
                    return m_ExpireTime;
                }

                set
                {
                    if (value < 0f)
                    {
                        throw new GameFrameworkException("ExpireTime is invalid.");
                    }

                    if (ExpireTime == value)
                    {
                        return;
                    }

                    m_ExpireTime = value;
                    Release();
                }
            }

            /// <summary>
            /// 获取或设置对象池的优先级。
            /// </summary>
            public override int Priority
            {
                get
                {
                    return m_Priority;
                }
                set
                {
                    m_Priority = value;
                }
            }

            /// <summary>
            /// 创建对象。
            /// </summary>
            /// <param name="obj">对象。</param>
            /// <param name="spawned">对象是否已被获取。</param>
            public void Register(T obj, bool spawned)
            {
                if (obj == null)
                {
                    throw new GameFrameworkException("Object is invalid.");
                }

                Object<T> internalObject = Object<T>.Create(obj, spawned);
                m_Objects.Add(obj.Name, internalObject);
                if (obj.Target != null)
                    SetTarget(obj, obj.Target);


                if (Count > m_Capacity)
                {
                    Release();
                }
            }

            /// <summary>
            /// 检查对象。
            /// </summary>
            /// <returns>要检查的对象是否存在。</returns>
            public bool CanSpawn()
            {
                return CanSpawn(string.Empty);
            }

            /// <summary>
            /// 检查对象。
            /// </summary>
            /// <param name="name">对象名称。</param>
            /// <returns>要检查的对象是否存在。</returns>
            public bool CanSpawn(string name)
            {
                GameFrameworkLinkedListRange<Object<T>> objectRange = default(GameFrameworkLinkedListRange<Object<T>>);
                if (m_Objects.TryGetValue(name, out objectRange))
                {
                    foreach (Object<T> internalObject in objectRange)
                    {
                        if (m_AllowMultiSpawn || !internalObject.IsInUse)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            /// <summary>
            /// 获取对象。
            /// </summary>
            /// <returns>要获取的对象。</returns>
            public T Spawn()
            {
                return Spawn(string.Empty);
            }

            /// <summary>
            /// 获取对象。
            /// </summary>
            /// <param name="name">对象名称。</param>
            /// <returns>要获取的对象。</returns>
            public T Spawn(string name)
            {
                GameFrameworkLinkedListRange<Object<T>> objectRange = default(GameFrameworkLinkedListRange<Object<T>>);
                if (m_Objects.TryGetValue(name, out objectRange))
                {
                    foreach (Object<T> internalObject in objectRange)
                    {
                        if (m_AllowMultiSpawn || !internalObject.IsInUse)
                        {
                            return internalObject.Spawn();
                        }
                    }
                }

                return null;
            }

            /// <summary>
            /// 回收对象。
            /// </summary>
            /// <param name="obj">要回收的对象。</param>
            public void Unspawn(T obj)
            {
                if (obj == null)
                {
                    throw new GameFrameworkException("Object is invalid.");
                }
                Unspawn(obj.Name);
            }
            /// <summary>
            /// 回收对象。
            /// </summary>
            /// <param name="target">要回收的对象。</param>
            public void Unspawn(object target)
            {
                if (target == null)
                {
                    throw new GameFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = GetObject(target);

                if (internalObject == null)
                    throw new GameFrameworkException(Utility.Text.Format("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.", new TypeNamePair(typeof(T), Name).ToString(), target.GetType().FullName, target.ToString()));

                Unspawn(internalObject);
            }

            public void Unspawn(string name)
            {
                GameFrameworkLinkedListRange<Object<T>> objectRange;
                if (m_Objects.TryGetValue(name, out objectRange))
                {
                    if (objectRange.First != null)
                    {
                        Object<T> internalObject;
                        internalObject = objectRange.First.Value;
                        if (internalObject != null)
                        {
                            Unspawn(internalObject);
                        }
                    }
                }
            }

            public void Unspawn(Object<T> internalObject)
            {
                if (internalObject != null)
                {

                    try
                    {
                        unspawnDepth++;
                        if (unspawnDepth == 1)
                            checkCircleUnspawn.Clear();
                        if (checkCircleUnspawn.Contains(internalObject.Target))
                        {
                            //循环依赖，停止引用减数
                            return;
                        }
                        checkCircleUnspawn.Add(internalObject.Target);
                        internalObject.Unspawn();
                        if (Count > m_Capacity && internalObject.SpawnCount <= 0)
                        {
                            Release();
                        }
                    }
                    catch
                    {
                        throw;
                    }
                    finally
                    {
                        unspawnDepth--;
                    }

                }
                else
                {
                    throw new GameFrameworkException(Utility.Text.Format("Can not find target in object pool '{0}', target name is '{1}'.", new TypeNamePair(typeof(T), Name).ToString(), internalObject.Name));
                }
            }

            /// <summary>
            /// 设置对象是否被加锁。
            /// </summary>
            /// <param name="obj">要设置被加锁的对象。</param>
            /// <param name="locked">是否被加锁。</param>
            public void SetLocked(T obj, bool locked)
            {
                if (obj == null)
                {
                    throw new GameFrameworkException("Object is invalid.");
                }

                SetLocked(obj.Target, locked);
            }

            /// <summary>
            /// 设置对象是否被加锁。
            /// </summary>
            /// <param name="target">要设置被加锁的对象。</param>
            /// <param name="locked">是否被加锁。</param>
            public void SetLocked(object target, bool locked)
            {
                if (target == null)
                {
                    throw new GameFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = GetObject(target);
                if (internalObject != null)
                {
                    internalObject.Locked = locked;
                }
                else
                {
                    throw new GameFrameworkException(Utility.Text.Format("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.", new TypeNamePair(typeof(T), Name).ToString(), target.GetType().FullName, target.ToString()));
                }
            }

            /// <summary>
            /// 设置对象的优先级。
            /// </summary>
            /// <param name="obj">要设置优先级的对象。</param>
            /// <param name="priority">优先级。</param>
            public void SetPriority(T obj, int priority)
            {
                if (obj == null)
                {
                    throw new GameFrameworkException("Object is invalid.");
                }

                SetPriority(obj.Target, priority);
            }

            /// <summary>
            /// 设置对象的优先级。
            /// </summary>
            /// <param name="target">要设置优先级的对象。</param>
            /// <param name="priority">优先级。</param>
            public void SetPriority(object target, int priority)
            {
                if (target == null)
                {
                    throw new GameFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = GetObject(target);
                if (internalObject != null)
                {
                    internalObject.Priority = priority;
                }
                else
                {
                    throw new GameFrameworkException(Utility.Text.Format("Can not find target in object pool '{0}', target type is '{1}', target value is '{2}'.", new TypeNamePair(typeof(T), Name).ToString(), target.GetType().FullName, target.ToString()));
                }
            }

            /// <summary>
            /// 释放对象池中的可释放对象。
            /// </summary>
            public override void Release()
            {
                Release(Count - m_Capacity, m_DefaultReleaseObjectFilterCallback);
            }

            /// <summary>
            /// 释放对象池中的可释放对象。
            /// </summary>
            /// <param name="toReleaseCount">尝试释放对象数量。</param>
            public override void Release(int toReleaseCount)
            {
                Release(toReleaseCount, m_DefaultReleaseObjectFilterCallback);
            }

            /// <summary>
            /// 释放对象池中的可释放对象。
            /// </summary>
            /// <param name="releaseObjectFilterCallback">释放对象筛选函数。</param>
            public void Release(ReleaseObjectFilterCallback<T> releaseObjectFilterCallback)
            {
                Release(Count - m_Capacity, releaseObjectFilterCallback);
            }

            /// <summary>
            /// 释放对象池中的可释放对象。
            /// </summary>
            /// <param name="toReleaseCount">尝试释放对象数量。</param>
            /// <param name="releaseObjectFilterCallback">释放对象筛选函数。</param>
            public void Release(int toReleaseCount, ReleaseObjectFilterCallback<T> releaseObjectFilterCallback)
            {
                if (releaseObjectFilterCallback == null)
                {
                    throw new GameFrameworkException("Release object filter callback is invalid.");
                }

                if (toReleaseCount < 0)
                {
                    toReleaseCount = 0;
                }

                DateTime expireTime = DateTime.MinValue;
                if (m_ExpireTime < float.MaxValue)
                {
                    expireTime = DateTime.Now.AddSeconds(-m_ExpireTime);
                }

                m_AutoReleaseTime = 0f;
                GetCanReleaseObjects(m_CachedCanReleaseObjects);
                List<T> toReleaseObjects = releaseObjectFilterCallback(m_CachedCanReleaseObjects, toReleaseCount, expireTime);
                if (toReleaseObjects == null || toReleaseObjects.Count <= 0)
                {
                    return;
                }

                foreach (T toReleaseObject in toReleaseObjects)
                {
                    ReleaseObject(toReleaseObject);
                }

                ReleaseAllUnusedCircleDepend();
            }

            /// <summary>
            /// 释放对象池中的所有未使用对象。
            /// </summary>
            public override void ReleaseAllUnused()
            {
                m_AutoReleaseTime = 0f;
                GetCanReleaseObjects(m_CachedCanReleaseObjects);
                foreach (T toReleaseObject in m_CachedCanReleaseObjects)
                {
                    ReleaseObject(toReleaseObject);
                }

                ReleaseAllUnusedCircleDepend();
            }

            /// <summary>
            /// 获取所有对象信息。
            /// </summary>
            /// <returns>所有对象信息。</returns>
            public override ObjectInfo[] GetAllObjectInfos()
            {
                List<ObjectInfo> results = new List<ObjectInfo>();
                foreach (KeyValuePair<string, GameFrameworkLinkedListRange<Object<T>>> objectRanges in m_Objects)
                {
                    foreach (Object<T> internalObject in objectRanges.Value)
                    {
                        results.Add(new ObjectInfo(internalObject.Name, internalObject.Locked, internalObject.CustomCanReleaseFlag, internalObject.Priority, internalObject.LastUseTime, internalObject.SpawnCount));
                    }
                }

                return results.ToArray();
            }

            internal override void Update(float elapseSeconds, float realElapseSeconds)
            {
                m_AutoReleaseTime += realElapseSeconds;
                if (m_AutoReleaseTime < m_AutoReleaseInterval)
                {
                    return;
                }

                Release();
            }

            internal override void Shutdown()
            {
                foreach (KeyValuePair<object, Object<T>> objectInMap in m_ObjectMap)
                {
                    objectInMap.Value.Release(true);
                    ReferencePool.Release(objectInMap.Value);
                }

                m_Objects.Clear();
                m_ObjectMap.Clear();
                m_CachedCanReleaseObjects.Clear();
                m_CachedToReleaseObjects.Clear();
            }

            public T Peek(object target)
            {
                return GetObject(target)?.Peek();
            }

            public T Peek(string name)
            {
                var obj = GetObject(name);
                if (obj != null)
                    return obj.Peek();
                return default(T);
            }

            public Object<T> GetObject(object target)
            {
                if (target == null)
                {
                    throw new GameFrameworkException("Target is invalid.");
                }

                Object<T> internalObject = null;
                if (m_ObjectMap.TryGetValue(target, out internalObject))
                {
                    return internalObject;
                }

                return null;
            }
            public Object<T> GetObject(string name)
            {
                Object<T> internalObject;
                GameFrameworkLinkedListRange<Object<T>> range;
                m_Objects.TryGetValue(name, out range);
                if (range.First == null)
                {
                    return null;
                }

                internalObject = range.First.Value;
                return internalObject;
            }

            private void ReleaseObject(T obj)
            {
                if (obj == null)
                {
                    throw new GameFrameworkException("Object is invalid.");
                }
                //Log.Info($"[{Name}] Release '{ obj.Name}'");
                Object<T> internalObject = GetObject(obj.Name);
                if (internalObject == null)
                {
                    throw new GameFrameworkException("Can not release object which is not found.");
                }

                m_Objects.Remove(obj.Name, internalObject);
                Object<T> mapObj;
                if (obj.Target != null && m_ObjectMap.TryGetValue(obj.Target, out mapObj))
                {
                    ///可能多key 映射到同一个target
                    if (mapObj.Target == obj)
                    {
                        m_ObjectMap.Remove(obj.Target);
                    }
                    else
                    {

                    }
                }

                internalObject.Release(false);
                ReferencePool.Release(internalObject);
            }

            private void GetCanReleaseObjects(List<T> results)
            {
                if (results == null)
                {
                    throw new GameFrameworkException("Results is invalid.");
                }

                results.Clear();
                foreach (var item in m_Objects)
                {
                    if (item.Value.First == null)
                        continue;
                    Object<T> internalObject = item.Value.First.Value;
                    if (internalObject == null)
                        continue;
                    if (internalObject.IsInUse || internalObject.Locked || !internalObject.CustomCanReleaseFlag)
                    {
                        continue;
                    }

                    results.Add(internalObject.Peek());
                }
            }

            private List<T> DefaultReleaseObjectFilterCallback(List<T> candidateObjects, int toReleaseCount, DateTime expireTime)
            {
                m_CachedToReleaseObjects.Clear();

                if (expireTime > DateTime.MinValue)
                {
                    for (int i = candidateObjects.Count - 1; i >= 0; i--)
                    {
                        if (candidateObjects[i].LastUseTime <= expireTime)
                        {
                            m_CachedToReleaseObjects.Add(candidateObjects[i]);
                            candidateObjects.RemoveAt(i);
                            continue;
                        }
                    }

                    toReleaseCount -= m_CachedToReleaseObjects.Count;
                }

                for (int i = 0; toReleaseCount > 0 && i < candidateObjects.Count; i++)
                {
                    for (int j = i + 1; j < candidateObjects.Count; j++)
                    {
                        if (candidateObjects[i].Priority > candidateObjects[j].Priority
                            || candidateObjects[i].Priority == candidateObjects[j].Priority && candidateObjects[i].LastUseTime > candidateObjects[j].LastUseTime)
                        {
                            T temp = candidateObjects[i];
                            candidateObjects[i] = candidateObjects[j];
                            candidateObjects[j] = temp;
                        }
                    }

                    m_CachedToReleaseObjects.Add(candidateObjects[i]);
                    toReleaseCount--;
                }

                return m_CachedToReleaseObjects;
            }
            public void SetTarget(T obj, object newTarget)
            {
                if (obj.Target != null)
                {
                    if (m_ObjectMap.ContainsKey(obj.Target))
                        m_ObjectMap.Remove(obj.Target);
                }

                if (newTarget != null)
                {
                    Object<T> internalObject = GetObject(obj.Name);
                    if (internalObject != null)
                    {
                        m_ObjectMap[newTarget] = internalObject;
                    }
                }

            }



            /// <summary>
            /// 释放循环依赖
            /// </summary>
            void ReleaseAllUnusedCircleDepend()
            {

                releaseCircleDepAllZeroRef.Clear();

                Action<Object<T>, HashSet<Object<T>>> GetAllBeDependObjects = null;

                GetAllBeDependObjects = (obj, set) =>
                {
                    foreach (T beDep in obj.Target.GetBeDependencyResources())
                    {
                        var beObj = GetObject(beDep.Name);
                        if (beObj != null && !set.Contains(beObj))
                        {
                            set.Add(beObj);
                            GetAllBeDependObjects(beObj, set);
                        }
                    }
                };
                //获取所有引用数为0的资源
                foreach (var item in m_Objects)
                {
                    if (item.Value.First == null)
                        continue;
                    Object<T> internalObject = item.Value.First.Value;
                    if (internalObject == null)
                        continue;
                    if (internalObject.IsInUse || internalObject.Locked)
                    {
                        continue;
                    }
                    releaseCircleDepAllZeroRef.Add(internalObject);
                }

                if (releaseCircleDepAllZeroRef.Count > 0)
                {
                    for (int i = 0, len = releaseCircleDepAllZeroRef.Count; i < len; i++)
                    {
                        var obj = releaseCircleDepAllZeroRef[i];
                        if (obj == null)
                            continue;
                        releaseCircleDepGroup.Clear();
                        //获取所有被依赖的资源作为依赖组
                        GetAllBeDependObjects(obj, releaseCircleDepGroup);

                        //忽略非循环依赖
                        if (!releaseCircleDepGroup.Contains(obj))
                        {
                            continue;
                        }

                        bool hasSpawn = false;

                        //依赖组中任何资源是否被使用中
                        foreach (var beDep in releaseCircleDepGroup)
                        {
                            var beObj = GetObject(beDep.Name);
                            if (beObj == null)
                                break;
                            if (beObj.SpawnCount > 0)
                            {
                                hasSpawn = true;
                                break;
                            }
                        }

                        //释放循环依赖组的资源
                        for (int j = i; j < len && releaseCircleDepGroup.Count > 0; j++)
                        {
                            var item = releaseCircleDepAllZeroRef[j];
                            if (item == null)
                                continue;
                            if (releaseCircleDepGroup.Contains(item))
                            {
                                releaseCircleDepGroup.Remove(item);
                                if (!hasSpawn)
                                {
                                    //释放没有被使用的资源
                                    ReleaseObject(item.Target);
                                }
                                //标记已处理过
                                releaseCircleDepAllZeroRef[j] = null;
                            }
                        }
                    }
                }
            }



        }

    }
}
