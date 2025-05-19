using UnityEditor.AssetImporters;
using UnityEditor;

namespace ForgeLightToolkit.Editor.Importers.Editors {
    [CustomEditor(typeof(CdtImporter))]
    public class CdtImporterEditor : ScriptedImporterEditor {
        public override void OnInspectorGUI() {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("invertMeshIndicies"));
            ApplyRevertGUI();
        }
    }
}

