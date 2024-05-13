// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "ToolsCommon.h"

namespace Quantum::tools {

    struct mesh
    {
        // initial data
        util::vector<math::v3>          positions;
        util::vector<math::v3>          normals;
        util::vector<math::v3>          tangents;
        util::vector<math::v3>          uv_sets;

        util::vector<math::v3>          raw_indices;

        // Intermediate data
 
        // Output data
    };

    struct lod_group
    {
        std::string         name;
        util::vector<mesh>  meshes;
    };

    struct scene 
    {
        std::string              name;        
        util::vector<lod_group>  log_groups;
    };

    struct geometry_import_settings
    {
        f32 smothing_angle;
        u8 calculate_normals;
        u8 reverse_handedness;
        u8 import_embeded_textures;
        u8 import_animations;
    };

    struct scene_data
    {
        u8*                      buffer;
        u32                      buffer_size;
        geometry_import_settings settings;
    };
}
