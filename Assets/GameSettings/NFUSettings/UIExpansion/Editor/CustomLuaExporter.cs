using System;
using System.Collections.Generic;
using ND.UI;
using ND.UI.NDUI;


public class CustomLuaExporter : NFULuaExporter
{
    public override List<Type> GetCanExportTypes()
    {
        
        return new List<Type>()
        {
            //UGUI
            typeof(UnityEngine.Events.UnityEvent),
            typeof(UnityEngine.UI.Button),
            typeof(UnityEngine.UI.Selectable),
            typeof(UnityEngine.UI.Text),
            typeof(UnityEngine.UI.MaskableGraphic),
            typeof(UnityEngine.Canvas),
            typeof(UnityEngine.UI.GraphicRaycaster),
            typeof(UnityEngine.RenderMode),
            typeof(UnityEngine.CameraClearFlags),
            typeof(UnityEngine.LayerMask),
            typeof(UnityEngine.Sprite),
            typeof(UnityEngine.UI.Scrollbar),
            typeof(UnityEngine.UI.Image),
            typeof(UnityEngine.UI.RawImage),


            //NDGUI
            typeof(NDText),
            typeof(NDRichText),
            typeof(NDImage),
            typeof(NDRawImage),
            typeof(NDButton),
            typeof(NDToggle),
            typeof(NDSlider),
            typeof(NDDragButton),
            typeof(NDList),
            typeof(NDRating),
            typeof(NDRawRating),
            typeof(NDHitArea),
            typeof(NDScrollArrow),
            typeof(NDComboList),
            typeof(NDTableView),
            typeof(NDGridView),
            typeof(NDTableViewCell),
            typeof(NDScrollRect),
            typeof(ND.UI.UIExpansion),
            typeof(NDScrollView),
            typeof(NDInputField)
        };
    }
}
