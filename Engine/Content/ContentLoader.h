// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "CommonHeaders.h"
#if !defined(SHIPPING)
namespace Quantum::content {
    bool load_game();
    void unload_game();
}
#endif // !defined(SHIPPING)
