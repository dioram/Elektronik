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
            StructuredBuffer<float3> _PositionsBuffer;
            StructuredBuffer<float3x3> _RotationsBuffer;
            StructuredBuffer<float4> _ColorsBuffer;

            struct VertexInput
            {
                uint vertexID : SV_VertexID;
            };

            struct Observation
            {
                float3x3 rotation: TEXCOORD8;
                float3 position: TEXCOORD7;
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
                o.position = _PositionsBuffer[input.vertexID];
                o.rotation = _RotationsBuffer[input.vertexID];
                o.color = _ColorsBuffer[input.vertexID].rgb;
                return o;
            }

            bool IsInvalid(const in Observation o)
            {
                return o.rotation[0][0] == 0 && o.rotation[0][1] == 0 && o.rotation[0][2] == 0
                    && o.rotation[1][0] == 0 && o.rotation[1][1] == 0 && o.rotation[1][2] == 0
                    && o.rotation[2][0] == 0 && o.rotation[2][1] == 0 && o.rotation[2][2] == 0
                    && o.position[0] == 0    && o.position[1] == 0    && o.position[2] == 0
                    && o.color[0] == 0       && o.color[1] == 0       && o.color[2] == 0;
            }

            [maxvertexcount(VERTEX_COUNT)]
            void Geometry(point Observation input[1], inout TriangleStream<VertexOutput> stream)
            {
                if (IsInvalid(input[0])) return;
                
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
                
                for (uint i = 0; i < VERTEX_COUNT; i++)
                {
                    VertexOutput o;
                    const float3 pos = mul(input[0].rotation, points[indexes[i]] * INTERNAL_SCALE);
                    o.position = UnityObjectToClipPos(pos + input[0].position * _Scale);
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
                return input.color * minBary + (1 - input.color) * (1 - minBary);
            }
            ENDCG
        }
    }
}