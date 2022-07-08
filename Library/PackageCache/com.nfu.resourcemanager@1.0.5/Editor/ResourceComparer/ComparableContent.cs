using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ND.Managers.ResourceMgr.Editor.Comparer
{
    /// <summary>
    /// 对比内容
    /// </summary>
    class ComparableContent
    {
        private SortedDictionary<string, ComparableItem> sortedItems = new SortedDictionary<string, ComparableItem>();
        private Dictionary<string, ComparableItem> items = new Dictionary<string, ComparableItem>();

        //public ResourceInfo[] ResourceInfos;

        //public List<ResourceInfo> DisplayItems = new List<ResourceInfo>();

        public bool Diried { get; internal set; }

        public int ItemsCount { get => items.Count; }

        public IEnumerable<ComparableItem> Items { get => sortedItems.Values; }

        public List<ComparableContentHeader> Headers = new List<ComparableContentHeader>();

        public int AddedCount { get; set; } = 0;
        public int ChangedCount { get; set; } = 0;
        public int RemovedCount { get; set; } = 0;

        internal bool updateDisplayText;



        public void AddHeader(string name, object value)
        {
            Headers.Add(new ComparableContentHeader() { Name = name, Value = value });
        }


        public void Add(ComparableItem item)
        {
            sortedItems.Add(item.Key, item);
            items.Add(item.Key, item);
        }

        public ComparableItem FindComparableItem(ComparableItem findItem)
        {
            if (findItem == null)
                throw new ArgumentNullException(nameof(findItem));

            ComparableItem result;
            if (items.TryGetValue(findItem.Key, out result))
            {
                if (findItem.CanComparer(result))
                    return result;
            }

            return null;
        }

        public void Diry()
        {
            Diried = true;
        }

        public override string ToString()
        {
            return GetType().Name;
        }

    }
    class ComparableContentHeader
    {
        public string Name { get; set; }
        public object Value { get; set; }
    }

    public class ComparableItemExtend : ComparableItem
    {
        private string name;
        private string value;

        public string Name { get => name; set => name = value; }
        public string Value { get => value; set => this.value = value; }
    }

}
