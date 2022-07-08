using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ND.Framework.XLua.Example
{
    public abstract class Singleton<T> where T : class, new()
    {
        public class Options
        {

        }
        protected static T p_instance;

        public static T GetInstance()
        {
            if (p_instance != null)
            {
                //UnityEngine.Debug.LogError("GetInstance Can only be called once,public methods please use public static method(){ p_instance.[methods]}");
                return p_instance;
            }
            p_instance = Activator.CreateInstance<T>();
            return p_instance;
        }
        public virtual void Update() { }
        public virtual void LateUpdate() { }
        public virtual void OnGizmos() { }
        public virtual void OnGUI() { }
        public virtual void Initialize(Options options=null) { }
        public virtual void Dispose()
        {
            if (p_instance != null)
            {
                p_instance = null;
            }
        }
    }
    public abstract class SingletonMono<T>: MonoBehaviour where T : Component
    {
        public class Options
        {

        }
        protected static T p_instance;
        public static T GetInstance(string objName)
        {
            if (p_instance != null) return p_instance;
            var obj = new GameObject("[" + objName + "]");
            p_instance = (T)obj.AddComponent(typeof(T));
            return p_instance;
        }
        public virtual void Initialize(Options options = null) { }
        public virtual void Dispose()
        {
            if (p_instance != null)
            {
                p_instance = null;
            }
            UnityEngine.Object.Destroy(gameObject);
        }
    }
}