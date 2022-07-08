using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;


namespace ND.Managers.ResourceMgr.Editor
{
    class UXMLWindow : EditorWindow
    {
        static string ussDirectory;
        static string uxmlDirectory;
        [NonSerialized]
        private bool isLoaded;
        private Vector2 lastSize;

        static string USSDirectory
        {
            get
            {
                if (ussDirectory == null)
                {
                    ussDirectory = EditorUtilityx.PackageDir + "/Editor/USS";
                }
                return ussDirectory;
            }
        }
        static string UXMLDirectory
        {
            get
            {
                if (uxmlDirectory == null)
                {
                    uxmlDirectory = EditorUtilityx.PackageDir + "/Editor/UXML";
                }
                return uxmlDirectory;
            }
        }


        public string GetUXMLPath(string uxml)
        {
            return UXMLDirectory + "/" + uxml;
        }
        public string GetUSSPath(string uss)
        {
            return USSDirectory + "/" + uss;
        }
        public bool IsLoaded { get => isLoaded; }


        public VisualTreeAsset LoadVisualTreeAsset(string uxml)
        {
            string path;
            path = GetUXMLPath(uxml);
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(path);
            if (visualTree == null)
                throw new Exception("VisualTreeAsset load fail. path: " + path);
            return visualTree;
        }

        public VisualElement LoadUXML(string uxml, string bindingPath = null)
        {
            return LoadUXML(null, uxml, bindingPath);
        }

        public VisualElement LoadUXML(VisualElement parent, string uxml, string bindingPath = null)
        {
            var visualTree = LoadVisualTreeAsset(uxml);
            VisualElement tree;
            if (!string.IsNullOrEmpty(bindingPath))
                tree = visualTree.CloneTree(bindingPath);
            else
                tree = visualTree.CloneTree();

            if (parent == null)
                parent = rootVisualElement;
            parent.Add(tree);
            return tree;
        }

        public void LoadStyleSheet(string uss)
        {
            string path;
            path = GetUSSPath(uss);
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(path);
            if (styleSheet == null)
                throw new Exception("Load StyleSheet null. path: " + path);
            rootVisualElement.styleSheets.Add(styleSheet);
        }

        protected virtual string GetMainUXML()
        {
            string uxml = $"{GetType().Name}.uxml";
            return uxml;
        }

        protected virtual void Load()
        {

        }

        protected virtual void Update()
        {
            if (lastSize != this.position.size)
            {
                lastSize = this.position.size;
                OnResize();
            }
        }

        private void CreateGUI()
        {
            string uxml = GetMainUXML();

            if (!string.IsNullOrEmpty(uxml))
            {
                var elem = LoadUXML(uxml);
                if (elem != null && (elem is TemplateContainer))
                {
                    elem.style.flexGrow = 1f;
                }
            }
            Load();
            isLoaded = true;
        }

        protected virtual void OnResize()
        {

        }

    }
}