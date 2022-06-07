Shader "Elektronik/MeshShaderTransparent"
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
            #include "Wireframe.cginc"

            half _Scale;
            StructuredBuffer<float4> _VertsBuffer;
            StructuredBuffer<float4> _NormalsBuffer;

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                half3 color: COLOR;
                BARYCENTRIC_COORDINATES
            };

            VertexOutput vertex_shader(const VertexInput input)
            {
                VertexOutput o;
                float4 pt = _VertsBuffer[input.vertex_id];
                o.position = UnityObjectToClipPos(float4(pt.xyz * _Scale, 1));
                o.barycentricCoordinates = float2(0, 0);
                o.color = decode_color(asuint(pt.w));
                return o;
            }

            [maxvertexcount(3)]
            void geometry_shader(triangle VertexOutput i[3], inout TriangleStream<VertexOutput> stream)
            {
                i[0].barycentricCoordinates = float2(1, 0);
                i[1].barycentricCoordinates = float2(0, 1);
                stream.Append(i[0]);
                stream.Append(i[1]);
                stream.Append(i[2]);
            }

            half3 fragment_shader(VertexOutput i) : SV_Target
            {
                return Wireframe(i.barycentricCoordinates, float4(i.color, 0), float4(0, 0, 0, 0));
            }
            ENDCG
        }
    }
}