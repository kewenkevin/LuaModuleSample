
using ND.UI.Core.StyleDataEditor;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace ND.UI.NDUI
{
    [CustomEditor(typeof(NDTextColorStyle))]
    public class NDTextColorStyleEditor : ColorStyleBaseEditor
    {
       
        protected override void ApplyToCurrentStage()
        {
            var colorStyle = (NDTextColorStyle) serializedObject.targetObject;
            
            var stageHandle = StageUtility.GetCurrentStageHandle();
            if (stageHandle == null)
            {
                return;
            }
                
            var textArr = stageHandle.FindComponentsOfType<NDText>();
            foreach (var text in textArr)
            {
                if (text.colorStyle == colorStyle)
                {
                    colorStyle.Apply(text);
                }
            }
        }
        
    }

}