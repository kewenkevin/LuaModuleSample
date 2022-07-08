using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace UnityEditor
{
    [UnityEditor.AssetImporters.ScriptedImporter(1, "lua")]
    public class LuaImporter : UnityEditor.AssetImporters.ScriptedImporter
    {
        public override void OnImportAsset(UnityEditor.AssetImporters.AssetImportContext ctx)
        {
            var text = File.ReadAllText(ctx.assetPath);
            var asset = new TextAsset(text);
            ctx.AddObjectToAsset("main", asset);
            ctx.SetMainObject(asset);
        }
    }
}