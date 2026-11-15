using ForgeLightToolkit.Editor.FileTypes.Dsk;
using ForgeLightToolkit.Editor.XmlData;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace ForgeLightToolkit.Editor.Importers {
    [ScriptedImporter(1, "dsk")]
    public class DskImporter : ScriptedImporter {
        public override void OnImportAsset(AssetImportContext ctx) {
            if (string.IsNullOrEmpty(ctx.assetPath)) {
                ctx.LogImportError($"Invalid asset path. ({ctx.assetPath})");
            }

            var dskFile = ScriptableObject.CreateInstance<DskFile>();

            ctx.AddObjectToAsset("Dsk", dskFile);
            ctx.SetMainObject(dskFile);

            if (!dskFile.Load(ctx.assetPath)) {
                ctx.LogImportError($"Failed to load dsk file. ({ctx.assetPath})");
                return;
            }

            var skeleton = dskFile.CreateSkeletonObjects();
            ctx.AddObjectToAsset("Skeleton", skeleton);

            var masks = BoneMasks.Instance.CreateAnyMasks(dskFile.name, skeleton);
            foreach (var (name, mask) in masks) {
                ctx.AddObjectToAsset(name, mask);
            }
        }
    }
}
