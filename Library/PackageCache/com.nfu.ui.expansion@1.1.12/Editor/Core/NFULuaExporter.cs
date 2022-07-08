using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ND.UI
{
    public class NFULuaExporter : ILuaExporter
    {
        private List<string> m_needGenFunctions = new List<string>();


        public virtual bool GenerateFile(UIExpansion uiExpansion, string filePath)
        {
            if (!File.Exists(filePath))
            {
                return GenerateFileNotExist(uiExpansion, filePath);
            }
            else
            {
                return GenerateFileExist(uiExpansion, filePath);
            }
        }

        public virtual List<Type> GetCanExportTypes()
        {
            return new List<Type>()
            {
                //UGUI
                typeof(UnityEngine.Events.UnityEvent),
                typeof(UnityEngine.UI.Button),
                typeof(UnityEngine.UI.Selectable),
                typeof(UnityEngine.UI.Text),
                typeof(UnityEngine.UI.MaskableGraphic),
                typeof(Canvas),
                typeof(UnityEngine.UI.GraphicRaycaster),
                typeof(RenderMode),
                typeof(CameraClearFlags),
                typeof(LayerMask),
                typeof(Sprite),
                typeof(UnityEngine.UI.Scrollbar),
                typeof(UnityEngine.UI.Image),
                typeof(UnityEngine.UI.RawImage),
            };
        }

        public virtual List<Type> GetForceIgnoreTypes()
        {
            return new List<Type>()
            {
                typeof(CanvasRenderer)
            };
        }


        /// <summary>
        /// 创建模板代码
        /// </summary>
        /// <param name="uiExpansion"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool GenerateFileNotExist(UIExpansion uiExpansion, string filePath)
        {
            var index = EditorUtility.DisplayDialogComplex("模板类型", "请选择Lua模板类型", "页面（Page）", "挂件(Widget)","子模块(Module)");

            StringBuilder builder = new StringBuilder();
            var exportName = Path.GetFileNameWithoutExtension(filePath);

            switch (index)
            {
                case 0:
                default:
                    builder.AppendLine("--regionCustomCreator 项目可自定义的ui page代码生成部分");
                    string path = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/PageTemplate.txt";

                    if (!File.Exists(path))
                    {
                        Debug.Log("不存在页面page的模板代码，请打开ProjectSetting的UIExpansion设置界面自动生成");
                    }
                    else
                    {
                        string[] strs = File.ReadAllLines(path);

                        foreach (string item in strs)
                        {
                            string tmp = item.Replace("?????", exportName);
                            builder.AppendLine(tmp);
                        }
                    }

                    builder.AppendLine("--regionEndCustomCreator");
                    break;
                
                case 1:
                    builder.AppendLine("--regionCustomCreator 项目可自定义的ui widget代码生成部分");

                    string path3 = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/WidgetTemplate.txt";

                    if (!File.Exists(path3))
                    {
                        Debug.Log("不存在挂件Widget的模板代码，请打开ProjectSetting的UIExpansion设置界面自动生成");
                    }
                    else
                    {
                        string[] strs = File.ReadAllLines(path3);

                        foreach (string item in strs)
                        {
                            string tmp = item.Replace("?????", exportName);
                            builder.AppendLine(tmp);
                        }
                    }

                    builder.AppendLine("--regionEndCustomCreator");
                    break;
                case 2:
                    builder.AppendLine("--regionCustomCreator 项目可自定义的ui module代码生成部分");

                    string path2 = Application.dataPath + "/GameSettings/NFUSettings/UIExpansion/Editor/ModuleTemplate.txt";

                    if (!File.Exists(path2))
                    {
                        Debug.Log("不存在模组module的模板代码，请打开ProjectSetting的UIExpansion设置界面自动生成");
                    }
                    else
                    {
                        string[] strs = File.ReadAllLines(path2);

                        foreach (string item in strs)
                        {
                            string tmp = item.Replace("?????", exportName);
                            builder.AppendLine(tmp);
                        }
                    }

                    builder.AppendLine("--regionEndCustomCreator");
                    break;
                
                    
            }
            

            builder.AppendLine();
            
            //先生成按钮回调方法
            GenCallBackFunction(builder, exportName, String.Empty,uiExpansion);
            
            //再生成自定义生成绑定方法
            GenCustomFunction(builder, exportName, String.Empty);

            //最后生成绑定控件
            GenBindComponents(builder, uiExpansion, exportName, String.Empty);

            

           

            builder.AppendLine();
            builder.AppendLine($"return {exportName}");

            byte[] buffer1 = Encoding.Default.GetBytes(builder.ToString());
            byte[] buffer2 = Encoding.Convert(Encoding.UTF8, Encoding.Default, buffer1, 0, buffer1.Length);
            File.WriteAllBytes(filePath, buffer2);
            return true;
        }

        /// <summary>
        /// 覆盖模板代码
        /// </summary>
        /// <param name="uiExpansion"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private bool GenerateFileExist(UIExpansion uiExpansion, string filePath)
        {
            var exportName = Path.GetFileNameWithoutExtension(filePath);

            var allTextLines = File.ReadAllLines(filePath).ToArray();
            var allContent = File.ReadAllText(filePath);

            var returnTag = string.Empty;
            for (int i = allTextLines.Length - 1; i >= 0; i--)
            {
                if (allTextLines[i].StartsWith("return "))
                {
                    returnTag = allTextLines[i].Substring("return ".Length);
                    returnTag = returnTag.Replace(";", "");
                    returnTag = returnTag.Replace(" ", "");
                    break;
                }
            }

            if (returnTag != string.Empty)
            {
                StringBuilder builder = new StringBuilder();

                bool containsBindComponents = allContent.Contains($"function {returnTag}:initAutoBind");
                bool inRegionBindComponent = false;
                bool ifAddLine;
                for (int i = 0; i < allTextLines.Length; i++)
                {
                    var line = allTextLines[i];
                    ifAddLine = !inRegionBindComponent;
                    if (containsBindComponents)
                    {
                        if (line.StartsWith("--regionAutoBind"))
                        {
                            inRegionBindComponent = true;
                        }
                        else if (line.StartsWith("--endRegionAutoBind"))
                        {
                            inRegionBindComponent = false;
                            
                            GenCallBackFunction(builder, exportName, allContent,uiExpansion);
                            
                            GenCustomFunction(builder, exportName, allContent);
                            
                            GenBindComponents(builder, uiExpansion, returnTag, allContent);
                            
                        }
                    }
                    else
                    {
                        if (line.StartsWith("return " + returnTag))
                        {
                            
                            GenCallBackFunction(builder, exportName, allContent,uiExpansion);
                            
                            GenCustomFunction(builder, exportName, allContent);
                            
                            GenBindComponents(builder, uiExpansion, returnTag, allContent);
                            
                        }
                    }

                    if (!inRegionBindComponent && ifAddLine)
                    {
                        builder.AppendLine(line);
                    }
                }

                File.WriteAllText(filePath, builder.ToString());

                return true;
            }
            else
            {
                Debug.LogError("Lua代码无返回，请检查。文件路径 " + filePath);
                return false;
            }
        }

        /// <summary>
        /// 生成绑定的组件
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="uiExpansion"></param>
        /// <param name="exportName"></param>
        /// <param name="allContent"></param>
        private void GenBindComponents(StringBuilder builder, UIExpansion uiExpansion, string exportName, string allContent)
        {
            var dic = GetComponents(uiExpansion);
            if (dic.Count > 0)
            {
                builder.AppendLine();
                builder.AppendLine("--regionAutoBind 导出控件代码自动生成，请勿手动修改");
                builder.AppendLine($"function {exportName}:initAutoBind(gameObject)");
                builder.AppendLine("    ---@type UIExpansion");
                builder.AppendLine("    self.uiExpansion = gameObject:GetComponent(typeof(CS.ND.UI.UIExpansion))");

                foreach (var kv in dic)
                {
                    bool bRoot = kv.Key.Equals(uiExpansion.name);

                    if (bRoot)
                    {
                        builder.AppendLine($"    --UIExpansion根节点组件");
                        builder.AppendLine($"    self.Root" + " = {");
                    }
                    else
                    {
                        builder.AppendLine($"    self.{kv.Key}" + " = {");
                    }

                    foreach (var sv in kv.Value)
                    {
                        builder.AppendLine($"       ---@type {sv.Key}");
                        builder.AppendLine($"       {sv.Key} = self.uiExpansion:GetBindObject({sv.Value}),");
                    }

                    builder.AppendLine("    }");
                }

                builder.AppendLine("    self:BindCustomModules()");
                builder.AppendLine("    self:BindCommonFunction()");
                builder.AppendLine("end");

                builder.AppendLine();

                GenBindCustomModules(dic, builder, uiExpansion, exportName);
                GenBindCommonFunction(dic, builder, uiExpansion, exportName);

                builder.AppendLine("\n--endRegionAutoBind");
            }
        }

        /// <summary>
        /// 生成绑定的module
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="builder"></param>
        /// <param name="uiExpansion"></param>
        /// <param name="exportName"></param>
        private void GenBindCustomModules(Dictionary<string, Dictionary<string, int>> dic, StringBuilder builder, UIExpansion uiExpansion,
            string exportName)
        {
            builder.AppendLine("");
            builder.AppendLine($"---BindCustomModules绑定静态Module方法");
            builder.AppendLine($"function {exportName}:BindCustomModules()");
            foreach (var kv in dic)
            {
                bool bRoot = kv.Key.Equals(uiExpansion.name);
                if (!bRoot)
                {
                    foreach (var i in kv.Value)
                    {
                        if (i.Key.Equals("UIExpansion"))
                        {
                            UIExpansion moduleUIExpansion = GetUIExpansionByGoName(uiExpansion, kv.Key);
                            if (moduleUIExpansion && moduleUIExpansion.LuaBindPath != String.Empty)
                            {
                                string[] luaName = moduleUIExpansion.LuaBindPath.Split('.');
                                //Debug.Log(luaName[luaName.Length-1]);
                                UIExpansionEditorConfig config = UIExpansionEditorConfigSettingsProvider.GetConfig();

                                string finalConfig = config.nfuModule.Replace("AAA", kv.Key).Replace("BBB", luaName[luaName.Length - 1]);
                                builder.AppendLine($"   ---@type {luaName[luaName.Length - 1]}");
                                builder.AppendLine($"   {finalConfig}");
                            }
                        }
                    }
                }
            }

            builder.AppendLine("end");
        }

        /// <summary>
        /// 生成绑定通用事件
        /// </summary>
        /// <param name="dic"></param>
        /// <param name="builder"></param>
        /// <param name="uiExpansion"></param>
        /// <param name="exportName"></param>
        private void GenBindCommonFunction(Dictionary<string, Dictionary<string, int>> dic, StringBuilder builder, UIExpansion uiExpansion,
            string exportName)
        {
            m_needGenFunctions.Clear();

            builder.AppendLine("");
            builder.AppendLine($"---BindCustomFunction绑定通用事件方法");
            builder.AppendLine($"function {exportName}:BindCommonFunction()");

            foreach (var kv in dic)
            {
                bool bRoot = kv.Key.Equals(uiExpansion.name);
                if (bRoot)
                {
                    foreach (var v in kv.Value)
                    {
                        if (v.Key.Equals("NDButton"))
                        {
                            builder.AppendLine($"   self.Root.NDButton.onClick:AddListener(handler(self,self.OnClick_Root))");
                            m_needGenFunctions.Add("Root");
                        }
                    }
                }
                else
                {
                    foreach (var v in kv.Value)
                    {
                        if (v.Key.Equals("NDButton"))
                        {
                            builder.AppendLine($"   self.{kv.Key}.NDButton.onClick:AddListener(handler(self,self.OnClick_{kv.Key}))");
                            m_needGenFunctions.Add(kv.Key);
                        }
                    }
                }
            }

            builder.AppendLine("   self:BindCustomFunction()");
            builder.AppendLine("end");
        }

        /// <summary>
        /// 生成自定义绑定方法
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="exportName"></param>
        private void GenCustomFunction(StringBuilder builder, string exportName, string allContent)
        {
            //已存在不再生成
            if (allContent.Contains("---BindCustomFunction绑定自定义事件方法"))
            {
                return;
            }

            builder.AppendLine("");
            builder.AppendLine($"---BindCustomFunction绑定自定义事件方法");
            builder.AppendLine($"function {exportName}:BindCustomFunction()");
            builder.AppendLine("");
            builder.AppendLine("end");
        }

        /// <summary>
        /// 生成绑定回调方法
        /// </summary>
        /// <param name="builder"></param>
        /// <param name="exportName"></param>
        /// <param name="allContent"></param>
        private void GenCallBackFunction(StringBuilder builder, string exportName, string allContent,UIExpansion uiExpansion)
        {

            CollectNeedGenFunction(uiExpansion);
            
            foreach (string funcName in m_needGenFunctions)
            {
                string tag = $"function {exportName}:OnClick_{funcName}()";

                if (!allContent.Contains(tag))
                {
                    builder.AppendLine("");
                    builder.AppendLine($"-----OnClick_{funcName}()");
                    builder.AppendLine($"function {exportName}:OnClick_{funcName}()");
                    builder.AppendLine("");
                    builder.AppendLine("end");
                }
            }
        }


        private Dictionary<string, Dictionary<string, int>> GetComponents(UIExpansion uiExpansion)
        {
            var result = new Dictionary<string, Dictionary<string, int>>();

            if (uiExpansion.BindedObjects != null)
            {
                for (int i = 0; i < uiExpansion.BindedObjects.Length; i++)
                {
                    UnityEngine.Object o = uiExpansion.BindedObjects[i];

                    // string toggleName;
                    // if (o is GameObject)
                    // {
                    //     toggleName = o.name;
                    // }
                    // else
                    // {
                    //     var type = o.GetType();
                    //     var go = (o as Component).gameObject;
                    //     toggleName = $"{go.name}.{type.Name}";
                    // }
                    //
                    // if (!uiExpansion.BindObjectToggleGroup.ContainsKey(toggleName) ||uiExpansion.BindObjectToggleGroup[toggleName] == false )
                    // {
                    //     continue;
                    // }


                    if (o is GameObject)
                    {
                        var name = (o as GameObject).name;
                        name = name.Replace(" ", "_");
                        name = name.Replace("(", "");
                        name = name.Replace(")", "");
                        if (!result.ContainsKey(name))
                        {
                            result.Add(name, new Dictionary<string, int>());
                        }

                        if (!result[name].ContainsKey("GameObject"))
                        {
                            result[name].Add("GameObject", i);
                        }
                        else
                        {
                            Debug.LogWarningFormat("命名冲突：{0}!!!", name);
                        }
                    }
                    else
                    {
                        var go = (o as Component).gameObject;
                        var type = o.GetType();
                        var name = go.name;
                        name = name.Replace(" ", "_");
                        name = name.Replace("(", "");
                        name = name.Replace(")", "");

                        var pname = type.Name;

                        if (!result.ContainsKey(name))
                        {
                            result.Add(name, new Dictionary<string, int>());
                        }

                        if (!result[name].ContainsKey(pname))
                        {
                            result[name].Add(pname, i);
                        }
                        else
                        {
                            Debug.LogWarningFormat("命名冲突：{0}.{1}", name, pname);
                        }
                    }
                }
            }

            return result;
        }


        /// <summary>
        /// 获取module的uiExpansion
        /// </summary>
        /// <param name="parentUIExpansion"></param>
        /// <param name="goName"></param>
        /// <returns></returns>
        private UIExpansion GetUIExpansionByGoName(UIExpansion parentUIExpansion, string goName)
        {
            foreach (var bindObject in parentUIExpansion.BindedObjects)
            {
                if (bindObject is UIExpansion)
                {
                    if (goName.Equals(bindObject.name))
                    {
                        return (UIExpansion)bindObject;
                    }
                }
            }

            return null;
        }

        private void CollectNeedGenFunction(UIExpansion uiExpansion)
        {
            m_needGenFunctions.Clear();
            
            var dic = GetComponents(uiExpansion);
            if (dic.Count > 0 )
            {
                foreach (var kv in dic)
                {
                    bool bRoot = kv.Key.Equals(uiExpansion.name);
                    if (bRoot)
                    {
                        foreach (var v in kv.Value)
                        {
                            if (v.Key.Equals("NDButton"))
                            {
                                m_needGenFunctions.Add("Root");
                            }
                        }
                    }
                    else
                    {
                        foreach (var v in kv.Value)
                        {
                            if (v.Key.Equals("NDButton"))
                            {
                                m_needGenFunctions.Add(kv.Key);
                            }
                        }
                    }
                }
            }
        }
    }
}