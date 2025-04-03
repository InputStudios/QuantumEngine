// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
#pragma once
#include "CommonHeaders.h"
#include "EngineAPI/Input.h"

namespace Quantum::input {

    void bind(input_source source);
    void unbind(input_source::type type, input_code::code code);
    void unbind(u64 binding);
    void set(input_source::type type, input_code::code code, math::v3 value);
}
