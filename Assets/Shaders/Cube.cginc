#include "UnityCG.cginc"
#include "Markers.cginc"

#define VERTEX_COUNT 36

[maxvertexcount(VERTEX_COUNT)]
void CubeGeometryProgram(point Marker input[1], inout TriangleStream<FragmentInput> stream)
{
    const float4x4 transform = input[0].transform;
    const float3 scale = input[0].scale;

    if (IsInvalid(scale)) return;

    const float3 points[8] = {
        float3(1, 1, 1),
        float3(1, 1, -1),
        float3(1, -1, 1),
        float3(1, -1, -1),
        float3(-1, 1, 1),
        float3(-1, 1, -1),
        float3(-1, -1, 1),
        float3(-1, -1, -1),
    };

    const uint indexes[VERTEX_COUNT] = {
        0, 3, 1,
        0, 2, 3,
        0, 1, 5,
        0, 5, 4,
        0, 6, 2,
        0, 4, 6,
        7, 1, 3,
        7, 5, 1,
        7, 3, 2,
        7, 2, 6,
        7, 4, 5,
        7, 6, 4,
    };

    for (uint i = 0; i < VERTEX_COUNT; i++)
    {
        FragmentInput o;
        const float3 pos = mul(transform, float4(points[indexes[i]] * scale * 0.5, 1));
        o.position = UnityObjectToClipPos(pos * _Scale);
        o.localPos = points[indexes[i]];
        o.color = input[0].color;
        o.scale = scale;
        stream.Append(o);
        if (i % 3 == 2) stream.RestartStrip();
    }
}

bool IsGrid(FragmentInput input)
{
    const float x = abs(input.localPos.x);
    const float y = abs(input.localPos.y);
    const float z = abs(input.localPos.z);

    float x1, y1, sx, sy;
    if (abs(x - 1) < 0.001f)
    {
        x1 = y;
        y1 = z;
        sx = input.scale.y;
        sy = input.scale.z;
    }
    else if (abs(y - 1) < 0.001f)
    {
        x1 = x;
        y1 = z;
        sx = input.scale.x;
        sy = input.scale.z;
    }
    else
    {
        x1 = x;
        y1 = y;
        sx = input.scale.x;
        sy = input.scale.y;
    }

    return abs(x1) > (1 - 0.05 / sx) || abs(y1) > (1 - 0.05 / sy);
}
