// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

using Editor.Components;
using Editor.EngineAPIStructs;
using Editor.GameProject;
using Editor.Utilities;
using System;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;

namespace Editor.EngineAPIStructs
{
    [StructLayout(LayoutKind.Sequential)]
    class TransformComponent
    {
        public Vector3 Position;
        public Vector3 Rotation;
        public Vector3 Scale = new Vector3(1, 1, 1);
    }

    [StructLayout(LayoutKind.Sequential)]
    class ScriptComponent
    {
        public IntPtr ScriptCreator;
    }

    [StructLayout(LayoutKind.Sequential)]
    class GameEntityDescriptor
    {
        public TransformComponent Transform = new TransformComponent();
        public ScriptComponent Script = new ScriptComponent();
    }
}

namespace Editor.DLLWrappers
{
    static class EngineAPI
    {
        private const string EngineDLL = "EngineDll.dll";

        [DllImport(EngineDLL, CharSet = CharSet.Ansi)]
        public static extern int LoadGameCodeDll(string dllpath);

        [DllImport(EngineDLL)]
        public static extern int UnloadGameCodeDll();

        [DllImport(EngineDLL)]
        public static extern IntPtr GetScriptCreator(string name);

        [DllImport(EngineDLL)]
        [return: MarshalAs(UnmanagedType.SafeArray)]
        public static extern string[] GetScriptNames();

        [DllImport(EngineDLL)]
        public static extern int CreateRenderSurface(IntPtr host, int width, int height);

        [DllImport(EngineDLL)]
        public static extern void RemoveRenderSurface(int surfaceId);

        [DllImport(EngineDLL)]
        public static extern IntPtr GetWindowHandle(int surfaceId);

        [DllImport(EngineDLL)]
        public static extern void ResizeRenderSurface(int surfaceId);

        internal static class EntityAPI
        {
            [DllImport(EngineDLL)]
            private static extern int CreateGameEntity(GameEntityDescriptor desc);

            public static int CreateGameEntity(GameEntity entity)
            {
                GameEntityDescriptor desc = new GameEntityDescriptor();

                // Transform component
                {
                    var c = entity.GetComponent<Transform>();
                    desc.Transform.Position = c.Position;
                    desc.Transform.Rotation = c.Rotation;
                    desc.Transform.Scale = c.Scale;
                }
                // Script component
                {
                    // NOTE: here we also check if current project is not null, so we can tell whether the game code DLL
                    //       has been loaded or not. This way, creation of entities with a script component is deferred
                    //       until the DLL has been loaded.
                    var c = entity.GetComponent<Script>();
                    if (c != null && Project.Current != null)
                    {
                        if (Project.Current.AvailableScripts.Contains(c.Name)) desc.Script.ScriptCreator = GetScriptCreator(c.Name);
                        else Logger.Log(MessageType.Error, $"Unable to find script with name '{c.Name}'. Game entity was created without script component!");
                    }
                    return CreateGameEntity(desc);
                }
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
