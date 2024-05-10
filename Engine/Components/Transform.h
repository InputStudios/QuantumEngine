// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma once
#include "ComponentsCommon.h"

namespace Quantum::transform {

	struct init_info
	{
		f32 position[3]{};
		f32 rotation[4]{};
		f32 scale[3]{1.f, 1.f, 1.f};
	};

	component create(init_info info, game_entity::entity entity);
	void remove(component c);
}