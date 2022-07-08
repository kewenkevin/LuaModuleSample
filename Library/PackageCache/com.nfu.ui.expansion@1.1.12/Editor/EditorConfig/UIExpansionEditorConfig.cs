using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    public class UIExpansionEditorConfig : ScriptableObject
    {
        [SerializeField] public List<string> luaFolderPathArray = new List<string>(){@"..\Lua"};
        [SerializeField] public string luaExporterClass = "CustomLuaExporter";//lua模板导出类，nfu默认提供一套模板
        [SerializeField] public string goPrefix = "E_";//导出对象名称前缀
        [SerializeField] public string nfuModule = "self.M_AAA = self:registerModule(\"M_AAA\",\"BBB\",self.AAA.GameObject)"; //AAA是GO名称占位符，BBB是module类名占位符
        [SerializeField] public string nfuCallback = "handler(self,?????)"; //?????是Go占位符
        [SerializeField] public AnimationClip defaultOpenAni;
        [SerializeField] public AnimationClip defaultCloseAni;
        [SerializeField] public TextAsset pageTemplate;
        [SerializeField] public TextAsset moduleTemplate;
        [SerializeField] public TextAsset widgetTemplate;
    }
}