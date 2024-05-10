// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "..\Components\ComponentsCommon.h"

namespace Quantum::script {

    DEFINE_TYPED_ID(script_id);

    class component final
    {
    public:
        constexpr explicit component(script_id id) : _id{ id } {}
        constexpr component() : _id{ id::invalid_id } {}
        constexpr script_id get_id() const { return _id; }
        constexpr bool is_valid() const { return id::is_valid(_id); }

        math::v4 rotation() const;
        math::v3 position() const;
        math::v3 scale() const;
    private:
        script_id _id;
    };
}
