Shader "Elektronik/PointCloudShader"
{
    Properties
    {
        _PointSize("Point Size", Float) = 0.05
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        //Cull Off
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma vertex Vertex
            #pragma geometry Geometry
            #pragma fragment Fragment
            #pragma multi_compile _ _COMPUTE_BUFFER
            
            #include "UnityCG.cginc"
            #define MAX_BRIGHTNESS 16

            half _PointSize;
            StructuredBuffer<float4> _PointBuffer;
            
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
                float4 pt = _PointBuffer[input.vertexID];
                if (distance(pt.xyz, float3(0, 0, 0)) < 0.01f)
                {
                    o.position = float4(10000, 10000, 10000, 1);
                }
                else
                {
                    o.position = UnityObjectToClipPos(float4(pt.xyz, 1));
                }
                o.color = DecodeColor(asuint(pt.w));
                return o;
            }

            [maxvertexcount(4)]
            void Geometry(point VertexOutput input[1], inout TriangleStream<VertexOutput> outStream)
            {
                float4 origin = input[0].position;
                float2 extent = abs(UNITY_MATRIX_P._11_22 * _PointSize);

                // Top
                VertexOutput o = input[0];
                o.position.y = origin.y + extent.y;
                o.position.xzw = origin.xzw;
                outStream.Append(o);

                // Right
                o.position.x = origin.x + extent.x;
                o.position.yzw = origin.yzw;
                outStream.Append(o);

                // Left
                o.position.x = origin.x - extent.x;
                o.position.yzw = origin.yzw;
                outStream.Append(o);

                // Bottom
                o.position.y = origin.y - extent.y;
                o.position.xzw = origin.xzw;
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
