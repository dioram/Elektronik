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
            #include "ComputeShaders.cginc"

            half _Scale;
            half _Size;
            StructuredBuffer<float4> _VertsBuffer;

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float3 viewPos : TEXTCOORD;
                half3 color : COLOR0;
            };

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput o;
                float4 pt = _VertsBuffer[input.vertex_id];
                if (distance(pt.xyz, float3(0, 0, 0)) < 0.001f)
                {
                    o.position = float4(10000, 10000, 10000, 1);
                }
                else
                {
                    o.position = UnityObjectToClipPos(float4(pt.xyz * _Scale, 1));
                }
                o.color = decode_color(asuint(pt.w));
                o.viewPos = UnityObjectToViewPos(o.position);
                return o;
            }

            half3 Fragment(VertexOutput input) : SV_Target
            {
                float camDist = length(input.viewPos) / 10;
                return float4(input.color, 1) / camDist;
            }
            ENDCG
        }
    }
}