using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ND.UI
{
    public class ControllerDesignWindow : EditorWindow
    {

        // [MenuItem("Tools/KaneTest/Controller Design")]
        public static void Show()
        {
            EditorWindow.GetWindow<ControllerDesignWindow>(true, "Controller Design", true);
        }

        private Vector2 _scrollPos;

        private void Awake()
        {
            this.minSize = new Vector2(400, 600);
            
        }

        private void OnEnable()
        {
            Debug.Log("On Controller Design Window Enable");
        }

        private void OnDestroy()
        {
            Debug.Log("On Controller Design Window Destroy");
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginHorizontal();
            _scrollPos =EditorGUILayout.BeginScrollView(_scrollPos,  GUILayout.Height(100));
            GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb"); GUILayout.Label("bbbbbbbb");
            EditorGUILayout.EndScrollView();
        }
    }
}