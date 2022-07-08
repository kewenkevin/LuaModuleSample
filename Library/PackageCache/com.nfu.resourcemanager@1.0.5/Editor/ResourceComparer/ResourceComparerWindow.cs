using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.FileSystem;
using ND.Managers.ResourceMgr.Framework.Resource;
using ND.Managers.ResourceMgr.Runtime;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UIElements;
using Utility = ND.Managers.ResourceMgr.Framework.Utility;

namespace ND.Managers.ResourceMgr.Editor.Comparer
{
    /// <summary>
    /// 版本对比工具，支持(.dat, AssetBundle, AssetBundleManifest)
    /// </summary>
    class ResourceComparerWindow : UXMLWindow
    {

        private List<ComparableSource> sources;
        private List<ComparableContent> contents;
        private string filter;
        private ComparableStatus visiableStatus;
        private bool comparerChanged;
        private bool filterChanged;
        private bool columnWidthChanged;
        private bool expand = false;
        private int dragIndex = -1;
        private bool isDragAdd;
        private float columnWidth;
        static int numWidth = 25;
        [NonSerialized]
        VisualElement headerItemsRoot;

        [NonSerialized]
        VisualElement contentItemsRoot;

        private bool isUIDrired;
        int contentListLineExpandCount = 0;
        private ListView syncListScrollPos;

        [MenuItem(EditorUtilityx.AdvancedMenuPrefix + "Comparer")]
        public static void ShowWindow()
        {
            CreateWindow<ResourceComparerWindow>().Show();
        }

        private void OnEnable()
        {
            titleContent = new GUIContent("Resource Comparer");
            if (sources == null)
            {
                sources = new List<ComparableSource>();
            }

            if (contents == null)
            {
                contents = new List<ComparableContent>();
            }
            while (contents.Count < sources.Count)
                contents.Add(null);

            if (sources.Count == 0)
            {
                AddSource(null);
            }


            foreach (var src in sources)
            {
                src.Initalize();
                src.Diry();
            }
            columnWidthChanged = true;

        }


        protected override void Update()
        {
            if (sources.Count == 0)
            {
                AddSource(null);
            }

            for (int i = 0; i < sources.Count; i++)
            {
                var source = sources[i];
                if (source.Diried)
                {
                    Load(i);
                }
            }

            foreach (var content in contents)
            {
                if (content == null)
                    continue;
                if (filterChanged || content.Diried)
                {
                    Filter(content);
                }
            }
            filterChanged = false;


            foreach (var content in contents)
            {
                if (content == null)
                    continue;
                if (columnWidthChanged || content.Diried)
                {
                    content.updateDisplayText = true;
                }
            }
            columnWidthChanged = false;


            foreach (var content in contents)
            {
                if (content == null)
                    continue;
                if (content.Diried)
                {
                    content.Diried = false;
                    comparerChanged = true;
                    DiryUI();
                }
            }

            if (comparerChanged)
            {
                comparerChanged = false;
                Comparer();
                filterChanged = true;
                DiryUI();
            }

            if (isUIDrired && !filterChanged)
            {
                isUIDrired = false;
                OnUIChanged();
            }

            base.Update();

        }

        void DiryUI()
        {
            isUIDrired = true;
        }

        /// <summary>
        /// 进行比较
        /// </summary>
        void Comparer()
        {
            HashSet<ComparableItem> added = new HashSet<ComparableItem>();
            HashSet<ComparableItem> changed = new HashSet<ComparableItem>();
            HashSet<ComparableItem> removed = new HashSet<ComparableItem>();

            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                if (content == null)
                    continue;

                added.Clear();
                changed.Clear();
                removed.Clear();

                content.AddedCount = 0;
                content.ChangedCount = 0;
                content.RemovedCount = 0;
                foreach (var item in content.Items)
                {
                    item.Status = ComparableStatus.None;
                }

                for (int j = 0; j < contents.Count; j++)
                {
                    var comp = contents[j];
                    if (comp == null || comp == content)
                        continue;
                    Comparer(content, comp, added, changed, removed);
                }

                foreach (var item in content.Items)
                {
                    if (added.Contains(item))
                    {
                        item.Status = ComparableStatus.Added;
                        content.AddedCount++;
                    }
                    else if (changed.Contains(item))
                    {
                        item.Status = ComparableStatus.Changed;
                        content.ChangedCount++;
                    }
                }
            }

            UpdateDisplayIndex();

        }

        Dictionary<int, ComparableItem> indexes = new Dictionary<int, ComparableItem>();

        void UpdateDisplayIndex()
        {
            indexes.Clear();
            SortedDictionary<string, int> all = new SortedDictionary<string, int>();

            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                if (content == null)
                    continue;

                foreach (var item in content.Items)
                {
                    if (!all.ContainsKey(item.Key))
                    {
                        all.Add(item.Key, 0);
                    }
                }
            }

            int index = 0;
            foreach (var item in all.ToArray())
            {
                all[item.Key] = index++;
            }

            for (int i = 0; i < contents.Count; i++)
            {
                var content = contents[i];
                if (content == null)
                    continue;
                foreach (var item in content.Items)
                {
                    int index2 = all[item.Key];
                    item.DisplayIndex = index2;
                    if (!indexes.ContainsKey(index2))
                        indexes[index2] = item;
                }
                content.RemovedCount = all.Count - content.ItemsCount;
            }

        }

        void Comparer(ComparableContent content, ComparableContent comparer, HashSet<ComparableItem> added, HashSet<ComparableItem> changed, HashSet<ComparableItem> removed)
        {
            foreach (ComparableItem item in content.Items)
            {
                var compItem = FindComparerItem(comparer, item);
                if (compItem == null)
                {
                    added.Add(item);
                    continue;
                }
                if (!item.Equals(compItem))
                {
                    changed.Add(item);
                }
            }

        }

        ComparableItem FindComparerItem(ComparableContent content, ComparableItem findItem)
        {
            if (findItem == null)
                throw new ArgumentNullException(nameof(findItem));

            //foreach (var item in content.Items)
            //{
            //    if (findItem.CanComparer(item))
            //        return item;
            //}
            //return null;
            return content.FindComparableItem(findItem);
        }


        protected override void Load()
        {
            base.Load();

            var contentHeaders = rootVisualElement.Q("content_headers");
            headerItemsRoot = contentHeaders.Q("items");

            contentItemsRoot = rootVisualElement.Q("contents");

            var btnAdd = contentHeaders.Q<Button>("add");
            btnAdd.clicked += () =>
            {
                AddSource(null);
            };
            BindDragEvent(btnAdd);

            var toolbar = rootVisualElement.Q("toolbar");


            var txtFilter = toolbar.Q<ToolbarSearchField>("filter");
            txtFilter.value = filter;
            txtFilter.RegisterValueChangedCallback(o =>
            {
                if (o.newValue != filter)
                {
                    filter = o.newValue;
                    filterChanged = true;
                }
            });

            OnUIChanged();
        }

        VisualElement FindParent(VisualElement elem, string name)
        {
            var parent = elem;
            while (parent != null)
            {
                if (parent.name == name)
                    return parent;
                parent = parent.parent;
            }
            return null;
        }

        ContentUI GetUIData(VisualElement elem)
        {
            var elemData = FindParent(elem, "content_header");
            if (elemData == null)
            {
                elemData = FindParent(elem, "content");
            }
            ContentUI result = null;

            if (elemData != null)
            {
                result = (ContentUI)elemData.userData;
            }
            return result;
        }


        void OnUIChanged()
        {
            if (!IsLoaded || contents == null)
                return;

            for (int i = 0; i < sources.Count; i++)
            {
                if (i >= contentUIs.Count)
                {
                    AddContentUI();
                }

                var ui = contentUIs[i];
                ui.index = i;
                var content = contents[i];

                if (i == 0)
                {
                    columnWidth = ui.headerRoot.parent.style.width.value.value / sources.Count;
                }

                ui.headerDatas.Clear();
                ui.contentValues.Clear();
                if (content != null)
                {
                    foreach (var header in content.Headers)
                    {
                        var data = new BindingData(header.Name, header.Value?.ToString());
                        ui.headerDatas.Add(data);
                    }

                    //foreach (var item in content.Items)
                    //{
                    //    if (!item.Visiable)
                    //        continue;
                    //    var data = new UIData(item);
                    //    ui.contentValues.Add(data);
                    //}
                }

                UpdateHeaderUI(ui);
                UpdateContentUI(ui);
            }

        }

        string GetDisplayName(ComparableSource source)
        {
            string name = null;
            if (source.Source != null)
            {
                name = Path.GetFileName(source.Source);
            }

            if (string.IsNullOrEmpty(name))
                name = "*";
            if (source.OriginChanged)
                name = "*" + name;
            return name;
        }
        List<ContentUI> contentUIs = new List<ContentUI>();
        class ContentUI
        {
            public int index;

            public VisualElement headerRoot;
            public VisualElement header;
            public List<BindingData> headerDatas = new List<BindingData>();
            public VisualElement contentRoot;
            public VisualElement content;
            public Toolbar contentToolbar;
            public ListView contentList;
            public List<BindingData> contentValues = new List<BindingData>();


        }
        class BindingData
        {
            public int Depth;
            public string Label;
            public object Value;
            public int DataIndex;
            public int Index;
            public BindingData(object value)
            {
                this.Value = value;
            }
            public BindingData(string label, object value)
            {
                this.Label = label;
                this.Value = value;
            }
        }
        int contentListItemHeight;
        VisualElement AddContentUI()
        {
            ContentUI ui = new ContentUI();

            ui.headerRoot = LoadUXML(headerItemsRoot, "ResourceComparerWindow_Header.uxml");

            ui.header = ui.headerRoot.Q("content_header");
            var header = ui.header;

            header.Q<Button>("left_arrow")
               .clicked += () =>
               {
                   int index = ui.index;
                   if (index > 0)
                   {
                       SwapSource(index, index - 1);
                   }
               };
            header.Q<Button>("right_arrow")
              .clicked += () =>
              {
                  int index = ui.index;
                  if (index < sources.Count - 1)
                  {
                      SwapSource(index, index + 1);
                  }
              };
            header.Q<Button>("close")
              .clicked += () =>
              {
                  int index = ui.index;
                  if (index >= 0 && index < sources.Count)
                  {
                      RemoveSource(index);
                  }
              };

            header.Q<Button>("refresh")
                .clicked += () =>
                {
                    int index = ui.index;
                    Load(index);
                };

            header.Q<TextField>("source").RegisterValueChangedCallback(o =>
            {
                var source = sources[ui.index];
                if (source.Source != o.newValue)
                {
                    source.SetSource(o.newValue);
                    source.Diry();
                }
            });

            header.Q<Button>("select_file")
          .clicked += () =>
          {
              string path = EditorUtility.OpenFilePanel("Select Compare File", "", "");
              if (!string.IsNullOrEmpty(path))
              {
                  var source = sources[ui.index];
                  source.SetSource(path);
                  source.Diry();
              }
          };
            var headerList = header.Q<ListView>("list");
            headerList.selectionType = SelectionType.None;
            headerList.itemsSource = ui.headerDatas;

            headerList.makeItem = () =>
            {
                var container = new VisualElement();

                container.Add(new Label()
                {
                    name = "name"
                });

                container.Add(new Label()
                {
                    name = "value"
                });

                container.AddToClassList("list_item");
                return container;

                //var root = new VisualElement();
                //root.AddToClassList("list_item");
                //root.Add(container);
                //return root;
            };
            headerList.bindItem = (view, index) =>
            {
                var data = ui.headerDatas[index];
                view.Q<Label>("name").text = data.Label;
                view.Q<Label>("value").text = data.Value?.ToString();
            };

            ui.contentRoot = LoadUXML(contentItemsRoot, "ResourceComparerWindow_Content.uxml");
            ui.contentRoot.style.flexGrow = 1f;

            ui.content = ui.contentRoot.Q("content");
            ui.content.userData = ui;

            ui.contentToolbar = ui.headerRoot.Q<Toolbar>();

            ToolbarToggle tgl;
            tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_all");
            tgl.RegisterValueChangedCallback((e) =>
            {
                if (e.newValue)
                {
                    visiableStatus = ComparableStatus.None;
                }
                else
                {
                    visiableStatus = ComparableStatus.Added | ComparableStatus.Changed | ComparableStatus.Removed;
                }
                filterChanged = true;
            });

            tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_added");
            tgl.RegisterValueChangedCallback((e) =>
             {
                 if ((visiableStatus & ComparableStatus.Added) == ComparableStatus.Added)
                 {
                     visiableStatus &= ~ComparableStatus.Added;
                 }
                 else
                 {
                     visiableStatus |= ComparableStatus.Added;
                 }
                 filterChanged = true;
             });

            tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_changed");
            tgl.RegisterValueChangedCallback((e) =>
            {
                if ((visiableStatus & ComparableStatus.Changed) == ComparableStatus.Changed)
                {
                    visiableStatus &= ~ComparableStatus.Changed;
                }
                else
                {
                    visiableStatus |= ComparableStatus.Changed;
                }
                filterChanged = true;
            });

            tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_removed");
            tgl.RegisterValueChangedCallback((e) =>
            {
                if ((visiableStatus & ComparableStatus.Removed) == ComparableStatus.Removed)
                {
                    visiableStatus &= ~ComparableStatus.Removed;
                }
                else
                {
                    visiableStatus |= ComparableStatus.Removed;
                }
                filterChanged = true;
            });

            tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_expand");
            tgl.RegisterValueChangedCallback((e) =>
            {
                expand = e.newValue;
                DiryUI();
            });

            var contentList = ui.contentRoot.Q<ListView>("list");
            contentList.selectionType = SelectionType.None;
            contentList.itemsSource = ui.contentValues;
            contentListItemHeight = contentList.itemHeight;
            contentList.makeItem = () =>
            {
                var itemContainer = new VisualElement();
                itemContainer.AddToClassList("list_item");

                var lblNum = new Label();
                lblNum.name = "num";
                itemContainer.Add(lblNum);

                var v1 = new VisualElement();

                var lblValue = new Label();
                lblValue.name = "value";

                v1.Add(lblValue);

                var expandContainer = new VisualElement();
                expandContainer.AddToClassList("expand");
                v1.Add(expandContainer);

                itemContainer.Add(v1);
                return itemContainer;
            };
            contentList.bindItem = (view, index) =>
            {
                var data = ui.contentValues[index];
                var value = (KeyValuePair<ComparableItem, ComparableStatus>)data.Value;
                var item = value.Key;
                var status = value.Value;

                view.RemoveFromClassList("alternate");
                view.RemoveFromClassList("cmp_item_added");
                view.RemoveFromClassList("cmp_item_changed");
                view.RemoveFromClassList("cmp_item_removed");


                if (index % 2 == 1)
                {
                    view.AddToClassList("alternate");
                }
                switch (status)
                {
                    case ComparableStatus.Added:
                        view.AddToClassList("cmp_item_added");
                        break;
                    case ComparableStatus.Changed:
                        view.AddToClassList("cmp_item_changed");
                        break;
                    case ComparableStatus.Removed:
                        view.AddToClassList("cmp_item_removed");
                        break;
                }

                view.Q<Label>("num").text = data.DataIndex.ToString();


                var lblValue = view.Q<Label>("value");
                lblValue.text = item.Key;
                lblValue.tooltip = item.Key;


                var expandContainer = view.Q(null, "expand");
                expandContainer.Clear();

                if (expand && item.Extends != null && ((status & ComparableStatus.Removed) != ComparableStatus.Removed))
                {
                    foreach (var member in item.Extends)
                    {
                        var expandItem = new VisualElement();
                        expandItem.AddToClassList("expand_item");

                        expandItem.Add(new Label(member.Name) { name = "expand_name" });
                        expandItem.Add(new Label(member.Value) { name = "expand_value" });

                        expandContainer.Add(expandItem);
                    }
                }
            };
            contentList.Q<ScrollView>().verticalScroller.valueChanged += (v) =>
                {
                    if (syncListScrollPos == null)
                    {
                        syncListScrollPos = contentList;
                        UpdateContentScroll(v);
                        syncListScrollPos = null;
                    }
                };
            ui.contentList = contentList;

            BindDragEvent(ui.content);

            contentUIs.Add(ui);
            OnResize();
            return header;
        }


        #region Drag file

        void BindDragEvent(VisualElement elem)
        {
            elem.RegisterCallback<DragUpdatedEvent>(OnDragUpdatedEvent);
            elem.RegisterCallback<DragPerformEvent>(OnDragPerformEvent);
            elem.RegisterCallback<DragLeaveEvent>(OnDragLeaveEvent);
            elem.RegisterCallback<DragExitedEvent>(OnDragExitedEvent);
        }


        void OnDragUpdatedEvent(DragUpdatedEvent e)
        {
            var elem = e.currentTarget as VisualElement;
            if (elem != null)
            {
                if (DragAndDrop.paths.Length > 0)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Link;
                    elem.AddToClassList("drag_over");
                }
                else
                {
                    elem.RemoveFromClassList("drag_over");
                }
            }
        }

        void OnDragPerformEvent(DragPerformEvent e)
        {

            var elem = e.currentTarget as VisualElement;
            if (elem != null)
            {
                if (DragAndDrop.paths.Length > 0)
                {
                    var ui = GetUIData(elem);
                    int index;
                    if (ui != null)
                        index = ui.index;
                    else
                        index = sources.Count;
                    DragAndDrop.AcceptDrag();
                    string[] paths = DragAndDrop.paths;
                    for (int i = 0; i < paths.Length; i++)
                    {
                        if (index + i < sources.Count)
                        {
                            ReplaceSource(index + i, paths[i]);
                        }
                        else
                        {
                            AddSource(paths[i]);
                        }
                    }
                }
            }
        }

        void OnDragLeaveEvent(DragLeaveEvent e)
        {
            var elem = e.currentTarget as VisualElement;
            if (elem != null)
            {
                elem.RemoveFromClassList("drag_over");
            }
        }

        void OnDragExitedEvent(DragExitedEvent e)
        {
            var elem = e.currentTarget as VisualElement;
            if (elem != null)
            {
                elem.RemoveFromClassList("drag_over");
            }
        }

        #endregion


        void UpdateContentScroll(float value)
        {
            foreach (var ui in contentUIs)
            {
                if (ui.contentList == syncListScrollPos)
                    continue;
                ui.contentList.Q<ScrollView>().verticalScroller.value = value;
            }
        }

        void RemoveContentUI(int index)
        {
            var ui = contentUIs[index];
            ui.headerRoot.parent.Remove(ui.headerRoot);
            ui.contentRoot.parent.Remove(ui.contentRoot);
            contentUIs.RemoveAt(index);
        }

        void UpdateHeaderUI(ContentUI ui)
        {
            VisualElement headerUI = ui.header;
            ComparableSource source = sources[ui.index];

            var btnLeftArrow = headerUI.Q("left_arrow");
            var btnRightArrow = headerUI.Q("right_arrow");
            var lblTitle = headerUI.Q<Label>("header_title");

            headerUI.userData = ui;
            //ui.header.style.flexGrow = new StyleFloat(1f / sources.Count);

            ui.headerRoot.style.width = columnWidth;

            if (ui.index > 0)
            {
                btnLeftArrow.visible = true;
            }
            else
            {
                btnLeftArrow.visible = false;
            }

            if (ui.index < sources.Count - 1)
            {
                btnRightArrow.visible = true;
            }
            else
            {
                btnRightArrow.visible = false;
            }

            lblTitle.text = GetDisplayName(source);

            headerUI.Q<TextField>("source").value = source.Source;

            var lblError = headerUI.Q<Label>("error");
            if (string.IsNullOrEmpty(source.Error))
            {

                lblError.text = null;
                lblError.visible = false;

            }
            else
            {
                lblError.text = source.Error;
                lblError.visible = true;
            }

            var list = ui.header.Q<ListView>("list");
            list.style.height = list.itemHeight * ui.headerDatas.Count;
            list.Refresh();

            var content = contents[ui.index];
            if (content != null)
            {
                if (!ui.contentToolbar.enabledSelf)
                {
                    ui.contentToolbar.SetEnabled(true);
                }

                ToolbarToggle tgl;

                tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_all");
                tgl.text = $"All ({content.ItemsCount})";
                if (visiableStatus == ComparableStatus.None)
                    tgl.SetValueWithoutNotify(true);
                else
                    tgl.SetValueWithoutNotify(false);

                tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_added");
                tgl.text = $"Added ({content.AddedCount})";
                if ((visiableStatus & ComparableStatus.Added) == ComparableStatus.Added)
                    tgl.SetValueWithoutNotify(true);
                else
                    tgl.SetValueWithoutNotify(false);

                tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_changed");
                tgl.text = $"Changed ({content.ChangedCount})";
                if ((visiableStatus & ComparableStatus.Changed) == ComparableStatus.Changed)
                    tgl.SetValueWithoutNotify(true);
                else
                    tgl.SetValueWithoutNotify(false);

                tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_removed");
                tgl.text = $"Removed ({content.RemovedCount})";
                if ((visiableStatus & ComparableStatus.Removed) == ComparableStatus.Removed)
                    tgl.SetValueWithoutNotify(true);
                else
                    tgl.SetValueWithoutNotify(false);

                tgl = ui.contentToolbar.Q<ToolbarToggle>("toolbar_expand");
                tgl.SetValueWithoutNotify(expand);

            }
            else
            {
                if (ui.contentToolbar.enabledSelf)
                {
                    ui.contentToolbar.SetEnabled(false);
                }
            }

        }


        protected override void OnResize()
        {
            foreach (var ui in contentUIs)
            {
                ui.contentRoot.style.width = new Length(1f / sources.Count * 100, LengthUnit.Percent);
            }
        }

        void UpdateContentUI(ContentUI ui)
        {
            var msgBox = ui.contentRoot.Q("content_msg_box");
            var contentBox = ui.contentRoot.Q("list_box");

            ui.contentValues.Clear();
            var content = contents[ui.index];
            if (content == null)
            {
                msgBox.visible = true;
                contentBox.visible = false;
                return;
            }

            msgBox.visible = false;
            contentBox.visible = true;

            int displayIndex = 0;
            int currentIndex = 0;
            contentListLineExpandCount = 0;

            Action<ComparableItem, int, bool> AddItem = (item, index, removed) =>
                {
                    KeyValuePair<ComparableItem, ComparableStatus> item2;
                    if (removed)
                    {
                        item2 = new KeyValuePair<ComparableItem, ComparableStatus>(item, ComparableStatus.Removed);
                    }
                    else
                    {
                        item2 = new KeyValuePair<ComparableItem, ComparableStatus>(item, item.Status);
                    }
                    BindingData data = new BindingData(item2);
                    data.DataIndex = displayIndex;
                    data.Index = currentIndex;
                    currentIndex++;
                    if (expand && item.Extends != null)
                    {
                        contentListLineExpandCount = Mathf.Max(contentListLineExpandCount, item.Extends.Count);
                    }
                    ui.contentValues.Add(data);
                };

            Action<int> DrawRemoved = (_displayIndex) =>
            {
                ComparableItem removedItem;

                if (!indexes.TryGetValue(_displayIndex, out removedItem))
                {
                    return;
                }
                if (visiableStatus != ComparableStatus.None && ((visiableStatus & ComparableStatus.Removed) != ComparableStatus.Removed))
                    return;

                AddItem(removedItem, _displayIndex, true);
            };
            foreach (var item in content.Items)
            {
                if (!item.Visiable)
                {
                    displayIndex++;
                    continue;
                }

                if (item.DisplayIndex > displayIndex)
                {
                    //填充空行进行对齐
                    while (item.DisplayIndex > displayIndex)
                    {
                        DrawRemoved(displayIndex);
                        displayIndex++;
                    }
                }


                AddItem(item, displayIndex, false);
                displayIndex++;
            }

            int total = indexes.Count;
            if (displayIndex < total)
            {
                //填充空行进行对齐
                while (displayIndex < total)
                {
                    DrawRemoved(displayIndex);
                    displayIndex++;
                }
            }

            var list = ui.contentRoot.Q<ListView>("list");
            list.itemHeight = (1 + contentListLineExpandCount) * contentListItemHeight;

            //通知ListView刷新
            var sv = list.Q<ScrollView>();
            var v = sv.verticalScroller.value;
            sv.verticalScroller.value = 0f;
            sv.verticalScroller.value = v;

            list.Refresh();
        }



        public void AddSource(string path)
        {
            var source = new ComparableSource();

            source.SetSource(path);
            source.Diry();
            sources.Add(source);
            contents.Add(null);


            DiryUI();
        }

        public void ReplaceSource(int index, string path)
        {
            if (index < 0 || index >= sources.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var source = sources[index];
            source.SetSource(path);
            source.Diry();
            DiryUI();
        }

        public void RemoveSource(int index)
        {
            if (index < 0 || index >= sources.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var source = sources[index];
            sources.RemoveAt(index);
            source.Dispose();
            contents.RemoveAt(index);

            RemoveContentUI(index);

            DiryUI();
        }

        public void SwapSource(int from, int to)
        {
            if (from < 0 || from >= sources.Count)
                throw new ArgumentOutOfRangeException(nameof(from));
            if (to < 0 || to >= sources.Count)
                throw new ArgumentOutOfRangeException(nameof(to));

            if (from == to)
                return;

            var a = sources[from];
            sources[from] = sources[to];
            sources[to] = a;

            var b = contents[from];
            contents[from] = contents[to];
            contents[to] = b;
            DiryUI();
        }

        public void Load(int index)
        {
            if (index < 0 || index >= sources.Count)
                throw new ArgumentOutOfRangeException(nameof(index));

            var source = sources[index];
            source.Diried = false;
            source.OriginChanged = false;
            if (source.Request != null)
            {
                source.Request.webRequest.Dispose();
                source.Request = null;
            }
            ComparableContent content = null;
            if (string.IsNullOrEmpty(source.Source))
            {

                contents[index] = null;
                comparerChanged = true;
                DiryUI();
                return;
            }

            string path = source.LocalPath;
            if (string.IsNullOrEmpty(path))
            {
                UnityWebRequest request = UnityWebRequest.Get(source.Source);
                source.Request = request.SendWebRequest();
                source.Request.completed += (req) =>
                {
                    if (string.IsNullOrEmpty(request.error))
                    {
                        byte[] bytes;
                        bytes = request.downloadHandler.data;
                        request.Dispose();
                        source.Request = null;
                        content = Load(index, path, bytes);
                    }
                    else
                    {

                        contents[index] = null;
                    }
                    comparerChanged = true;
                    DiryUI();
                };
            }
            else
            {
                content = Load(index, path, null);
                comparerChanged = true;
            }
            DiryUI();
        }


        private ComparableContent Load(int index, string path, byte[] bytes)
        {
            var source = sources[index];
            source.Error = null;
            var fsm = GameFrameworkEntry.GetModule<IFileSystemManager>();
            fsm.SetFileSystemHelper(new DefaultFileSystemHelper());
            Utility.ZipUtil.SetZipHelper(new DefaultZipHelper());

            ComparableContent content = null;

            if (!string.IsNullOrEmpty(path))
            {
                if (bytes == null)
                {
                    if (File.Exists(path))
                    {
                        bytes = File.ReadAllBytes(path);
                    }
                }
            }

            if (bytes == null)
            {
                source.Error = "No data";
                return null;
            }

            bool isZip = false;

            try
            {
                var tmp = Utility.ZipUtil.Decompress(bytes);
                if (tmp != null)
                    bytes = tmp;
                isZip = true;
            }
            catch
            {
            }

            if (content == null)
            {
                try
                {
                    var ver = ResourceInfo.GetPackedVersionSerializer().Deserialize(bytes);
                    content = new LocalVersionListContent(ver);
                }
                catch { }
            }
            if (content == null)
            {
                try
                {
                    var ver = ResourceInfo.GetReadWriteVersionSerializer().Deserialize(bytes);
                    content = new LocalVersionListContent(ver);
                }
                catch { }
            }

            if (content == null)
            {
                try
                {
                    var ver = ResourceInfo.GetPackageVersionSerializer().Deserialize(bytes);
                    content = new PackageVersionListContent(ver);
                }
                catch { }
            }

            if (content == null)
            {
                try
                {
                    var ver = ResourceInfo.GetFullVersionSerializer().Deserialize(bytes);
                    content = new UpdatableVersionListContent(ver);
                }
                catch { }
            }

            if (content == null)
            {
                try
                {
                    var fs = fsm.LoadFileSystem(path, FileSystemAccess.Read);
                    content = new FileSystemContent(fs);
                }
                catch (Exception ex)
                {
                }
            }
            if (content == null)
            {
                AssetBundle ab = null;
                try
                {
                    ab = AssetBundle.LoadFromMemory(bytes);
                    if (ab != null)
                    {
                        AssetBundleManifest manifest = ab.LoadAllAssets<AssetBundleManifest>().FirstOrDefault();
                        if (manifest != null)
                        {
                            content = new AssetBundleManifestContent(manifest);
                        }
                        else
                        {
                            content = new AssetBundleContent(ab);
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                finally
                {
                    if (ab != null)
                    {
                        ab.Unload(true);
                        ab = null;
                    }
                }
            }

            if (content != null)
            {
                if (isZip)
                {
                    content.AddHeader("Zip", isZip);
                }
            }


            foreach (var fs in fsm.GetAllFileSystems().ToArray())
            {
                fsm.DestroyFileSystem(fs, false);
            }

            while (contents.Count < sources.Count)
                contents.Add(null);

            contents[index] = content;

            if (content != null)
            {
                content.Diry();
            }
            else
            {
                source.Error = "Unknown format";
            }

            return content;
        }


        private void Filter(ComparableContent content)
        {
            foreach (var item in content.Items)
            {
                item.Visiable = true;
            }



            if (visiableStatus != ComparableStatus.None)
            {
                foreach (var item in content.Items)
                {
                    if (item.Visiable && (item.Status & visiableStatus) == 0)
                    {
                        item.Visiable = false;
                    }
                }
            }

            //搜索过滤
            if (!string.IsNullOrEmpty(filter))
            {
                Regex include = null;
                include = new Regex(filter, RegexOptions.IgnoreCase);
                foreach (var item in content.Items)
                {
                    if (item.Visiable && !include.IsMatch(item.DisplayText))
                    {
                        item.Visiable = false;
                    }
                }
            }


            DiryUI();
        }


        #region CompareContent

        class UpdatableVersionListContent : ComparableContent
        {
            public UpdatableVersionListContent(UpdatableVersionList versionList)
            {

                AddHeader("ApplicableGameVersion", versionList.ApplicableGameVersion);
                AddHeader("InternalResourceVersion", versionList.InternalResourceVersion.ToString());

                foreach (var resInfo in versionList.GetResourceInfos())
                {
                    ComparableItem item = new ComparableItem(resInfo.FullName, resInfo.HashCode.ToString("x8"));
                    item.AddExtend("Variant", resInfo.Variant);
                    item.AddExtend("Extension", resInfo.Extension);
                    item.AddExtend("HashCode", resInfo.HashCode.ToString("x8"));
                    item.AddExtend("Length", ((LoadType)resInfo.LoadType).ToString());
                    item.AddExtend("LoadType", resInfo.Variant);
                    item.AddExtend("FileSystem", resInfo.FileSystem);
                    item.AddExtend("ZipLength", resInfo.ZipLength.ToString());
                    item.AddExtend("ZipHashCode", resInfo.ZipHashCode.ToString("x8"));
                    Add(item);
                }
            }
        }
        class LocalVersionListContent : ComparableContent
        {
            public LocalVersionListContent(LocalVersionList versionList)
            {

                foreach (var resInfo in versionList.GetResourceInfos())
                {
                    ComparableItem item = new ComparableItem(resInfo.FullName, resInfo.HashCode.ToString("x8"));
                    item.AddExtend("Variant", resInfo.Variant);
                    item.AddExtend("Extension", resInfo.Extension);
                    item.AddExtend("HashCode", resInfo.HashCode.ToString("x8"));
                    item.AddExtend("Length", ((LoadType)resInfo.LoadType).ToString());
                    item.AddExtend("LoadType", resInfo.Variant);
                    item.AddExtend("FileSystem", resInfo.FileSystem);
                    Add(item);
                }
            }
        }


        class PackageVersionListContent : ComparableContent
        {
            public PackageVersionListContent(PackageVersionList versionList)
            {

                AddHeader("ApplicableGameVersion", versionList.ApplicableGameVersion);
                AddHeader("InternalResourceVersion", versionList.InternalResourceVersion.ToString());

                foreach (var resInfo in versionList.GetResourceInfos())
                {
                    ComparableItem item = new ComparableItem(resInfo.FullName, resInfo.HashCode.ToString("x8"));
                    item.AddExtend("Variant", resInfo.Variant);
                    item.AddExtend("Extension", resInfo.Extension);
                    item.AddExtend("HashCode", resInfo.HashCode.ToString("x8"));
                    item.AddExtend("Length", ((LoadType)resInfo.LoadType).ToString());
                    item.AddExtend("LoadType", resInfo.Variant);
                    item.AddExtend("FileSystem", resInfo.FileSystem);
                    Add(item);
                }
            }
        }

        class FileSystemContent : ComparableContent
        {
            public FileSystemContent(IFileSystem fileSystem)
            {
                foreach (var fileInfo in fileSystem.GetAllFileInfos())
                {
                    ComparableItem item = new ComparableItem(fileInfo.Name);
                    item.AddExtend("Offset", fileInfo.Offset.ToString());
                    item.AddExtend("Length", fileInfo.Length.ToString());
                    Add(item);
                }
            }
        }
        class AssetBundleManifestContent : ComparableContent
        {
            public AssetBundleManifestContent(AssetBundleManifest manifest)
            {
                foreach (var abName in manifest.GetAllAssetBundles())
                {
                    ComparableItem item = new ComparableItem(abName);
                    Add(item);
                }
            }
        }
        class AssetBundleContent : ComparableContent
        {
            public AssetBundleContent(AssetBundle assetBundle)
            {

                foreach (var assetName in assetBundle.GetAllAssetNames())
                {
                    var asset = assetBundle.LoadAsset(assetName);

                    string key = assetName;

                    ComparableItem item = new ComparableItem(key);
                    item.AddExtend("Type", asset.GetType().Name);
                    Add(item);
                }
            }
        }

        #endregion
    }



}