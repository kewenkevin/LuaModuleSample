using UnityEditor;
using UnityEngine;

namespace ND.UI.NDUI
{
    
    
    [CustomEditor(typeof(NDLine))]
    public class NDLineEditor:Editor
    {
        NDLine lineComponent;

        private int selectIdx = -1;
        
        private SerializedProperty pointArray;
        void OnEnable()
        {
            lineComponent=(NDLine)target;
            
            pointArray = serializedObject.FindProperty("points");
        }
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
        }
        
        private void OnSceneGUI()
        {
            // ProcessMouseEvent();
            DrawPointHandlers();


            // pointArray.arraySize++;

        }
        
        
        public const int MOUSE_BUTTON_LEFT = 0;
        public const int MOUSE_BUTTON_RIGHT = 1;
//         void ProcessMouseEvent()
//         {
//
//             if (Event.current.type == EventType.MouseDrag)
//             {
//
//             }
//
//             if (Event.current.type == EventType.MouseDown)
//             {
//                 if (Event.current.button == MOUSE_BUTTON_LEFT)
//                 {
//                     //用发射射线来确定点击目标
//                     // Event.current.mousePosition屏幕坐标，左上角是（0，0）右下角（camera.pixelWidth，camera.pixelHeight）
//
//                     // 需要转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
//
//                     // 当前屏幕坐标，左上角是（0，0）右下角（camera.pixelWidth，camera.pixelHeight）
//                     Vector2 mousePosition = Event.current.mousePosition;
//
//                     // Retina 屏幕需要拉伸值
//                     float mult = 1;
// #if UNITY_5_4_OR_NEWER
//                     mult = EditorGUIUtility.pixelsPerPoint;
// #endif
//
//                     // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
//                     mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y * mult;
//                     mousePosition.x *= mult;
//                     //获取世界坐标
//                     var worldPoint = SceneView.currentDrawingSceneView.camera.ScreenToWorldPoint(mousePosition);
//
//                     if (selectIdx < lineComponent.points.Length && selectIdx >= 0)
//                     {
//                         // var worldPos = lineComponent.transform.TransformPoint(lineComponent.points[selectIdx]);
//                         Vector2 localpos = lineComponent.transform.InverseTransformPoint(worldPoint);
//
//                         // if ((lineComponent.points[selectIdx] - localpos).magnitude <= lineComponent.linewidth)
//                         // {
//                         //     GenericMenu menu = new GenericMenu();
//                         //     menu.AddDisabledItem(new GUIContent("Not Recommend"));
//                         //     menu.AddItem(new GUIContent("Add"), true,
//                         //         callMethodUnSelect, null);
//                         //     // menu.AddSeparator("");
//                         //     // menu.AddItem(new GUIContent("Cancel"), true, callmethodCancel,
//                         //     //     null);
//                         //     menu.ShowAsContext();
//                         //     return;
//                         // }
//                     }
//                 }
//             }
//
//             if (Event.current.type == EventType.MouseUp)
//             {
//                 if (Event.current.button == MOUSE_BUTTON_LEFT)
//                 {
//                     //用发射射线来确定点击目标
//                 }
//                 else if (Event.current.button == MOUSE_BUTTON_RIGHT)
//                 {
//                     if ((Event.current.modifiers & EventModifiers.Control) != 0)
//                     {
//                         //按下Ctrl键盘
//                     }
//                     else
//                     {
//                         GenericMenu menu = new GenericMenu();
//                         menu.AddDisabledItem(new GUIContent("Not Recommend"));
//                         menu.AddItem(new GUIContent("Add"), true,
//                             callMethodUnSelect, null);
//                         // menu.AddSeparator("");
//                         // menu.AddItem(new GUIContent("Cancel"), true, callmethodCancel,
//                         //     null);
//                         menu.ShowAsContext();
//                     }
//
//                     //设置该事件被使用
//                     Event.current.Use();
//
//                 }
//                 else
//                 {
//                     //Debug.Log ("Event.current.button");
//                 }
//
//             }
//         }
        /*void callMethodUnSelect(object obj){

        }*/
        // private bool isTouching = false;
        // private Vector2 touchLocation = Vector2.zero;


        private void OnRightClk(Vector2 mousePosition,Camera camera)
        {
            GenericMenu menu = new GenericMenu();
            // menu.AddDisabledItem(new GUIContent("Insert Point Here"));
            menu.AddItem(new GUIContent("Insert Point Here"), false, (obj) =>
            {
                float maxDistance = float.MaxValue;
                float[] distancelist = new float[lineComponent.points.Length];
                for (int i = 0; i < lineComponent.points.Length; i++)
                {
                    var worldPos = lineComponent.transform.TransformPoint(lineComponent.points[i]);
                    Vector2 screenPos = camera.WorldToScreenPoint(worldPos);
                    distancelist[i] = (screenPos - mousePosition).magnitude;
                }

                float dis;
                int pIdx = distancelist.Length;
                for (int i = 1; i < distancelist.Length; i++)
                {
                    dis = distancelist[i - 1] + distancelist[i];
                    if (dis < maxDistance)
                    {
                        maxDistance = dis;
                        pIdx = i;
                    }
                }
                
                pointArray.InsertArrayElementAtIndex(pIdx);

                Vector3 world = camera.ScreenToWorldPoint(mousePosition);
                Vector2 local = lineComponent.transform.InverseTransformPoint(world);
                local.Set(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
                pointArray.GetArrayElementAtIndex(pIdx).vector2Value = local;
                serializedObject.ApplyModifiedProperties();
                
            }, null);
            
            
            
            menu.AddItem(new GUIContent("Delete Point Here"), false, (obj) =>
            {
                // float maxDistance = float.MaxValue;
                // float[] distancelist = new float[lineComponent.points.Length];
                // for (int i = 0; i < lineComponent.points.Length; i++)
                // {
                //     var worldPos = lineComponent.transform.TransformPoint(lineComponent.points[i]);
                //     Vector2 screenPos = camera.WorldToScreenPoint(worldPos);
                //     distancelist[i] = (screenPos - mousePosition).magnitude;
                // }
                //
                // float dis;
                // int pIdx = distancelist.Length;
                // for (int i = 1; i < distancelist.Length; i++)
                // {
                //     dis = distancelist[i - 1] + distancelist[i];
                //     if (dis < maxDistance)
                //     {
                //         maxDistance = dis;
                //         pIdx = i;
                //     }
                // }
                //
                // pointArray.InsertArrayElementAtIndex(pIdx);
                //
                // Vector3 world = camera.ScreenToWorldPoint(mousePosition);
                // Vector2 local = lineComponent.transform.InverseTransformPoint(world);
                // local.Set(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
                // pointArray.GetArrayElementAtIndex(pIdx).vector2Value = local;
                // serializedObject.ApplyModifiedProperties();
                
            }, null);
            menu.ShowAsContext();
        }

        private void DrawPointHandlers()
        {
            

            if (Event.current.type == EventType.MouseUp)
            {
                if (Event.current.button == MOUSE_BUTTON_RIGHT)
                {
                    
                    Vector2 mousePosition = Event.current.mousePosition;

                    // Retina 屏幕需要拉伸值
                    float mult = 1;
#if UNITY_5_4_OR_NEWER
                    mult = EditorGUIUtility.pixelsPerPoint;
#endif
                    // 转换成摄像机可接受的屏幕坐标，左下角是（0，0，0）右上角是（camera.pixelWidth，camera.pixelHeight，0）
                    mousePosition.y = SceneView.currentDrawingSceneView.camera.pixelHeight - mousePosition.y * mult;
                    mousePosition.x *= mult;
                    OnRightClk(mousePosition,SceneView.currentDrawingSceneView.camera);
                }
            }





            for (int i = 0; i < lineComponent.points.Length; i++)
            {
                var worldPos = lineComponent.transform.TransformPoint(lineComponent.points[i]);
                
                if (selectIdx == i)
                {
                    var newworldPos = Handles.PositionHandle(worldPos, Quaternion.identity);
                    Vector2 local = lineComponent.transform.InverseTransformPoint(newworldPos);
                    local.Set(Mathf.RoundToInt(local.x), Mathf.RoundToInt(local.y));
                    if (lineComponent.points[i] != local)
                    {
                        lineComponent.points[i].Set(local.x,local.y);
                        
                        lineComponent.SetVerticesDirty();
                        EditorUtility.SetDirty(lineComponent);
                    }
                }
                else
                {
                    Handles.Label(worldPos,i.ToString());
                    if (Handles.Button(worldPos,Quaternion.identity ,lineComponent.linewidth/2,lineComponent.linewidth/2*1.1f, Handles.RectangleHandleCap))
                    {
                        selectIdx = i;
                    }
                }
            }
        }
    }
}