using System;

namespace ForgeLightToolkit.Editor.FileTypes.Gzne {
    [Serializable]
    public class FloraDefinition {
        public string Name;
        public string Texture;

        public bool Enabled;

        public void Deserialize(Reader reader) {
            Name = reader.ReadNullTerminatedString();
            Texture = reader.ReadNullTerminatedString();

            Enabled = reader.ReadBool();
        }
    }
}
