using System;
using UnityEngine;

namespace ND.UI.Core
{
    public interface IBindable
    {
        void RemoveAllAction();
        void RemoveAction(string label);

        void SetController(string na, int index);

        int GetControllerSelectedIndex(string na);

        string GetControllerSelectedPageName(string na);

        
        void LinkerSetQuaternion(string label, Quaternion value);
        void LinkerSetString(string label, string value);

        void LinkerSetSingle(string label, float value);

        void LinkerSetVector2(string label, Vector2 value);

        void LinkerSetVector3(string label, Vector3 value);

        void LinkerSetColor(string label, Color value);
        void LinkerSetSystemObject(string label, System.Object value);

        void LinkerSetAction(string label, UnityEngine.Events.UnityAction value);

        void LinkerSetActionInt32(string label, UnityEngine.Events.UnityAction<int> value);

        void LinkerSetActionSingle(string label, UnityEngine.Events.UnityAction<float> value);

        void LinkerSetActionString(string label, UnityEngine.Events.UnityAction<string> value);

        void LinkerSetInt32(string label, int value);

        void LinkerSetBoolean(string label, bool value);

        void LinkerSetSprite(string label, Sprite value);

        void LinkerSetChar(string label, char value);

        void LinkerSetRect(string label, Rect value);

        void LinkerSetActionBoolean(string label, UnityEngine.Events.UnityAction<bool> value);

        void LinkerSetActionVector2(string label, UnityEngine.Events.UnityAction<Vector2> value);
        void LinkerSetAction2(string label, UnityEngine.Events.UnityAction value);
        void LinkerSetAction2Boolean(string label, UnityEngine.Events.UnityAction<bool> value);

        void LinkerSetAction2Single(string label, UnityEngine.Events.UnityAction<Single> value);

        void LinkerSetAction2Int32(string label, UnityEngine.Events.UnityAction<Int32> value);

        void LinkerSetAction2String(string label, UnityEngine.Events.UnityAction<string> value);

        void LinkerSetAction2Vector2(string label, UnityEngine.Events.UnityAction<Vector2> value);
    }
}