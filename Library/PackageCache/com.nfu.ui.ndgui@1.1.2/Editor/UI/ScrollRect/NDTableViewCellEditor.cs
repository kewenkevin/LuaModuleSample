
using UnityEngine.UI;
using UnityEngine;
using UnityEditor;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDTableViewCell), true)]
    public class NDTableViewCellEditor : Editor
    {
        SerializedProperty m_CellSpacing;
        SerializedProperty m_TableViewCells;
        SerializedProperty m_TotalCount;

        protected void OnEnable()
        {
            var cell = target as NDTableViewCell;
            cell.UpdateCellSize();


        }

        public override void OnInspectorGUI()
        {
            //serializedObject.Update();
            //EditorGUILayout.PropertyField(m_TableViewCells);
            //EditorGUILayout.PropertyField(m_CellSpacing);
            //serializedObject.ApplyModifiedProperties();
            base.OnInspectorGUI();

            if (!Application.isPlaying)
            {
                OnEditorModeGUI();
            }


            //var scroll = target as YTableView;
            //scroll.CheckLayout();
        }

        private void OnEditorModeGUI()
        {
            if (GUILayout.Button("Refresh Cell Size"))
            {
                //UIExpansionWindow window = EditorWindow.GetWindow<UIExpansionWindow>();
                //window.ChangeTarget(_ui);
                var cell = target as NDTableViewCell;
                cell.UpdateCellSize();
            }
        }
    }
}