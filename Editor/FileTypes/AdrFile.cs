#nullable enable

using System;
using System.Buffers.Binary;
using System.IO;
using System.Linq;
using UnityEngine;

namespace ForgeLightToolkit.Editor.FileTypes
{
    public class AdrFile : ScriptableObject
    {
        // TODO: use the Actor Tool Export, logic, facts, and knowledge (here in my garage) to parse as much data as possible from the ADRs in FLTK
        public enum EnumPrimaryDataType : byte {
            Unknown = 0,
            FileName = 2,
            CollisionData = 0x0D,
        }

        public enum EnumFileNameType : byte {
            Unknown = 0,
            ModelFile = 1,
            MaterialFile = 2,
            UpdateRadius = 3,
        }

        public string? ModelFileName;
        public string? MaterialFileName;
        public float updateRadius;
        public string collisionFile;

        public bool Load(string filePath)
        {
            name = Path.GetFileNameWithoutExtension(filePath);

            var reader = new Reader(File.OpenRead(filePath));

            while (!reader.ReachedEnd)
            {
                var definitionType = (EnumPrimaryDataType) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();
                var definitionData = reader.ReadBytes(definitionSize);

                switch (definitionType)
                {
                    case EnumPrimaryDataType.FileName:
                        ParseModelDefinition(definitionData, filePath);
                        break;
                    case EnumPrimaryDataType.CollisionData:
                        ParseCollisionDefinition(definitionData, filePath);
                        break;
                }
            }

            return true;
        }

        private void ParseModelDefinition(byte[] data, string adrFilePath)
        {
            var reader = new Reader(data);

            while (!reader.ReachedEnd)
            {
                var definitionType = (EnumFileNameType) reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType)
                {
                    case EnumFileNameType.ModelFile:
                        ModelFileName = reader.ReadNullTerminatedString();
                        break;

                    case EnumFileNameType.MaterialFile:
                        MaterialFileName = reader.ReadNullTerminatedString();
                        break;

                    case EnumFileNameType.UpdateRadius:
                        updateRadius = BitConverter.ToSingle(reader.ReadBytes(4).Reverse().ToArray());
                        Debug.Log($"{adrFilePath} ur {updateRadius} ");
                        break;

                    default:
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }

        private void ParseCollisionDefinition(byte[] data, string adrFilePath) {
            var reader = new Reader(data);

            while (!reader.ReachedEnd) {
                var definitionType = reader.ReadByte();
                var definitionSize = reader.ReadCompressedLength();

                switch (definitionType) {
                    case 1:
                        collisionFile = reader.ReadNullTerminatedString();
                        break;

                    default:
                        reader.Skip(definitionSize);
                        break;
                }
            }
        }
    }
}
