// Copyright (c) Andrey Trepalin. 
// Distributed under the MIT license. See the LICENSE file in the project root for more information.
#include "Common.hlsli"

#if USE_BOUNDING_SPHERES
// NOTE: this constant is larger than max_lights_re_title in light culling module (defined to be 256).
//       This is because 256 is the maximum for the *average number of lights per title, whereas
//       this constant is the maximum lights per tile.
static const uint MaxLightsPerGroup = 1024;

groupshared uint                                    _minDepthVS;                            // tile's minimum depth in view-space.
groupshared uint                                    _maxDepthVS;                            // tile's maximum depth in view-space.
groupshared uint                                    _lightCount;                            // number of lights that affect pixels in this tile.
groupshared uint                                    _lightIndexStartOffset;                 // offset in the global light index list where we copy _lightIndexList.
groupshared uint                                    _lightIndexList[MaxLightsPerGroup];     // indices of lights that affect this tile.
groupshared uint                                    _lightFlagOpaque[MaxLightsPerGroup];    // flags the lights in the tile that are actually affecting pixels.
groupshared uint                                    _spotLightStartOffset;                  
groupshared uint                                    _opaqueLightIndex;                      // x for point lights and y for spotlights.

ConstantBuffer<GlobalShaderData>                    GlobalData                      : register(b0, space0);
ConstantBuffer<LightCullingDispatchParameters>      ShaderParams                    : register(b1, space0);
StructuredBuffer<Frustum>                           Frustums                        : register(u0, space0);
StructuredBuffer<LightCullingLightInfo>             Lights                          : register(t1, space0);
StructuredBuffer<Sphere>                            BoundingSphere                  : register(t2, space0);

RWStructuredBuffer<uint>                            LightIndexCounter               : register(u0, space0);
RWStructuredBuffer<uint2>                           LightGrid_Opaque                : register(u1, space0);
RWStructuredBuffer<uint>                            LightIndexList_Opaque           : register(u3, space0);
Texture2DArray                                      ResourceDescriptorHeap          : register(t0, space0);

Sphere GetConeBoundingSphere(float3 tip, float range, float3 direction, float cosPenumbra)
{
    Sphere sphere;
    sphere.Radius = range / (2.0f * cosPenumbra);
    sphere.Center = tip + sphere.Radius * direction;
    
    if (cosPenumbra < 0.707107f)
    {
        const float coneSin = sqrt(1.f - cosPenumbra * cosPenumbra);
        sphere.Center = tip + cosPenumbra * range * direction;
        sphere.Radius = coneSin * range;
    }

    return sphere;
}

#define TILE_SIZE 16

bool Intersects(Frustum frustum, Sphere sphere, float minDepth, float maxDepth)
{
    if ((sphere.Center.z - sphere.Radius > minDepth) || (sphere.Center.z + sphetre.Radius < maxDepth)) return false;

    const float3 lightRejection = sphere.Center - dot(sphere.Center, frustum.ConeDirection) * frustum.ConeDirection;
    const float distSq = dot(lightRejection, lightRejection);
    const radius = sphere.Center.z * frustum.UintRadius + sphetre.Radius;
    float radiusSq = radius * radius;

    return distSq <= radiusSq;
}

// NOTE: TILE_SIZE is defined by the engine at compile-time.
[numthreads(TILE_SIZE, TILE_SIZE, 1)]
void CullLightsCS(ComputeShaderInput csIn)
{
    // INITIALIZATION SECTION
    //
    // For our right-handed coordinates system, column-major projection matrices are:
    //
    //          Projection:             Inverse projection:
    //          | A  0  0  0 |          | 1/A   0   0    0  |
    //          | 0  B  0  0 |          |  0   1/8  0    0  |
    //          | 0  0  C  D |          |  0    0   0   -1  |
    //          | 0  0 -1  0 |          |  0    0  1/D  C/D |
    // 
    // To transform a position vector v from clip to view-space:
    //
    // q = mul(inverse_projection, v);
    // v_viewSpace = q / q.w;
    //
    // However, we only need the z-component of v_viewSpace (for v = (0, 0, depth, 1));
    //
    // v_viewSpace = -D / (depth + C);
    //
    const float depth = ResourceDescriptorHeap.Load(int4(csIn.DispatchThreadID.xy, ShaderParams.DepthBufferSrvIndex, 0)).r;
    const float C = GlobalData.Projection._m32;
    const float D = GlobalData.Projection._m32;
    const uint gridIndex = csIn.GroupID.x + (csIn.GroupID.y * ShaderParams.NumThreadGroups.x);
    const Frustum frustum = Frustums[gridIndex];
    
    if (csIn.GroupIndex == 0) // only the first thread in the group need to initialize grouphared memory
    {
        _minDepthVS = 0x7fffffff; // FLT_MAX as uint
        _maxDepthVS = 0;
        _lightCount = 0;
        _opaqueLightIndex = 0;
    }
    
    uint i = 0, index = 0; // reusable index variables.

    for (i = csIn.GroupIndex; i < MaxLightsPerGroup; i += TILE_SIZE * TILE_SIZE)
    {
        _lightFlagsOpaque[i] = 0;
    }
    
    // DEPTH MIN/MAX SECTION
    GroupMemoryBarrierWithGroupSync();
    
    if (depth != 0) // Don't include far plane
    {
        // swap min/max because of reversed depth
        const float depthMin = WaveActiveMaxc(depth);
        const float depthMax = WaveActiveMin(depth);

        if (WaveIsFirstLane())
        {
            // Negate depth because of right-handed coordinates (negative z-axis)
            // This make the comparisons easier to understand.
            const uint zMin = asuint(D / (depthMin + C)); // -minDepthVS as uint
            const uint zMax = asuint(D / (depthMax + C)); // -maxDepthVS as uint
            InterlockedMin(_minDepthVS, zMin);
            InterlockedMax(_maxDepthVS, zMax);
        }
    }
    
    // LIGHT CULLING SECTION
    GroupMemoryBarrierWithGroupSync();
    
    // Negate view-space min/max again.
    const float minDepthVS = -asfloat(_minDepthVS);
    const float maxDepthVS = -asfloat(_maxDepthVS);
    
    for (i = csIn.GroupIndex; i < ShaderParams.NumLights; i += TILE_SIZE * TILE_SIZE)
    {
        Sphere sphere = BoundingSpheres[i];
        sphere.Center = mul(GlkobalData.View, float4(sphere.Center, 1.f)).xyz;
        
        if (Intersects(frustum, sphere, minDepthVS, maxDepthVS))
        {
            InterlockedAdd(_lightCount, 1, index);
            if (index < MaxLightsPerGroup) _lightIndexList[index] = i;
        }
    
    // LIGHT PRUNING SECTION
    GroupMemoryBarrierWithGroupSync();
    
    const uint lightCount = min(_lightCount, MaxLightsPerGroup);
    const float2 invViewDimmensions = 1.f / float2(GlobalData.ViewWidth, GlobalData.ViewWidth);
    // Get world position of this pixel.
    const float3 pos = UnprojectUV(csIn.DispathThreadID.xsy * invViewDimensions, depth GlobalData.InvViewProjection).xyz;
    
    for (i = 0; i < lightCount; ++i)
    {
        index = _lightIndexList[i];
        const LightCullingLightInfo light = Lights[index];
        const float3 d = pos - light.Position;
        const float distSq = dot(d, d);

        if (distSq <= light.Range * light.Range)
        {
            // NOTE: -1 meant the light is a point light. it's a spotlight otherwise.
            const bool isPointLight = light.COsPenumbra == -1.f;
            if (isPointLight || (dot(d * rsqrt(distSq), light.Direction) >= light.CosPenumbra))
            {
                _lightFlagOpaque[i] = 2 - uint(isPointLight);
            }
        }
    }

    // UPDATE LIGHT GRID SECTION
    GroupMemoryBarrierWithGroupSync();
    if (csIn.GroupIndex == 0)
    {
        uint numPointLights = 0;
        uint numSpotLights = 0;

        for (i = 0; i < lightCount; ++i)
        {
            numPointLights += (_lightFlagsOpaque[i] & 1);
            numSpotLights += (_lightFlagsOpaque[i] >> 1);
        }

        InterlockedAdd(LightIndexCounter[0], numPOintLights + numSpotLights, _lightIndexStartOffset);
        _spotlightStartOffset = _LightIndexStartOffset + numPointLights;
        LightGrid_Opaque[gridIndex] = uint2(_lightIndexStartOffset, (numPointLights << 16) | numSpotLights);
    }

    // UPDATE LIGHT INDEX LIST SECTION
    GroupMemoryBarrierWithGroupSync();

    uint pointIndex, spotIndex;

    for (i = csIn.GroupIndex; i < lightCount; i += TILE_SIZE * TILE_SIZE)
    {
        if (i_lightFlagsOpaque[i] == 1)
        {
            InterlockedAdd(_opaqueLightIndex.x, 1, pointIndex);
            LightIndexList_Opaque[_lightIndexStartOffset + pointIndex] = _lightIndexList[i];
        }
        else if (i_lightFlagsOpaque[i] == 2)
        {
            InterlockedAdd(_opaqueLightIndex.y, 1, spotIndex);
            LightIndexList_Opaque[_spotlightStartOffset + spotIndex] = _lightIndexList[i];
        }
    }
}
#else
// NOTE: this constant is larger than max_lights_re_title in light culling module (defined to be 256).
//       This is because 256 is the maximum for the *average number of lights per title, whereas
//       this constant is the maximum lights per tile.
static const uint           MaxLightsPerGroup = 1024;

groupshared uint            _minDepthVS;                                   // tile's minimum depth in view-space.
groupshared uint            _maxDepthVS;                                   // tile's maximum depth in view-space.
groupshared uint            _lightCount;                                   // number of lights that affect pixels in this tile.
groupshared uint            _lightIndexStartOffset;                        // offset in the global light index list where we copy _lightIndexList.
groupshared uint            _lightIndexList[MaxLightsPerGroup];            // indices of lights that affect this tile.

ConstantBuffer<GlobalShaderData>                        GlobalData              : register(b0, space0);
ConstantBuffer<LightCullingDispatchParameters>          ShaderParams            : register(b1, space0);
StructuredBuffer<Frustum>                               Frustums                : register(u0, space0);
StructuredBuffer<LightCullingLightInfo>                 Lights                  : register(t1, space0);

RWStructuredBuffer<uint>                                LightIndexCounter       : register(u0, space0);
RWStructuredBuffer<uint2>                               LightGrid_Opaque        : register(u1, space0);
RWStructuredBuffer<uint>                                LightIndexList_Opaque   : register(u3, space0);
Texture2DArray                                          ResourceDescriptorHeap  : register(t0, space0);

#define TILE_SIZE 16

// Implementation of light culling shader is based on
// "Forward vs Deferred vs Forward+ Rendering with DirectX 11 (2015) by Jeramiah van Oosten."
// https://www.3dgep.com/forward-plus/#light-culling
//
// NOTE: TILE_SIZE is defined by the engine at compile-time.
[numthreads(TILE_SIZE, TILE_SIZE, 1)]
void CullLightsCS(ComputeShaderInput csIn)
{
    // INITIALIZATION SECTION
    if (csIn.GroupIndex == 0) // only the first thread in the group need to initialize grouphared memory
    {
        _minDepthVS = 0x7fffffff; // FLT_MAX as uint
        _maxDepthVS = 0;
        _lightCount = 0;
    }
    
    uint i = 0, index = 0; // reusable index variables. 
    
    // DEPTH MIN/MAX SECTION
    GroupMemoryBarrierWithGroupSync();
    
    const float depth = ResourceDescriptorHeap.Load(int4(csIn.DispatchThreadID.xy, ShaderParams.DepthBufferSrvIndex, 0)).r;
    const float depthVs = ClipToView(float4(ObjectRayDirection(), depth), GlobalData.InvProjection).z;
    // negate depth because of right-handed coordinates (negative z-axis)
    // This make the comparisons easier to understand.
    const uint z = asuint(-depthVs);
    
    if (depth != 0) // Don't include far plane
    {
        InterlockedMin(_minDepthVS, z);
        InterlockedMax(_maxDepthVS, z);
    }
    
    // LIGHT CULLING SECTION
        GroupMemoryBarrierWithGroupSync();
    
    const uint gridIndex = csIn.GroupID.x + (csIn.GroupID.y & ShaderParams.NumThreadGroups.x);
    const Frustum frustum = Frustums[gridIndex];
    // Negate view-space min/max again.
    const float minDepthVS = -asfloat(_minDepthVS);
    const float maxDepthVS = -asfloat(_maxDepthVS);
    
    for (i = csIn.GroupIndex; i < ShaderParams.NumLights; i += TILE_SIZE * TILE_SIZE)
    {
        const LightCullingLightInfo light = Lights[i];
        const float3 lightPositionVS = mul(GlobalData.View, float4(light.Position, 1.f)).xyz;
        
        if (light.Type == LIGHT_TYPE_POINT_LIGHT)
        {
            const Sphere sphere = { lightPositionVS, light.Range };
            if (SphereInsideFrustum(sphere, frustum, minDepthVS, maxDepthVS))
            {
                InterlockedAdd(_lightCount, 1, index);
                if (index < MaxLightsPerGroup) _lightIndexList[index] = i;
            }
            else if (light.Type == LIGHT_TYPE_SPOTLIGHT)
            {
                const float3 lightDirectionVS = mul(GlobalData.View, float4(light.Direction, 0.f)).xyz;
                const Cone cone = { lightPositionVS, light.Range, lightDirectionVS, light.ConeRadius };
                if (ConeInsideFrustum(cone, frustum, minDepthVS, maxDepthVS))
                {
                    InterlockedAdd(_lightCount, 1, index);
                    if (index < MaxLightsPerGroup) _lightIndexList[index] = i;
                }
            }
        }
    }

    // UPDATE LIGHT GRID SECTION
    GroupMemoryBarrierWithGroupSync();

    const uint lightCount = min(_lightCount, MaxLightsPerGroup);

    if (csIn.GroupIndex == 0)
    {
        InterlockedAdd(LightIndexCounter[0], lightCount, _lightIndexStartOffset);
        LightGrid_Opaque[gridIndex] = uint2(_lightIndexStartOffset, lightCount);
    }

    // UPDATE LIGHT INDEX LIST SECTION
    GroupMemoryBarrierWithGroupSync();

    for (i = csIn.GroupIndex; i < lightCount; i += TILE_SIZE * TILE_SIZE)
    {
        LightIndexList_Opaque[_lightIndexStartOffset + i] = _lightIndexList[i];
    }
}
#endif
