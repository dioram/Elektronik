Shader "Elektronik/MeshShaderUnlit"
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
            #pragma target 3.0
            #pragma require geometry
            #pragma vertex vertex_shader
            #pragma geometry geometry_shader
            #pragma fragment fragment_shader
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #include "ComputeShaders.cginc"

            half _Scale;
            StructuredBuffer<float4> _VertsBuffer;
            StructuredBuffer<float4> _NormalsBuffer;

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float2 bary_coord: TEXTCOORD2;
                half3 color: COLOR;
            };

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

                clip(0.5 - min_bary);

                return i.color * min_bary;
            }
            ENDCG
        }
    }
}