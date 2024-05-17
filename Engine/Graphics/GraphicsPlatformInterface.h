// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "CommonHeaders.h"
#include "Renderer.h"

namespace Quantum::graphics {
    struct platform_interface
    {
        bool(*initialize)(void);
        void(*shutdown)(void);
        void(*render)(void);
    };
}
