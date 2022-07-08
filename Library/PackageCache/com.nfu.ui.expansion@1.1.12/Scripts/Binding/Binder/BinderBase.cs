using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ND.UI
{
    public abstract class BinderBase
    {
        protected bool _active;

        protected UIExpansion _owner;

        protected string _linkerType;

        protected BinderBase _next;

        public string linkerType {
            get { return _linkerType; }
            set { _linkerType = value; }
        }

        public BinderBase(UIExpansion owner, LinkerConfig config)
        {
            _owner = owner;
            _linkerType = owner.StoredStrings[config.LinkerTypeIndex];
            Init(config);
        }

        public virtual void SetVector2(Vector2 value)
        {
            if (_next != null)
            {
                _next.SetVector2(value);
            }
        }

        public virtual void SetVector3(Vector3 value)
        {
            if (_next != null)
            {
                _next.SetVector3(value);
            }
        }

        public virtual void SetQuaternion(Quaternion value)
        {
            if (_next != null)
            {
                _next.SetQuaternion(value);
            }
        }

        public virtual void SetBoolean(bool value)
        {
            if (_next != null)
            {
                _next.SetBoolean(value);
            }
        }

        public virtual void SetInt32(int value)
        {
            if (_next != null)
            {
                _next.SetInt32(value);
            }
        }

        public virtual void SetString(string value)
        {
            if (_next != null)
            {
                _next.SetString(value);
            }
        }

        public virtual void SetSingle(float value)
        {
            if (_next != null)
            {
                _next.SetSingle(value);
            }
        }

        public virtual void SetColor(Color value)
        {
            if (_next != null)
            {
                _next.SetColor(value);
            }
        }

        public virtual void SetSprite(Sprite value)
        {
            if (_next != null)
            {
                _next.SetSprite(value);
            }
        }

        public virtual void SetChar(char value)
        {
            if (_next != null)
            {
                _next.SetChar(value);
            }
        }

        public virtual void SetRect(Rect value)
        {
            if (_next != null)
            {
                _next.SetRect(value);
            }
        }

        public virtual void SetAction(UnityAction action)
        {
            if (_next != null)
            {
                _next.SetAction(action);
            }
        }

        public virtual void RemoveAction(UnityAction action)
        {
            if (_next != null)
            {
                _next.RemoveAction(action);
            }
        }

        public virtual void SetActionBoolean(UnityAction<bool> action)
        {
            if (_next != null)
            {
                _next.SetActionBoolean(action);
            }
        }

        public virtual void RemoveActionBoolean(UnityAction<bool> action)
        {
            if (_next != null)
            {
                _next.RemoveActionBoolean(action);
            }
        }

        public virtual void SetActionVector2(UnityAction<Vector2> action)
        {
            if (_next != null)
            {
                _next.SetActionVector2(action);
            }
        }

        public virtual void RemoveActionVector2(UnityAction<Vector2> action)
        {
            if (_next != null)
            {
                _next.RemoveActionVector2(action);
            }
        }

        public virtual void SetActionSingle(UnityAction<float> action)
        {
            if (_next != null)
            {
                _next.SetActionSingle(action);
            }
        }

        public virtual void RemoveActionSingle(UnityAction<float> action)
        {
            if (_next != null)
            {
                _next.RemoveActionSingle(action);
            }
        }

        public virtual void SetActionInt32(UnityAction<int> action)
        {
            if (_next != null)
            {
                _next.SetActionInt32(action);
            }
        }

        public virtual void RemoveActionInt32(UnityAction<int> action)
        {
            if (_next != null)
            {
                _next.RemoveActionInt32(action);
            }
        }

        public virtual void SetActionString(UnityAction<string> action)
        {
            if (_next != null)
            {
                _next.SetActionString(action);
            }
        }

        public virtual void RemoveActionString(UnityAction<string> action)
        {
            if (_next != null)
            {
                _next.RemoveActionString(action);
            }
        }

        public virtual void SetAction2(UnityAction action)
        {
            if (_next != null)
            {
                _next.SetAction2(action);
            }
        }

        public virtual void RemoveAction2(UnityAction action)
        {
            if (_next != null)
            {
                _next.RemoveAction2(action);
            }
        }
        public virtual void SetAction2Boolean(UnityAction<bool> action)
        {
            if (_next != null)
            {
                _next.SetAction2Boolean(action);
            }
        }

        public virtual void RemoveAction2Boolean(UnityAction<bool> action)
        {
            if (_next != null)
            {
                _next.RemoveAction2Boolean(action);
            }
        }


        public virtual void SetAction2Vector2(UnityAction<Vector2> action)
        {
            if (_next != null)
            {
                _next.SetAction2Vector2(action);
            }
        }

        public virtual void RemoveAction2Vector2(UnityAction<Vector2> action)
        {
            if (_next != null)
            {
                _next.RemoveAction2Vector2(action);
            }
        }

        public virtual void SetAction2Single(UnityAction<float> action)
        {
            if (_next != null)
            {
                _next.SetAction2Single(action);
            }
        }

        public virtual void RemoveAction2Single(UnityAction<float> action)
        {
            if (_next != null)
            {
                _next.RemoveAction2Single(action);
            }
        }

        public virtual void SetAction2Int32(UnityAction<int> action)
        {
            if (_next != null)
            {
                _next.SetAction2Int32(action);
            }
        }

        public virtual void RemoveAction2Int32(UnityAction<int> action)
        {
            if (_next != null)
            {
                _next.RemoveAction2Int32(action);
            }
        }

        public virtual void SetAction2String(UnityAction<string> action)
        {
            if (_next != null)
            {
                _next.SetAction2String(action);
            }
        }

        public virtual void RemoveAction2String(UnityAction<string> action)
        {
            if (_next != null)
            {
                _next.RemoveAction2String(action);
            }
        }
        public virtual void SetSystemObject(System.Object value)
        {
            if (_next != null)
            {
                _next.SetSystemObject(value);
            }
        }

        public virtual void RemoveAllAction()
        {

        }

        public virtual void SetSystemActionIntAndObject(System.Action<int, object> action)
        {
            if (_next != null)
            {
                _next.SetSystemActionIntAndObject(action);
            }
        }

        public virtual void RemoveSystemActionIntAndObject(System.Action<int, object> action)
        {
            if (_next != null)
            {
                _next.RemoveSystemActionIntAndObject(action);
            }
        }

        public virtual void SetSystemActionIntAndBool(System.Action<int, bool> action)
        {
            if (_next != null)
            {
                _next.SetSystemActionIntAndBool(action);
            }
        }
        
        public virtual void RemoveSystemActionIntAndBool(System.Action<int, bool> action)
        {
            if (_next != null)
            {
                _next.RemoveSystemActionIntAndBool(action);
            }
        }
        
        public virtual void SetSystemActionObject(System.Action<object> action)
        {
            if (_next != null)
            {
                _next.SetSystemActionObject(action);
            }
        }
        
        public virtual void RemoveSystemActionObject(System.Action<object> action)
        {
            if (_next != null)
            {
                _next.RemoveSystemActionObject(action);
            }
        }

        // public virtual void SetDelegateInt(Yoozoo.UI.YGUI.ListView.ListItemProvider function)
        // {
        //     if (_next != null)
        //     {
        //         _next.SetDelegateInt(function);
        //     }
        // }
        //
        // public virtual void RemoveDelegateInt(Yoozoo.UI.YGUI.ListView.ListItemProvider function)
        // {
        //     if (_next != null)
        //     {
        //         _next.RemoveDelegateInt(function);
        //     }
        // }
        
        public virtual void SetDelegateInt(Func<int,string> function)
        {
            if (_next != null)
            {
                _next.SetDelegateInt(function);
            }
        }

        public virtual void RemoveDelegateInt(Func<int,string> function)
        {
            if (_next != null)
            {
                _next.RemoveDelegateInt(function);
            }
        }

        public abstract void Init(LinkerConfig config);

        public void SetNextBinder(BinderBase binder)
        {
            BinderBase checkBinder = this;
            while (checkBinder._next != null)
            {
                checkBinder = checkBinder._next;
            }
            checkBinder._next = binder;
        }


        public virtual void Dispose()
        {
            
        }
    }
}