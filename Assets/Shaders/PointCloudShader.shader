Shader "Elektronik/PointCloudShader"
{
    Properties
    {
        _Size("Point Size", Float) = 0.05
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
            #pragma target 4.0
            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment
            #pragma multi_compile _ _COMPUTE_BUFFER
            
            #include "UnityCG.cginc"
            #define MAX_BRIGHTNESS 16
            
            half _Scale;
            half _Size;
            StructuredBuffer<float4> _ItemsBuffer;
            
            half3 DecodeColor(uint data)
            {
                half r = (data      ) & 0xff;
                half g = (data >>  8) & 0xff;
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
            };

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput o;
                float4 pt = _ItemsBuffer[input.vertexID];
                if (distance(pt.xyz, float3(0, 0, 0)) < 0.001f)
                {
                    o.position = float4(10000, 10000, 10000, 1);
                }
                else
                {
                    o.position = UnityObjectToClipPos(float4(pt.xyz * _Scale, 1));
                }
                o.color = DecodeColor(asuint(pt.w));
                return o;
            }

            [maxvertexcount(8)]
            void Geometry(point VertexOutput input[1], inout TriangleStream<VertexOutput> outStream)
            {
                float4 origin = input[0].position;
                float2 extent = abs(UNITY_MATRIX_P._11_22 * _Size);

                int slices = 4;
                // Top
                VertexOutput o = input[0];
                o.position.y = origin.y + extent.y;
                o.position.xzw = origin.xzw;
                outStream.Append(o);

                UNITY_LOOP for (int i = 1; i < slices; i++)
                {
                    float sn, cs;
                    sincos(UNITY_PI / slices * i, sn, cs);

                    // Right side vertex
                    o.position.xy = origin.xy + extent * float2(sn, cs);
                    outStream.Append(o);

                    // Left side vertex
                    o.position.x = origin.x - extent.x * sn;
                    outStream.Append(o);
                }

                // Bottom vertex
                o.position.x = origin.x;
                o.position.y = origin.y - extent.y;
                outStream.Append(o);

                outStream.RestartStrip();
            }

            half3 Fragment(VertexOutput input) : SV_Target
            {
                return input.color;
            }

            ENDCG
        }
    }
}
