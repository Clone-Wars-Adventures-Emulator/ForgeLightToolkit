#nullable enable

using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Agr {
    public class AgrFile : ScriptableObject {
        public ActorSet? ActorSet;

        public bool Load(string filePath) {
            name = Path.GetFileNameWithoutExtension(filePath);

            using var fileStream = File.OpenRead(filePath);

            var serializer = new XmlSerializer(typeof(ActorSet));

            if (serializer.Deserialize(fileStream) is not ActorSet actorSet) {
                return false;
            }

            ActorSet = actorSet;

            return true;
        }
    }
}
