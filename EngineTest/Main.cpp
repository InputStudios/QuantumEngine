// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.

#pragma comment(lib, "Engine.lib")

#include <Windows.h>

#define TEST_ENTITY_COMPONENTS 1

#if TEST_ENTITY_COMPONENTS
#include "EntityComponentTest.h"
#else
#error One of the tests need to be enabled
#endif

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE hPrevInstance, LPSTR lpCmdLine, int nCmdShow) {
#if _DEBUG
	_CrtSetDbgFlag(_CRTDBG_ALLOC_MEM_DF | _CRTDBG_LEAK_CHECK_DF);
#endif
	engine_test test{};

	if (test.initialize()) 
	{
		test.run();
	}

	test.shutdown();
}