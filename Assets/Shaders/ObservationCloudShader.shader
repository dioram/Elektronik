Shader "Elektronik/ObservationCloudShader"
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
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma geometry Geometry
            #pragma multi_compile _ _COMPUTE_BUFFER

            #define VERTEX_COUNT 18
            #define INTERNAL_SCALE 0.1

            #include "UnityCG.cginc"

            half _Scale;
            StructuredBuffer<float4x4> _TransformsBuffer;
            StructuredBuffer<float4> _ColorsBuffer;

            struct VertexInput
            {
                uint vertexID : SV_VertexID;
            };

            struct Observation
            {
                float4x4 transform: TEXCOORD8;
                half3 color : COLOR1;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                half3 color : COLOR0;
                half2 barycentricCoords: TEXCOORD9;
            };

            Observation Vertex(VertexInput input)
            {
                Observation o;
                o.transform = _TransformsBuffer[input.vertexID];
                o.color = _ColorsBuffer[input.vertexID].rgb;
                return o;
            }

            bool IsInvalid(const in float4x4 m)
            {
                return m[0][0] == 0 && m[0][1] == 0 && m[0][2] == 0 && m[0][3] == 0
                    && m[1][0] == 0 && m[1][1] == 0 && m[1][2] == 0 && m[1][3] == 0
                    && m[2][0] == 0 && m[2][1] == 0 && m[2][2] == 0 && m[2][3] == 0
                    && m[3][0] == 0 && m[3][1] == 0 && m[3][2] == 0 && m[3][3] == 0;
            }

            [maxvertexcount(VERTEX_COUNT)]
            void Geometry(point Observation input[1], inout TriangleStream<VertexOutput> stream)
            {
                const float4x4 transform = input[0].transform;
                
                if (IsInvalid(transform)) return;
                
                const float3 points[5] = {
                    float3(0, 0, -1),
                    float3(0.707, 0.707, 1 / 3.0),
                    float3(0.707, -0.707, 1 / 3.0),
                    float3(-0.707, -0.707, 1 / 3.0),
                    float3(-0.707, 0.707, 1 / 3.0)
                };
                
                const half2 bary[3] = {
                    half2(0, 0),
                    half2(0, 1),
                    half2(1, 0),
                };
                
                const uint indexes[VERTEX_COUNT] = {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 4,
                    0, 4, 1,
                    1, 3, 2,
                    3, 1, 4
                };

                const float4 T = float4(transform[0][3] * _Scale, transform[1][3] * _Scale, transform[2][3] * _Scale, 1);

                const float4x4 RS = float4x4(
                    transform[0][0], transform[0][1], transform[0][2], 0,
                    transform[1][0], transform[1][1], transform[1][2], 0,
                    transform[2][0], transform[2][1], transform[2][2], 0,
                    transform[3][0], transform[3][1], transform[3][2], 1
                );
                
                for (uint i = 0; i < VERTEX_COUNT; i++)
                {
                    VertexOutput o;
                    const float3 pos = mul(RS, float4(points[indexes[i]] * INTERNAL_SCALE, 1));
                    o.position = UnityObjectToClipPos(pos + T);
                    o.barycentricCoords = bary[i % 3];
                    o.color = input[0].color;
                    stream.Append(o);
                }
            }

            half3 Fragment(VertexOutput input) : SV_Target
            {
                float3 barys;
                barys.xy = input.barycentricCoords;
                barys.z = 1 - barys.x - barys.y;
                float minBary = min(barys.x, min(barys.y, barys.z));
                float delta = abs(ddx(minBary)) + abs(ddy(minBary));
                minBary = smoothstep(0, delta, minBary);
                minBary = clamp(minBary*2, 0.7, 1);
                return input.color * minBary + (1 - input.color) * (1 - minBary);
            }
            ENDCG
        }
    }
}