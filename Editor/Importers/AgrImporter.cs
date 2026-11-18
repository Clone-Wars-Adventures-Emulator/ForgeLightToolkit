using ForgeLightToolkit.Editor.FileTypes.Agr;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ForgeLightToolkit.Editor.Importers {
    // Theoretically, though unprovable with current game data, this would depend on the ADR files, and as such this should run after ADRs
    [ScriptedImporter(1, "agr", QueueOffsets.STAGE_3_POST_ADR)]
    public class AgrImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            if (string.IsNullOrEmpty(ctx.assetPath)) {
                ctx.LogImportError($"Invalid asset path. ({ctx.assetPath})");
                return;
            }

            var agrFile = ScriptableObject.CreateInstance<AgrFile>();

            if (!agrFile.Load(ctx.assetPath)) {
                ctx.LogImportError($"Failed to load agr file. ({ctx.assetPath})");
                return;
            }

            ctx.AddObjectToAsset("Agr", agrFile);
            ctx.SetMainObject(agrFile);
        }
    }
}
