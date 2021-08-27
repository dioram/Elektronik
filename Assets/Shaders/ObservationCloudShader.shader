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
        ZTest Less
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma geometry Geometry
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #define MAX_BRIGHTNESS 16

            half _Scale;
            StructuredBuffer<float4> _ItemsBuffer;

            half3 DecodeColor(uint data)
            {
                half r = (data) & 0xff;
                half g = (data >> 8) & 0xff;
                half b = (data >> 16) & 0xff;
                half a = (data >> 24) & 0xff;
                return half3(r, g, b) * a * MAX_BRIGHTNESS / (255 * 255);
            }

            struct VertexInput
            {
                uint vertexID : SV_VertexID;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                half3 color : COLOR0;
                float2 barycentricCoords: TEXCOORD9;
            };

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput o;
                float4 pt = _ItemsBuffer[input.vertexID];
                if (distance(pt.xyz, float3(0, 0, 0)) < 0.001f)
                {
                    o.position = float4(100000, 100000, 100000, 1);
                }
                else
                {
                    o.position = UnityObjectToClipPos(float4(pt.xyz * _Scale, 1));
                }
                o.color = DecodeColor(asuint(pt.w));
                o.barycentricCoords = float2(0, 0);
                return o;
            }

            [maxvertexcount(3)]
            void Geometry(triangle VertexOutput i[3], inout TriangleStream<VertexOutput> stream)
            {
                if (i[0].position.x > 99999) return;
                i[0].barycentricCoords = float2(1, 0);
                i[1].barycentricCoords = float2(0, 1);
                i[2].barycentricCoords = float2(0, 0);

                stream.Append(i[0]);
                stream.Append(i[1]);
                stream.Append(i[2]);
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