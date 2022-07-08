/*
 * Tencent is pleased to support the open source community by making xLua available.
 * Copyright (C) 2016 THL A29 Limited, a Tencent company. All rights reserved.
 * Licensed under the MIT License (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at
 * http://opensource.org/licenses/MIT
 * Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.
*/

using System.Collections.Generic;
using System;
using ND.UI;
using ND.UI.NDUI;
using UnityEngine;
using XLua;
//using System.Reflection;
//using System.Linq;

//配置的详细介绍请看Doc下《XLua的配置.doc》
public static class ExampleGenConfig
{
    //lua中要使用到C#库的配置，比如C#标准库，或者Unity API，第三方库等。
    [LuaCallCSharp]
    public static List<Type> LuaCallCSharp = new List<Type>() {
                typeof(System.Object),
                typeof(UnityEngine.Object),
                typeof(Vector2),
                typeof(Vector3),
                typeof(Vector4),
                typeof(Rect),
                typeof(Quaternion),
                typeof(Color),
                typeof(Ray),
                typeof(RaycastHit),
                typeof(Bounds),
                typeof(Ray2D),
                typeof(Time),
                typeof(GameObject),
                typeof(Component),
                typeof(Behaviour),
                typeof(Transform),
                typeof(RectTransform),
                typeof(Resources),
                typeof(TextAsset),
                typeof(Keyframe),
                typeof(AnimationCurve),
                typeof(AnimationClip),
                typeof(MonoBehaviour),
                typeof(ParticleSystem),
                typeof(SkinnedMeshRenderer),
                typeof(Renderer),
                typeof(WWW),
                typeof(Light),
                typeof(Mathf),
                typeof(System.Collections.Generic.List<int>),
                typeof(Action<string>),
                typeof(UnityEngine.Debug),
                
                
                 typeof(ND.Managers.ResourceMgr.Runtime.ResLoader),
                 typeof(ND.Managers.ResourceMgr.Runtime.NFUResource),
                 
                 typeof(ND.UI.UIExpansion),
                 typeof(ND.UI.LinkerData),
                 typeof(ND.UI.ModuleData),

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
                 typeof(UnityEngine.UI.InputField),
                 
                 
                 //NDGUI
                 typeof(NDText),
                 typeof(ND.UI.NDUI.NDRichText),
                 typeof(ND.UI.NDUI.NDImage),
                 typeof(ND.UI.NDUI.NDRawImage),
                 typeof(ND.UI.NDUI.NDButton),
                 typeof(ND.UI.NDUI.NDToggle),
                 typeof(ND.UI.NDUI.NDSlider),
                 typeof(ND.UI.NDUI.NDDragButton),
                 typeof(ND.UI.NDUI.NDList),
                 typeof(ND.UI.NDUI.NDRating),
                 typeof(ND.UI.NDUI.NDRawRating),
                 typeof(ND.UI.NDUI.NDHitArea),
                 typeof(ND.UI.NDUI.NDScrollArrow),
                 typeof(ND.UI.NDUI.NDComboList),
                 typeof(ND.UI.NDUI.NDTableView),
                 typeof(ND.UI.NDUI.NDGridView),
                 typeof(ND.UI.NDUI.NDTableViewCell),
                 typeof(ND.UI.NDUI.NDScrollRect),
                 typeof(ND.UI.NDUI.NDInputField),
                 typeof(ND.UI.UIExpansion),
                 
                 typeof(List<string>),
                 typeof(List<ND.UI.LinkerData>),

                 //Custom
                 typeof(UnityEngine.UI.Image),
                 typeof(UnityEngine.UI.RawImage),
    };

    //C#静态调用Lua的配置（包括事件的原型），仅可以配delegate，interface
    [CSharpCallLua]
    public static List<Type> CSharpCallLua = new List<Type>() {
                
                typeof(Func<double, double, double>),
                typeof(Func<int, int>),
                
                typeof(Action),
                typeof(Action<int>),
                typeof(Action<object>),
                typeof(Action<bool>),
                typeof(Action<Sprite[]>),
                typeof(Action<string>),
                typeof(Action<double>),
                
                typeof(UnityEngine.Events.UnityAction),
                typeof(System.Collections.IEnumerator),
                
                typeof(System.Predicate<int>),
                typeof(System.Comparison<int>),
                
                typeof(ND.Managers.ResourceMgr.Framework.Resource.LoadAssetSuccessCallback),
                typeof(ND.Managers.ResourceMgr.Framework.Resource.LoadAssetFailureCallback),
                typeof(ND.Managers.ResourceMgr.Runtime.Res.ResLoadCompleteCallBack),
                typeof(ND.UI.NDUI.GetCellReuseIdentifierDelegate),
                typeof(ND.UI.NDUI.CellOnUseDelegate),
    };

    //黑名单
    [BlackList]
    public static List<List<string>> BlackList = new List<List<string>>()  {
                new List<string>(){"System.Xml.XmlNodeList", "ItemOf"},
                new List<string>(){"UnityEngine.WWW", "movie"},
    #if UNITY_WEBGL
                new List<string>(){"UnityEngine.WWW", "threadPriority"},
    #endif
                new List<string>(){"UnityEngine.Texture2D", "alphaIsTransparency"},
                new List<string>(){"UnityEngine.Security", "GetChainOfTrustValue"},
                new List<string>(){"UnityEngine.CanvasRenderer", "onRequestRebuild"},
                new List<string>(){"UnityEngine.Light", "areaSize"},
                new List<string>(){"UnityEngine.Light", "lightmapBakeType"},
                
                new List<string>(){"UnityEngine.Light", "SetLightDirty"},
                new List<string>(){"UnityEngine.Light", "shadowAngle"},
                new List<string>(){"UnityEngine.Light", "shadowRadius"},
                
                new List<string>(){"UnityEngine.WWW", "MovieTexture"},
                new List<string>(){"UnityEngine.WWW", "GetMovieTexture"},
                new List<string>(){"UnityEngine.AnimatorOverrideController", "PerformOverrideClipListCleanup"},
    #if !UNITY_WEBPLAYER
                new List<string>(){"UnityEngine.Application", "ExternalEval"},
    #endif
                new List<string>(){"UnityEngine.GameObject", "networkView"}, //4.6.2 not support
                new List<string>(){"UnityEngine.Component", "networkView"},  //4.6.2 not support
                new List<string>(){"System.IO.FileInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.FileInfo", "SetAccessControl", "System.Security.AccessControl.FileSecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "GetAccessControl", "System.Security.AccessControl.AccessControlSections"},
                new List<string>(){"System.IO.DirectoryInfo", "SetAccessControl", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "CreateSubdirectory", "System.String", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"System.IO.DirectoryInfo", "Create", "System.Security.AccessControl.DirectorySecurity"},
                new List<string>(){"UnityEngine.MonoBehaviour", "runInEditMode"},
                
                new List<string>(){"UnityEngine.UI.Text", "OnRebuildRequested"},
                
            };
}
