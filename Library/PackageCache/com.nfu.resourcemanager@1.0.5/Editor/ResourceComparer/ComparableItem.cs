using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ND.Managers.ResourceMgr.Editor.Comparer
{

    /// <summary>
    /// 对比项
    /// </summary>
    public class ComparableItem : IEquatable<ComparableItem>
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public string DisplayText { get; set; }

        public bool Visiable { get; set; } = true;

        private List<ComparableItemExtend> extends = new List<ComparableItemExtend>();

        public List<ComparableItemExtend> Extends { get => extends; }

        public ComparableStatus Status { get; set; }

        public int DisplayIndex { get; set; }

        public ComparableItem()
        {
        }
        public ComparableItem(string key, string value)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));
            this.Key = key;
            this.Value = value;
            this.DisplayText = key;
        }
        public ComparableItem(string key)
            : this(key, null)
        {
        }

        public void AddExtend(string name, string value)
        {
            ComparableItemExtend member = new ComparableItemExtend();
            member.Name = name;
            member.Value = value;
            if (extends == null)
                extends = new List<ComparableItemExtend>();
            extends.Add(member);
        }

        public virtual bool CanComparer(ComparableItem other)
        {
            if (Key == other.Key)
                return true;
            return false;
        }

        public virtual bool Equals(ComparableItem other)
        {
            if (!CanComparer(other))
                return false;

            //有一个值为空则判断为相等
            if (Value == null || other.Value == null)
            {
                return true;
            }
            return object.Equals(Value, other.Value);
        }

        public override bool Equals(object obj)
        {
            var other = obj as ComparableItem;
            if (other == null)
                return false;
            return Equals(other);
        }
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    public class ComparableObject : ComparableItem
    {
        public ComparableObject()
        {
        }
        public ComparableObject(string key, string value)
            : base(key, value)
        {

        }


    }

    public class ComparableText : ComparableItem
    {
        public int Start { get; set; }
        public int End { get; set; }

        public int LineNumber { get; set; }

    }
}
