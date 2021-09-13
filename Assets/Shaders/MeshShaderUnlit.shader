Shader "Elektronik/MeshShaderUnlit"
{
    Properties
    {
        _Smoothness ("Smoothness", Range(0, 1)) = 0.9
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
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
            #pragma target 3.0

            #pragma vertex vertex_shader
            #pragma geometry geometry_shader
            #pragma fragment fragment_shader
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityPBSLighting.cginc"

            half _Scale;
            half _Size;
            StructuredBuffer<float4> _VertsBuffer;
            StructuredBuffer<float4> _NormalsBuffer;
            float _Metallic;
            float _Smoothness;
            #define MAX_BRIGHTNESS 16

            struct VertexInput
            {
                uint vertex_id : SV_VertexID;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float2 bary_coord: TEXTCOORD2;
                half3 color: COLOR;
            };

            half3 decode_color(uint data)
            {
                const half r = (data) & 0xff;
                const half g = (data >> 8) & 0xff;
                const half b = (data >> 16) & 0xff;
                const half a = (data >> 24) & 0xff;
                return half3(r, g, b) * a * MAX_BRIGHTNESS / (255 * 255);
            }

            VertexOutput vertex_shader(const VertexInput input)
            {
                VertexOutput o;
                float4 pt = _VertsBuffer[input.vertex_id];
                o.position = UnityObjectToClipPos(float4(pt.xyz * _Scale, 1));
                o.bary_coord = float2(0, 0);
                o.color = decode_color(asuint(pt.w));
                return o;
            }

            [maxvertexcount(3)]
            void geometry_shader(triangle VertexOutput i[3], inout TriangleStream<VertexOutput> stream)
            {
                i[0].bary_coord = float2(1, 0);
                i[1].bary_coord = float2(0, 1);
                stream.Append(i[0]);
                stream.Append(i[1]);
                stream.Append(i[2]);
            }

            half3 fragment_shader(VertexOutput i) : SV_Target
            {
                float3 barys;
                barys.xy = i.bary_coord;
                barys.z = 1 - barys.x - barys.y;
                float min_bary = min(barys.x, min(barys.y, barys.z));
                const float delta = fwidth(min_bary);
                min_bary = smoothstep(0.5 * delta, 1.5 * delta, min_bary);

                return i.color * min_bary;
            }
            ENDCG
        }
    }
}