// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "CommonHeaders.h"
#include "Renderer.h"
#include "Platform\Window.h"

namespace Quantum::graphics {
    struct platform_interface
    {
        bool(*initialize)(void);
        void(*shutdown)(void);

        struct {
            surface(*create)(platform::window);
            void(*remove)(surface_id);
            void(*resize)(surface_id, u32, u32);
            u32(*width)(surface_id);
            u32(*height)(surface_id);
            void(*render)(surface_id);
        } surface;
    };
}
