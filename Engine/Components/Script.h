// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "ComponentsCommon.h"

namespace Quantum::script {

    struct init_info
    {
        detail::script_creator script_creator;
    };

    component create(init_info info, game_entity::entity entity);
    void remove(component c);
}