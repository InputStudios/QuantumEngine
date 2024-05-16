// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
namespace Quantum::graphics {
    struct platform_interface;

    namespace d3d12 {
        void get_platform_interface(platform_interface& pi);
    }
}