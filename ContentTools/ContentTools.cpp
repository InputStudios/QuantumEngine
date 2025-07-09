// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
#include "ToolsCommon.h"

namespace Quantum::tools
{
	extern void ShutDownTextureTools();
}

EDITOR_INTERFACE void ShutDownContentTools()
{
	using namespace Quantum::tools;
	ShutDownTextureTools();
}
