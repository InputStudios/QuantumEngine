using Editor.Components;
using Editor.GameProject;
using Editor.Utilities;
using EngineAPIStructs;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale;
    }

    [StructLayout(LayoutKind.Sequential)]
    class ScriptComponent
    {
        public IntPtr ScriptCreator;
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new();
        public ScriptComponent Script = new();
    }
}

namespace Editor.DLLWrapper
{
    class EngineAPI
    {
        private const string EngineDLL = "Engine.dll";

        [DllImport(EngineDLL, CharSet = CharSet.Ansi)]
        public static extern int LoadGameDLL(string dllpath);

        [DllImport(EngineDLL)]
        public static extern int UnloadGameDLL();

        [DllImport(EngineDLL)]
        public static extern IntPtr GetScriptCreator(string name);

        [DllImport(EngineDLL)]
        [return: MarshalAs(UnmanagedType.SafeArray)]
        public static extern string[] GetScriptNames();

        [DllImport(EngineDLL)]
        public static extern int CreateRenderSurface(IntPtr host, int width, int height);

        [DllImport(EngineDLL)]
        public static extern void RemoveRenderSurface(int surfaceID);

        [DllImport(EngineDLL)]
        public static extern IntPtr GetWindowHandle(int surfaceID);

        [DllImport(EngineDLL)]
        public static extern void ResizeRenderSurface(int surfaceID);

        internal static class EntityAPI
        {
            [DllImport(EngineDLL)]
            private static extern int CreateGameEntity(GameEntityDescriptor desc);

            public static int CreateGameEntity(GameEntity entity)
            {
                GameEntityDescriptor desc = new();

                // Transform component
                {
                    var c = entity.GetComponent<Transform>();
                    desc.Transform.Position = c.Position;
                    desc.Transform.Rotation = c.Rotation;
                    desc.Transform.Scale = c.Scale;
                }
                // Script component
                // {
                    // var c = entity.GetComponent<Script>();
                    // if (c != null && Project.Current)
                    // {
                    //     if (Project.Current.AvailableScripts.Contains(c.Name)) desc.Script.ScriptCreator = GetScriptCreator(c.Name);
                    //     else Logger.Log(MessageType.Error, $"Unable to find script with name '{c.Name}'. Game entity was created without script");
                    // }
                    return CreateGameEntity(desc);
                // }
            }

            [DllImport(EngineDLL)]
            private static extern void RemoveGameEntity(int id);

            public static void RemoveGameEntity(GameEntity entity)
            {
                RemoveGameEntity(entity.EntityId);
            }
        }
    }
}
