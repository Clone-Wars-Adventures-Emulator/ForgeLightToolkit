using ForgeLightToolkit.Editor.FileTypes.Dme;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ForgeLightToolkit.Editor.Importers {
    // DMAs need images
    [ScriptedImporter(1, "dme", QueueOffsets.STAGE_1_POST_IMAGES)]
    public class DmeImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            if (string.IsNullOrEmpty(ctx.assetPath)) {
                ctx.LogImportError($"Invalid asset path. ({ctx.assetPath})");
                return;
            }

            var dmeFile = ScriptableObject.CreateInstance<DmeFile>();

            ctx.AddObjectToAsset("Dme", dmeFile);
            ctx.SetMainObject(dmeFile);

            // Logging the error here allows the DME file to still be searchable in the asset manager with t:dmeFile
            if (!dmeFile.Load(ctx.assetPath)) {
                ctx.LogImportError($"Failed to load dme file. ({ctx.assetPath})");
                return;
            }

            ctx.AddObjectToAsset("Dma", dmeFile.DmaFile);

            foreach (var submesh in dmeFile.submeshes){
                ctx.AddObjectToAsset(submesh.mesh.name, submesh.mesh);
            }

            var materials = dmeFile.DmaFile.CreateMaterialsFrom(dmeFile.name);
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
