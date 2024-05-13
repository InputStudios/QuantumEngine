// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "CommonHeaders.h"
#include "..\Platform\Window.h"

namespace Quantum::graphics {
    class surface {};

    struct render_surface
    {
        platform::window window{};
        surface surface{};
    };
}
