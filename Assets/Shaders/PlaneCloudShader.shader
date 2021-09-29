Shader "Elektronik/PlaneCloudShader"
{
    Properties
    {
        _PointSize("Point Size", Float) = 0.05
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
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #define MAX_BRIGHTNESS 16

            half _Scale;
            half _Size;
            StructuredBuffer<float4> _VertsBuffer;

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
                float3 viewPos : TEXTCOORD;
                half3 color : COLOR0;
            };

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput o;
                float4 pt = _VertsBuffer[input.vertexID];
                if (distance(pt.xyz, float3(0, 0, 0)) < 0.001f)
                {
                    o.position = float4(10000, 10000, 10000, 1);
                }
                else
                {
                    o.position = UnityObjectToClipPos(float4(pt.xyz * _Scale, 1));
                }
                o.color = DecodeColor(asuint(pt.w));
                o.viewPos = UnityObjectToViewPos(o.position);
                return o;
            }

            half3 Fragment(VertexOutput input) : SV_Target
            {
                const float cam_dist = length(input.viewPos) / 10;
                return float4(input.color, 1) / cam_dist;
            }
            ENDCG
        }
    }
}