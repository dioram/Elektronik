Shader "Elektronik/LineCloudShader"
{
    Properties
    {
        _Alpha("Alpha", Float) = 1
        _Scale("Scale", Float) = 1
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Transparent"
            "Queue"="Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha
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
            #pragma vertex Vertex alpha
            #pragma fragment Fragment alpha
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #include "ComputeShaders.cginc"

            half _Alpha;
            half _Scale;
            StructuredBuffer<float4> _ItemsBuffer;

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                half3 color : COLOR0;
            };

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput o;
                float4 pt = _ItemsBuffer[input.vertex_id];
                o.position = UnityObjectToClipPos(float4(pt.xyz * _Scale, 1));
                o.color = decode_color(asuint(pt.w));
                return o;
            }

            half4 Fragment(VertexOutput input) : SV_Target
            {
                return half4(input.color, _Alpha);
            }
            ENDCG
        }
    }
}