using System;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes.Dme {
    [Serializable]
    public class MeshEntry {
        public int MaterialIndex;
        public int SubMeshCount;

        public int Unknown3;
        public int Unknown4;

        public int VertexSize;
        public int VertexCount;

        public int IndexSize;
        public int IndexCount;

        // TODO: consider not serializing this, it will consume a lot of data on disk...
        // The downside is that you cannot use it for editor tools (which should be fine, it should only be needed on import for now)
        [HideInInspector] public byte[] VertexBuffer;
        [HideInInspector] public byte[] IndexBuffer;
    }
}
