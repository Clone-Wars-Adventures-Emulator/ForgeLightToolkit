using ForgeLightToolkit.Editor.FileTypes.Map;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ForgeLightToolkit.Editor.Importers {
    [ScriptedImporter(1, "map")]
    public class MapImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            if (string.IsNullOrEmpty(ctx.assetPath)) {
                ctx.LogImportError($"Invalid asset path. ({ctx.assetPath})");
                return;
            }

            var mapFile = ScriptableObject.CreateInstance<MapFile>();

            if (!mapFile.Load(ctx.assetPath)) {
                ctx.LogImportError($"Failed to load map file. ({ctx.assetPath})");
                return;
            }

            ctx.AddObjectToAsset("Map", mapFile);
            ctx.SetMainObject(mapFile);
        }
    }
}
