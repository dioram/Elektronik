Shader "Elektronik/LineCloudShader"
{
    Properties
    {
        _Alpha("Alpha", Float) = 1
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
            #define MAX_BRIGHTNESS 16

            half _Alpha;
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
                o.position = UnityObjectToClipPos(float4(pt.xyz, 1));
                o.color = DecodeColor(asuint(pt.w));
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
