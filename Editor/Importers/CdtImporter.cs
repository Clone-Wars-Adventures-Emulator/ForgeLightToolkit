using ForgeLightToolkit.Editor.FileTypes;
using UnityEngine;
using UnityEditor.AssetImporters;

namespace ForgeLightToolkit.Editor.Importers {
    [ScriptedImporter(1, "cdt")]
    public class CdtImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            if (string.IsNullOrEmpty(ctx.assetPath)) {
                ctx.LogImportError($"Invalid asset path. ({ctx.assetPath})");
                return;
            }

            var cdtFile = ScriptableObject.CreateInstance<CdtFile>();

            if (!cdtFile.Load(ctx.assetPath)) {
                ctx.LogImportError($"Failed to load cdt file. ({ctx.assetPath})");
                return;
            }

            ctx.AddObjectToAsset("cdt", cdtFile);
            ctx.SetMainObject(cdtFile);
            ctx.AddObjectToAsset("collider", cdtFile.colliderMesh);
        }
    }
}
