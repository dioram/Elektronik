Shader "Elektronik/CubeCloudShader"
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

            #define VERTEX_COUNT 36

            #include "UnityCG.cginc"

            half _Scale;
            StructuredBuffer<float4x4> _TransformsBuffer;
            StructuredBuffer<float3> _ScalesBuffer;
            StructuredBuffer<float4> _ColorsBuffer;

            struct VertexInput
            {
                uint vertexID : SV_VertexID;
            };

            struct Cube
            {
                float4x4 transform: TEXCOORD8;
                float3 scale: TEXCOORD7;
                half3 color : COLOR1;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                half3 color : COLOR0;
                float3 localPos: TEXCOORD5;
                half2 barycentricCoords: TEXCOORD9;
            };

            Cube Vertex(VertexInput input)
            {
                Cube o;
                o.transform = _TransformsBuffer[input.vertexID];
                o.scale = _ScalesBuffer[input.vertexID];
                o.color = _ColorsBuffer[input.vertexID].rgb;
                return o;
            }


            bool IsInvalid(const in float3 f)
            {
                return f[0] == 0 && f[1] == 0 && f[2] == 0;
            }

            [maxvertexcount(VERTEX_COUNT)]
            void Geometry(point Cube input[1], inout TriangleStream<VertexOutput> stream)
            {
                const float4x4 transform = input[0].transform;
                const float3 scale = input[0].scale;
                
                if (IsInvalid(input[0].scale)) return;

                const float3 points[8] = {
                    float3( 1,  1,  1),
                    float3( 1,  1, -1),
                    float3( 1, -1,  1),
                    float3( 1, -1, -1),
                    float3(-1,  1,  1),
                    float3(-1,  1, -1),
                    float3(-1, -1,  1),
                    float3(-1, -1, -1),
                };

                const half2 bary[3] = {
                    half2(0, 0),
                    half2(0, 1),
                    half2(1, 0),
                };

                const uint indexes[VERTEX_COUNT] = {
                    0, 3, 1,
                    0, 3, 2,
                    0, 5, 1,
                    0, 5, 4,
                    0, 6, 2,
                    0, 6, 4,
                    7, 1, 3,
                    7, 1, 5,
                    7, 2, 3,
                    7, 2, 6,
                    7, 4, 5,
                    7, 4, 6,
                };

                for (uint i = 0; i < VERTEX_COUNT; i++)
                {
                    VertexOutput o;
                    const float3 pos = mul(transform, float4(points[indexes[i]] * scale * 0.5, 1));
                    o.position = UnityObjectToClipPos(pos * _Scale);
                    o.localPos = points[indexes[i]];
                    o.barycentricCoords = bary[i % 3];
                    o.color = input[0].color;
                    stream.Append(o);
                }
            }

            half3 Fragment(VertexOutput input) : SV_Target
            {
                const float x = abs(input.localPos.x);
                const float y = abs(input.localPos.y);
                const float z = abs(input.localPos.z);

                float x1, y1;
                if (abs(x - 1) < 0.001f)
                {
                    x1 = y;
                    y1 = z;
                }
                else if (abs(y - 1) < 0.001f)
                {
                    x1 = x;
                    y1 = z;
                }
                else
                {
                    x1 = x;
                    y1 = y;
                }

                float dist = pow(x1, 10) + pow(y1, 10) - pow(0.9, 10);
                return input.color * clamp(dist*5, 0.7, 1);
            }
            ENDCG
        }
    }
}