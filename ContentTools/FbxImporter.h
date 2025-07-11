// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
#pragma once
#include "ToolsCommon.h"
#include <fbxsdk.h>

namespace Quantum::tools {
	
    struct scene_data;
    struct scene;
    struct mesh;
    struct geometry_import_settings;
	
    class fbx_context {
        public:
			fbx_context(const char* file, scene* scene, scene_data* data, progression* const progression) :_scene{ scene }, _scene_data{ data }, _progression{ progression }
            {
                assert(file && _scene && _scene_data && _progression);
                if (initialize_fbx())
                {
                    load_fbx_file(file);
                }
            }
			
            ~fbx_context()
            {
                _fbx_scene->Destroy();
                _fbx_manager->Destroy();
                ZeroMemory(this, sizeof(fbx_context));
            }
			
            void get_scene(FbxNode* node = nullptr);
			  
            constexpr bool is_valid() const { return _fbx_manager && _fbx_scene; }
            constexpr f32 scene_scale() const { return _scene_scale; }
			constexpr progression* get_progression() const { return _progression; }
        private:
			
            bool initialize_fbx();
            void load_fbx_file(const char* file);
            void get_meshes(FbxNode* node, util::vector<mesh>& meshes, u32 lod_id, f32 lod_threshold);
            void get_mesh(FbxNodeAttribute* attribute, util::vector<mesh>& meshes, u32 lod_id, f32 lod_threshold);
            void get_lod_group(FbxNodeAttribute* attribute);
            bool get_mesh_data(FbxMesh* fbx_mesh, mesh& m);
			
            scene*              _scene{ nullptr };
            scene_data*         _scene_data{ nullptr };
            FbxManager*         _fbx_manager{ nullptr };
            FbxScene*           _fbx_scene{ nullptr };
			progression*		_progression{ nullptr };
            f32                 _scene_scale{ 1.0f };
    };
}
