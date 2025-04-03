// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using Editor.ContentToolSAPIStructs;
using Editor.Utilities;
using System;
using System.Diagnostics;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Editor.ContentToolSAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class GeometryImportSettings
    {
        public float SmoothingAngle = 178f;
        public byte CalculateNormals = 0;
        public byte CalculateTangents = 1;
        public byte ReverseHandedness = 0;
        public byte ImportEmbededTextures = 1;
        public byte ImportAnimations = 1;

        private byte ToByte(bool value) => value ? (byte)1 : (byte)0;

        public void FromContentSettings(Content.Geometry geometry)
        {
            var settings = geometry.ImportSettings;

            SmoothingAngle = settings.SmoothingAngle;
            CalculateNormals = ToByte(settings.CalculateNormals);
            CalculateTangents = ToByte(settings.CalculateTangents);
            ReverseHandedness = ToByte(settings.ReverseHandedness);
            ImportEmbededTextures = ToByte(settings.ImportEmbeddedTextures);
            ImportAnimations = ToByte(settings.ImportAnimations);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    class SceneData : IDisposable
    {
        public IntPtr Data;
        public int DataSize;
        public GeometryImportSettings ImportSettings = new GeometryImportSettings();

        public void Dispose()
        {
            Marshal.FreeCoTaskMem(Data);
            GC.SuppressFinalize(this);
        }

        ~SceneData()
        {
            Dispose();
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    class PrimitiveInitInfo
    {
        public Content.PrimitiveMeshType Type;
        public int SegmentX = 1;
        public int SegmentY = 1;
        public int SegmentZ = 1;
        public Vector3 Size = new Vector3(1f);
        public int LOD = 0;
    }
}

namespace Editor.DLLWrappers
{
    static class ContentToolsAPI
    {
        private const string ToolsDLL = "ContentTools.dll";

        private static void GeometryFromSceneData(Content.Geometry geometry, Action<SceneData> sceneDataGenearator, string failureMessage)
        {
            Debug.Assert(geometry != null);
            using var sceneData = new SceneData();
            try
            {
                sceneData.ImportSettings.FromContentSettings(geometry);
                sceneDataGenearator(sceneData);
                Debug.Assert(sceneData.Data != IntPtr.Zero && sceneData.DataSize > 0);
                var data = new byte[sceneData.DataSize];
                Marshal.Copy(sceneData.Data, data, 0, sceneData.DataSize);
                geometry.FromRawData(data);
            }
            catch (Exception ex)
            {
                Logger.Log(MessageType.Error, failureMessage);
                Debug.WriteLine(ex.Message);
            }
        }

        [DllImport(ToolsDLL)]
        private static extern void CreatePrimitiveMesh([In, Out] SceneData data, PrimitiveInitInfo info);
        public static void CreatePrimitiveMesh(Content.Geometry geometry, PrimitiveInitInfo info)
        {
            GeometryFromSceneData(geometry, (sceneData) => CreatePrimitiveMesh(sceneData, info), $"Failed to create {info.Type} primitive mesh.");
        }

        [DllImport(ToolsDLL)]
        private static extern void ImportFbx(string file, [In, Out] SceneData data);

        public static void ImportFbx(string file, Content.Geometry geometry)
        {
            GeometryFromSceneData(geometry, (sceneData) => ImportFbx(file, sceneData), $"Failed to import from FBX file: {file}");
        }
    }
}
