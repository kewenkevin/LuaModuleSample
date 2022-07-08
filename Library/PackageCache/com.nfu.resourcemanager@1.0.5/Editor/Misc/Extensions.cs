using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ND.Managers.ResourceMgr.Editor.ResourceTools;
using ND.Managers.ResourceMgr.Framework;
using ND.Managers.ResourceMgr.Framework.Resource;
using UnityEditor;
using UnityEngine;

namespace ND.Managers.ResourceMgr.Editor
{
    internal static class Extensions
    {

        public static GUIContent EllipsisLabel = new GUIContent("…");
        private static LinkedList<IEnumerator> routines;

        /// <summary>
        /// 设置节点属性值
        /// </summary>
        public static void SetAttribute(this XmlNode node, string attributeName, string value)
        {
            var attr = node.Attributes.GetNamedItem(attributeName) as XmlAttribute;
            if (attr == null)
            {
                attr = node.OwnerDocument.CreateAttribute(attributeName);
                node.Attributes.Append(attr);
            }
            attr.Value = value;
        }

        /// <summary>
        /// 获取节点属性值
        /// </summary>
        public static string GetAttribute(this XmlNode node, string attributeName, string defaultValue = null)
        {
            string value;
            var attr = node.Attributes.GetNamedItem(attributeName) as XmlAttribute;
            if (attr == null)
            {
                value = attr.Value;
            }
            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        public static XmlNode FindChildNode(this XmlNode node, string nodeName)
        {
            foreach (XmlNode child in node.ChildNodes)
            {
                if (child.Name == nodeName)
                    return child;
            }
            return null;
        }

        /// <summary>
        /// 设置节点值
        /// </summary>
        public static void SetNodeValue(this XmlNode node, string nodeName, string value)
        {
            var valueNode = FindChildNode(node, nodeName);
            if (valueNode == null)
            {
                valueNode = node.OwnerDocument.CreateElement(nodeName);
                node.AppendChild(valueNode);
            }
            valueNode.InnerText = value;
        }

        /// <summary>
        /// 获取节点值
        /// </summary>
        public static T GetNodeValue<T>(this XmlNode node)
        {
            string str = node.InnerText;
            System.Type type = typeof(T);
            object result = default(T);
            if (type == typeof(string))
            {
                result = str;
            }
            else if (type == typeof(float))
            {
                float n;
                if (float.TryParse(str, out n))
                    result = n;
            }
            else if (type == typeof(int))
            {
                int n;
                if (int.TryParse(str, out n))
                    result = n;
            }
            else if (type == typeof(bool))
            {
                bool n;
                if (bool.TryParse(str, out n))
                    result = n;
            }
            return (T)result;
        }

        /// <summary>
        /// 读取数组，子节点值转数组
        /// </summary>
        public static T[] ReadArray<T>(this XmlNode parent, Func<XmlNode, T> parse)
        {
            List<T> result = new List<T>();
            var nodes = parent.ChildNodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                result.Add(parse(nodes[i]));
            }
            return result.ToArray();
        }
        /// <summary>
        /// 写入数组
        /// </summary>
        public static void WriteArray(this XmlNode parent, string itemName, IEnumerable values)
        {
            foreach (var item in values)
            {
                var itemNode = parent.OwnerDocument.CreateElement(itemName);
                itemNode.InnerText = item.ToString();
                parent.AppendChild(itemNode);
            }
        }

        /// <summary>
        /// 创建并添加节点
        /// </summary>
        public static XmlElement AppendChild(this XmlNode parent, string name)
        {
            var elem = parent.OwnerDocument.CreateElement(name);
            parent.AppendChild(elem);
            return elem;
        }

        public static GUIContent MenuLabel = new GUIContent("◥");
        public static void RightMenuButton<T>(Func<T, GenericMenu> createMenu, T userData)
        {
            using (new GUILayout.HorizontalScope())
            {
                GUILayout.FlexibleSpace();

                GUIStyle style = new GUIStyle();
                style.padding.left = 5;
                style.padding.right = 5;
                style.fixedHeight = EditorGUIUtility.singleLineHeight - 6;

                MenuButton(MenuLabel, style, createMenu, userData);
            }
        }

        public static void MenuButton<T>(GUIContent label, GUIStyle style, Func<T, GenericMenu> createMenu, T userData, params GUILayoutOption[] options)
        {
            var old = GUI.changed;

            if (GUILayout.Button(label, style, options))
            {
                createMenu(userData).ShowAsContext();
            }
            GUI.changed = old;
        }

        /// <summary>
        /// 数组字段
        /// </summary>
        public static IList<T> ArrayField<T>(this GUIContent label, IList<T> list, Func<T, int, T> onGUI, bool initExpand = false, GUIStyle itemStyle = null, Func<T> createInstance = null, Func<GenericMenu> createMenu = null, Func<GenericMenu, GenericMenu> itemMenu = null, bool handleDelete = false, bool useFoldoutHeader = true)
        {
            int crlId = GUIUtility.GetControlID(typeof(ArrayFieldState).GetHashCode(), FocusType.Passive);
            var state = (ArrayFieldState)GUIUtility.GetStateObject(typeof(ArrayFieldState), crlId);
            if (!state.initialized)
            {
                state.initialized = true;
            }
            if (state.changed)
            {
                state.changed = false;
                list = (IList<T>)state.value;
                GUI.changed = true;
                EditorGUIUtility.editingTextField = false;
            }

            using (var foldout = new EditorGUILayoutx.FoldoutHeaderGroupScope(label, initExpand: initExpand, menuAction: (r) =>
             {
                 GenericMenu menu = createMenu?.Invoke();
                 if (menu != null)
                 {
                     menu.ShowAsContext();
                 }
                 else
                 {
                     menu = new GenericMenu();
                     menu.AddItem(new GUIContent("Add"), false, () =>
                     {
                         T instance;
                         if (createInstance != null)
                             instance = createInstance();
                         else
                         {
                             if (typeof(T) == typeof(string))
                                 instance = (T)(object)string.Empty;
                             else
                                 instance = Activator.CreateInstance<T>();
                         }
                         T[] array = list as T[];

                         if (array != null)
                         {
                             ArrayUtility.Insert(ref array, array.Length, instance);
                             list = array;
                         }
                         else
                         {
                             list.Add(instance);
                         }
                         state.value = list;
                         state.changed = true;

                     });
                     menu.ShowAsContext();
                 }
             }))
            {
                if (foldout.Visiable)
                {

                    for (int i = 0; i < list.Count; i++)
                    {
                        var item = list[i];
                        //using (new IndentLevelVerticalScope(itemStyle))
                        {
                            if (!handleDelete)
                            {
                                RightMenuButton((itemIndex) =>
                                {
                                    GenericMenu menu = new GenericMenu();

                                    if (itemIndex > 0)
                                    {
                                        menu.AddItem(new GUIContent("Move Up"), false, () =>
                                        {
                                            var tmp = list[itemIndex];
                                            list[itemIndex] = list[itemIndex - 1];
                                            list[itemIndex - 1] = tmp;
                                            state.value = list;
                                            state.changed = true;
                                        });
                                    }
                                    else
                                    {
                                        menu.AddDisabledItem(new GUIContent("Move Up"));
                                    }
                                    if (itemIndex < list.Count - 1)
                                    {
                                        menu.AddItem(new GUIContent("Move Down"), false, () =>
                                        {
                                            var tmp = list[itemIndex];
                                            list[itemIndex] = list[itemIndex + 1];
                                            list[itemIndex + 1] = tmp;
                                            state.value = list;
                                            state.changed = true;
                                        });
                                    }
                                    else
                                    {
                                        menu.AddDisabledItem(new GUIContent("Move Down"));
                                    }

                                    menu.AddItem(new GUIContent("Delete"), false, () =>
                                    {
                                        T[] array = list as T[];
                                        if (array != null)
                                        {
                                            ArrayUtility.RemoveAt(ref array, itemIndex);
                                            list = array;
                                        }
                                        else
                                        {
                                            list.RemoveAt(itemIndex);
                                        }
                                        state.value = list;
                                        state.changed = true;
                                    });

                                    if (itemMenu != null)
                                        menu = itemMenu(menu);

                                    return menu;
                                }, i);
                            }
                            item = onGUI(item, i);
                            list[i] = item;
                        }
                    }

                }
            }


            return list;
        }

        [Serializable]
        class ArrayFieldState
        {
            public bool initialized;
            public int addIndex = -1;
            public int removeIndex = -1;
            public bool changed = false;
            public object value;
        }


        public static bool ItemsEquals(this IEnumerable source, IEnumerable compare)
        {
            if (source == null)
            {
                if ((compare != null) && compare.GetEnumerator().MoveNext())
                {
                    return false;
                }
                return true;
            }
            if (compare == null)
            {
                if ((source != null) && source.GetEnumerator().MoveNext())
                {
                    return false;
                }
                return true;
            }

            return ItemsEquals(source.GetEnumerator(), compare.GetEnumerator());
        }
        public static bool ItemsEquals(this IEnumerator source, IEnumerator compare)
        {
            while (source.MoveNext())
            {
                if (!compare.MoveNext())
                    return false;

                if (!object.Equals(source.Current, compare.Current))
                    return false;
            }
            return !compare.MoveNext();
        }
        public static bool ItemsEquals<T>(this T[] source, T[] compare)
        {
            if (source == null)
            {
                if ((compare != null) && (compare.Length != 0))
                {
                    return false;
                }
                return true;
            }
            if (compare == null)
            {
                if ((source != null) && (source.Length != 0))
                {
                    return false;
                }
                return true;
            }
            int len = source.Length;
            if (len != compare.Length)
                return false;

            if (len != 0)
            {
                for (int i = 0; i < len; i++)
                {
                    if (!object.Equals(source[i], compare[i]))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// 选择文件夹字段，返回文件夹路径
        /// </summary>
        public static string FolderField(this GUIContent label, string folder, string title, string relativePath = null, char? directorySeparator = null, params GUILayoutOption[] options)
        {
            using (new GUILayout.HorizontalScope())
            {
                EditorGUILayout.PrefixLabel(label);
                var oldIndentLevel = EditorGUI.indentLevel;
                EditorGUI.indentLevel = 0;
                folder = FolderField(folder, title, relativePath: relativePath, style: "textfield", directorySeparator, options);
                EditorGUI.indentLevel = oldIndentLevel;
            }
            return folder;
        }

        public static string FolderField(string folder, string title, string relativePath = null, GUIStyle style = null, char? directorySeparator = null, params GUILayoutOption[] options)
        {
            if (style == null) style = "textfield";
            GUIContent content = new GUIContent(folder ?? string.Empty);
            Rect rect = GUILayoutUtility.GetRect(content, style, options);
            return FolderField(rect, folder, title, relativePath: relativePath, style: style, directorySeparator, options);
        }


        public static string FolderField(Rect rect, string folder, string title, string relativePath = null, GUIStyle style = null, char? directorySeparator = null, params GUILayoutOption[] options)
        {
            if (style == null)
                style = "textfield";
            float ellipsisLabelWidth = Styles.Ellipsis.CalcSize(EllipsisLabel).x;
            folder = EditorGUI.TextField(new Rect(rect.x, rect.y, rect.width - ellipsisLabelWidth, rect.height), folder ?? string.Empty, style);
            if (GUI.Button(new Rect(rect.xMax - ellipsisLabelWidth, rect.y, ellipsisLabelWidth, rect.height), EllipsisLabel, Styles.Ellipsis))
            {
                string newPath = EditorUtility.OpenFolderPanel(title, folder, "");
                if (!string.IsNullOrEmpty(newPath))
                {
                    string result;
                    if (!string.IsNullOrEmpty(relativePath) && newPath.ToRelativePath(relativePath, out result))
                    {
                        newPath = result;
                    }

                    if (!string.Equals(folder, newPath, StringComparison.InvariantCultureIgnoreCase))
                    {
                        folder = newPath;
                        if (directorySeparator != null)
                        {
                            folder = folder.Replace('/', directorySeparator.Value)
                                .Replace('\\', directorySeparator.Value);
                        }
                        GUIUtility.keyboardControl = -1;
                        GUI.changed = true;
                    }
                }
            }

            return folder;
        }

        public class Styles
        {
            static GUIStyle ellipsis;
            public static GUIStyle Ellipsis
            {
                get
                {
                    if (ellipsis == null)
                    {
                        ellipsis = new GUIStyle("label");
                    }
                    return ellipsis;
                }
            }
        }

        /// <summary>
        /// 获取相对路径
        /// </summary>
        public static bool ToRelativePath(this string path, string relativeTo, out string result)
        {
            string fullRelativeTo = Path.GetFullPath(relativeTo).Trim();
            string fullPath = Path.GetFullPath(path).Trim();

            if (fullPath.EndsWith("/") || fullPath.EndsWith("\\"))
                fullPath = fullPath.Substring(0, fullPath.Length - 1);
            if (fullRelativeTo.EndsWith("/") || fullRelativeTo.EndsWith("\\"))
                fullRelativeTo = fullRelativeTo.Substring(0, fullRelativeTo.Length - 1);

            string[] relativeToParts = fullRelativeTo.Split('/', '\\');
            string[] fullPathParts = fullPath.Split('/', '\\');
            int index = -1;

            if (fullPathParts.Length <= 1)
            {
                result = path;
                return false;
            }

            if (!string.Equals(fullPathParts[0], relativeToParts[0], StringComparison.InvariantCultureIgnoreCase))
            {
                result = path;
                return false;
            }


            for (int i = 0; i < fullPathParts.Length && i < relativeToParts.Length; i++)
            {
                if (!string.Equals(fullPathParts[i], relativeToParts[i], StringComparison.InvariantCultureIgnoreCase))
                    break;
                index = i;
            }

            result = "";
            for (int i = index + 1; i < relativeToParts.Length; i++)
            {
                if (result.Length > 0)
                    result += Path.DirectorySeparatorChar;
                result += "..";
            }
            for (int i = index + 1; i < fullPathParts.Length; i++)
            {
                if (result.Length > 0)
                    result += Path.DirectorySeparatorChar;
                result += fullPathParts[i];
            }
            return true;
        }
        public static string ToRelativePath(this string path, string relativeTo)
        {
            string result;
            if (ToRelativePath(path, relativeTo, out result))
                return result;
            return path;
        }

        public static T Deserialize<T>(this GameFrameworkSerializer<T> serializer, byte[] bytes)
        {
            T versionList;

            using (MemoryStream ms = new MemoryStream(bytes))
            {
                versionList = serializer.Deserialize(ms);
            }
            return versionList;
        }



        public static void StartCoroutine(this IEnumerator routine)
        {
            if (!routine.MoveNext())
                return;
            if (routines == null)
            {
                routines = new LinkedList<IEnumerator>();
            }
            routines.AddLast(routine);

            EditorApplication.delayCall -= CoroutineUpdate;
            EditorApplication.delayCall += CoroutineUpdate;
        }

        private static void CoroutineUpdate()
        {
            var current = routines.First;
            while (current != null)
            {
                if (!current.Value.MoveNext())
                {
                    var next = current.Next;
                    routines.Remove(current);
                    current = next;
                    continue;
                }
                current = current.Next;
            }

            if (routines.First != null)
            {
                EditorApplication.delayCall += CoroutineUpdate;
            }
        }

        public static T Deserialize<T>(this GameFrameworkSerializer<T> serializer, string filePath, bool decompress = false)
        {
            byte[] bytes = File.ReadAllBytes(filePath);
            if (decompress)
            {
                bytes = Utility.ZipUtil.Decompress(bytes);
            }
            return serializer.Deserialize(bytes);
        }


        public static ResourceInfo[] GetResourceInfos(this LocalVersionList versionList)
        {
            Dictionary<LocalVersionList.Resource, string> fileSystems = new Dictionary<LocalVersionList.Resource, string>();
            var allResources = versionList.GetResources();
            foreach (var fs in versionList.GetFileSystems())
            {
                foreach (var resIndex in fs.GetResourceIndexes())
                {
                    fileSystems[allResources[resIndex]] = fs.Name;
                }
            }
            string fileSystem;
            List<ResourceInfo> result = new List<ResourceInfo>();
            foreach (var resource in allResources)
            {
                fileSystems.TryGetValue(resource, out fileSystem);
                result.Add(new ResourceInfo(resource.Name, resource.Variant, resource.Extension, fileSystem, (LoadType)resource.LoadType, resource.Length, resource.HashCode));
            }
            return result.ToArray();
        }

        public static ResourceInfo[] GetResourceInfos(this PackageVersionList versionList)
        {
            Dictionary<PackageVersionList.Resource, string> fileSystems = new Dictionary<PackageVersionList.Resource, string>();
            var allResources = versionList.GetResources();
            var allAssets = versionList.GetAssets();

            foreach (var fs in versionList.GetFileSystems())
            {
                foreach (var resIndex in fs.GetResourceIndexes())
                {
                    fileSystems[allResources[resIndex]] = fs.Name;
                }
            }
            string fileSystem;
            List<ResourceInfo> result = new List<ResourceInfo>();
            foreach (var resource in allResources)
            {
                fileSystems.TryGetValue(resource, out fileSystem);
                var newResource = new ResourceInfo(resource.Name, resource.Variant, resource.Extension, fileSystem, (LoadType)resource.LoadType, resource.Length, resource.HashCode);

                foreach (var assetIndex in resource.GetAssetIndexes())
                {
                    var asset = allAssets[assetIndex];
                    var newAsset = Asset.CreateByAssetPath(asset.Name, newResource.Resource);
                    var depAssetIndexes = asset.GetDependencyAssetIndexes();
                    string[] depAssetPaths = new string[depAssetIndexes.Length];
                    for (int i = 0; i < depAssetIndexes.Length; i++)
                    {
                        var depAsset = allAssets[depAssetIndexes[i]];
                        depAssetPaths[i] = depAsset.Name;
                    }
                    newAsset.DependencyAssetPaths = depAssetPaths;

                    bool isScene = asset.Name.EndsWith(".unity");
                    newResource.Resource.AssignAsset(newAsset, isScene);
                }

                result.Add(newResource);
            }
            return result.ToArray();
        }

        public static ResourceInfo[] GetResourceInfos(this UpdatableVersionList versionList)
        {
            Dictionary<UpdatableVersionList.Resource, string> fileSystems = new Dictionary<UpdatableVersionList.Resource, string>();
            var allResources = versionList.GetResources();
            var allAssets = versionList.GetAssets();
            foreach (var fs in versionList.GetFileSystems())
            {
                foreach (var resIndex in fs.GetResourceIndexes())
                {
                    fileSystems[allResources[resIndex]] = fs.Name;
                }
            }
            
            string fileSystem;
            List<ResourceInfo> result = new List<ResourceInfo>();
            foreach (var resource in allResources)
            {
                fileSystems.TryGetValue(resource, out fileSystem);
                var newResource = new ResourceInfo(resource.Name, resource.Variant, resource.Extension, fileSystem, (LoadType)resource.LoadType, resource.Length, resource.HashCode);
                newResource.ZipLength = resource.ZipLength;
                newResource.ZipHashCode = resource.ZipHashCode;

                foreach (var assetIndex in resource.GetAssetIndexes())
                {
                    var asset = allAssets[assetIndex];
                    var newAsset = Asset.CreateByAssetPath(asset.Name, newResource.Resource);
                    var depAssetIndexes=asset.GetDependencyAssetIndexes();
                    string[] depAssetPaths = new string[depAssetIndexes.Length];
                    for (int i = 0; i < depAssetIndexes.Length; i++)
                    {
                        var depAsset = allAssets[depAssetIndexes[i]];
                        depAssetPaths[i] = depAsset.Name;
                    }
                    newAsset.DependencyAssetPaths = depAssetPaths;
                    bool isScene = asset.Name.EndsWith(".unity");
                    newResource.Resource.AssignAsset(newAsset, isScene);
                }
                result.Add(newResource);
            }

            return result.ToArray();
        }



    }
}