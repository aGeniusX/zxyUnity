using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor.AssetImporters;
using UnityEngine;

[ScriptedImporter(1, ".lua")]
public class LuaImporter : ScriptedImporter
{
    public override void OnImportAsset(AssetImportContext ctx)
    {
        var luaTxt = File.ReadAllText(ctx.assetPath);
        var asssetText = new TextAsset(luaTxt);
        ctx.AddObjectToAsset("main obj", asssetText);

        ctx.SetMainObject(asssetText);
    }
}
