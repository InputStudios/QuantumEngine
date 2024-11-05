// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "D3D12CommonHeaders.h"

namespace Quantum::graphics::d3d12::shaders {

    struct shader_type {
        enum type : u32 {
            vertex = 0,
            hull,
            domain,
            geometry,
            pixel,
            compute,
            amplification,
            mesh,

            count
        };
    };

    struct engine_shader {
        enum id : u32 {
            fullscreen_triangle_vs = 0,
            fill_color_ps = 1,

            count
        };
    };

    bool initialize();
    void shutdown();

    D3D12_SHADER_BYTECODE get_engine_shader(engine_shader::id id);
}
