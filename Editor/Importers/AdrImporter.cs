using ForgeLightToolkit.Editor.FileTypes.Adr;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ForgeLightToolkit.Editor.Importers {
    [ScriptedImporter(1, "adr")]
    public class AdrImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            if (string.IsNullOrEmpty(ctx.assetPath)) {
                ctx.LogImportError($"Invalid asset path. ({ctx.assetPath})");
                return;
            }

            var adrFile = ScriptableObject.CreateInstance<AdrFile>();

            // Logging the error here allows the ADR file to still be searchable in the asset manager with t:adrFile
            ctx.AddObjectToAsset("Adr", adrFile);
            ctx.SetMainObject(adrFile);

            if (!adrFile.Load(ctx.assetPath)) {
                ctx.LogImportError($"Failed to load adr file. ({ctx.assetPath})");
                return;
            }
        }
    }
}
