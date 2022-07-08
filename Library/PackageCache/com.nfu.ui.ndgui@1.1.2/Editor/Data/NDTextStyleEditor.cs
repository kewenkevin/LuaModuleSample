using ND.UI.Core.StyleDataEditor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDTextStyle))]
    public class NDTextStyleEditor : TextStyleBaseEditor
    {
        protected override void ApplyToCurrentStage()
        {
            var fontStyle = (NDTextStyle) serializedObject.targetObject;
            
            var stageHandle = StageUtility.GetCurrentStageHandle();
            if (stageHandle == null)
            {
                return;
            }
                
            var textArr = stageHandle.FindComponentsOfType<NDText>();
            foreach (var text in textArr)
            {
                if (text.style == fontStyle)
                {
                    fontStyle.Apply(text);
                }
            }
        }
    }
}