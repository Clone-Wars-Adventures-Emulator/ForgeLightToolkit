using ForgeLightToolkit.Editor.FileTypes;
using ForgeLightToolkit.Editor.FileTypes.Dma;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ForgeLightToolkit.Editor.Importers {
    [ScriptedImporter(1, "dma")]
    public class DmaImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            if (string.IsNullOrEmpty(ctx.assetPath)) {
                ctx.LogImportError($"Invalid asset path. ({ctx.assetPath})");
                return;
            }

            var dmaFile = ScriptableObject.CreateInstance<DmaFile>();

            ctx.AddObjectToAsset("Dma", dmaFile);
            ctx.SetMainObject(dmaFile);

            if (!dmaFile.Load(ctx.assetPath)) {
                ctx.LogImportError($"Failed to load dma file. ({ctx.assetPath})");
                return;
            }

            var materials = dmaFile.CreateMaterialsFrom();
            foreach (var mat in materials) {
                // on the off chance that one of the shaders was not found, we need to make sure we dont add a null asset to the CTX
                if (mat == null) {
                    continue;
                }
                ctx.AddObjectToAsset(mat.name, mat);
            }
        }
    }
}
