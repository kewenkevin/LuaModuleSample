using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace ND.Managers.ResourceMgr.Runtime
{

    /// <summary>
    /// 效率更高的对象池，复用对象避免GC
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal static class ObjectPool<T>
    {
        private static Queue<T> freelist = new Queue<T>();
        private static IProvider provider;

        static ObjectPool()
        {
            Initalize();
        }

        static void Initalize()
        {
            foreach (var type in AppDomain.CurrentDomain.GetAssemblies().Referenced(typeof(IProvider).Assembly)
                .SelectMany(o => o.GetTypes()))
            {
                if (type.IsAbstract || !typeof(IProvider).IsAssignableFrom(type))
                    continue;
                var p = Activator.CreateInstance(type) as IProvider;
                if (provider == null || p.Priority > provider.Priority)
                {
                    provider = p;
                }
            }
        }


        public static T Get()
        {
            T obj;

            if (freelist.Count > 0)
            {
                obj = freelist.Dequeue();
                if (provider != null)
                {
                    provider.Use(obj, false);
                }
            }
            else
            {
                if (provider != null)
                {
                    obj = provider.Create();
                    provider.Use(obj, true);
                }
                else
                {
                    obj = Activator.CreateInstance<T>();
                }
            }
            return obj;
        }

        public static void Release(T obj)
        {
            if (provider != null)
                provider.Release(obj);
            freelist.Enqueue(obj);
        }


        public interface IProvider
        {
            /// <summary>
            /// 如果存在多个提供程序使用优先级值更大的提供程序
            /// </summary>
            int Priority { get; }

            /// <summary>
            /// 创建实例
            /// </summary>
            T Create();

            /// <summary>
            /// 使用
            /// </summary>
            /// <param name="obj"></param>
            void Use(T obj, bool newCreate);

            /// <summary>
            /// 释放
            /// </summary>
            /// <param name="obj"></param>
            void Release(T obj);
        }

    }
}