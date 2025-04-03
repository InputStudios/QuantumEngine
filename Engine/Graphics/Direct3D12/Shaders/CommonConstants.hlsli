// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
#if !defined(QUANTUM_COMMON_HLSLI) && !defined(__cplusplus)
#error Do not inclucde this header directly in shader files. Only include this file via Common.hlsli.
#endif

// Light types
// NOTE: these to be the same as Quantum::graphics::light::type enumeration!
static const uint LIGHT_TYPE_DIRECTIONAL_LIGHT = 0;
static const uint LIGHT_TYPE_POINT_LIGHT = 1;
static const uint LIGHT_TYPE_SPOTLIGHT = 2;
