Shader "Elektronik/CrystalCloudShader"
{
    Properties
    {
        _Scale("Scale", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Cull Off
        ZWrite On
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma require geometry
            #pragma vertex MarkerVertexProgram
            #pragma geometry CubeGeometryProgram
            #pragma fragment Fragment
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "Markers.cginc"
            #include "Wireframe.cginc"

            struct FragmentInput
            {
                float4 position : SV_POSITION;
                float4 color : COLOR0;
                BARYCENTRIC_COORDINATES
            };

            #define VERTEX_COUNT 24

            [maxvertexcount(VERTEX_COUNT)]
            void CubeGeometryProgram(point Marker input[1], inout TriangleStream<FragmentInput> stream)
            {
                const float4x4 transform = input[0].transform;
                const float3 scale = input[0].scale;

                if (IsInvalid(scale)) return;

                const float3 points[6] = {
                    float3(0, 1, 0),
                    float3(0, -1, 0),
                    float3(0.5, 0, 0),
                    float3(0, 0, 0.5),
                    float3(-0.5, 0, 0),
                    float3(0, 0, -0.5),
                };

                const uint indexes[VERTEX_COUNT] = {
                    0, 2, 3,
                    0, 3, 4,
                    0, 4, 5,
                    0, 5, 2,
                    1, 2, 3,
                    1, 3, 4,
                    1, 4, 5,
                    1, 5, 2,
                };

                const half2 bary[3] = {
                    half2(0, 0),
                    half2(0, 1),
                    half2(1, 0),
                };

                for (uint i = 0; i < VERTEX_COUNT; i++)
                {
                    FragmentInput o;
                    const float3 pos = mul(transform, float4(points[indexes[i]] * scale * 0.5, 1));
                    o.position = UnityObjectToClipPos(pos * _Scale);
                    o.color = float4(input[0].color, 1);
                    o.barycentricCoordinates = bary[i % 3];
                    stream.Append(o);
                    if (i % 3 == 2) stream.RestartStrip();
                }
            }

            half3 Fragment(FragmentInput input) : SV_Target
            {
                return Wireframe(input.barycentricCoordinates, input.color * 0.7, input.color);
            }
            ENDCG
        }
    }
}