Shader "Elektronik/MeshShaderLit"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1) 
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
            float _Metallic;
            float _Smoothness;
            half3 _Color;
            #define MAX_BRIGHTNESS 16

            struct VertexInput
            {
                uint vertex_id : SV_VertexID;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float3 world_pos : TEXTCOORD1;
                half3 normal : TEXTCOORD0;
                float2 bary_coord: TEXTCOORD2;
            };

            half3 decode_color(uint data)
            {
                half r = (data) & 0xff;
                half g = (data >> 8) & 0xff;
                half b = (data >> 16) & 0xff;
                half a = (data >> 24) & 0xff;
                return half3(r, g, b) * a * MAX_BRIGHTNESS / (255 * 255);
            }

            VertexOutput vertex_shader(const VertexInput input)
            {
                VertexOutput o;
                float4 pt = _VertsBuffer[input.vertex_id];
                o.world_pos = pt.xyz * _Scale;
                o.position = UnityObjectToClipPos(float4(o.world_pos, 1));
                o.normal = float3(0, 0, 0);
                o.bary_coord = float2(0, 0);
                return o;
            }

            [maxvertexcount(3)]
            void geometry_shader(triangle VertexOutput i[3], inout TriangleStream<VertexOutput> stream)
            {
                const float3 p0 = i[0].world_pos;
                const float3 p1 = i[1].world_pos;
                const float3 p2 = i[2].world_pos;
                const float3 triangle_normal = normalize(cross(p1 - p0, p2 - p0));
                i[0].normal = UnityObjectToWorldNormal(triangle_normal);
                i[1].normal = UnityObjectToWorldNormal(triangle_normal);
                i[2].normal = UnityObjectToWorldNormal(triangle_normal);
                i[0].bary_coord = float2(1, 0);
                i[1].bary_coord = float2(0, 1);
                stream.Append(i[0]);
                stream.Append(i[1]);
                stream.Append(i[2]);
            }

            half3 fragment_shader(VertexOutput i) : SV_Target
            {
                i.normal = normalize(i.normal);
                const float3 light_dir = _WorldSpaceLightPos0.xyz;
                float3 view_dir = normalize(_WorldSpaceCameraPos - i.world_pos);

                const float3 light_color = _LightColor0.rgb;
                float3 albedo = _Color;

                float3 specular_tint;
                float one_minus_reflectivity;
                albedo = DiffuseAndSpecularFromMetallic(albedo, _Metallic, specular_tint, one_minus_reflectivity);

                UnityLight light;
                light.color = light_color;
                light.dir = light_dir;
                light.ndotl = DotClamped(i.normal, light_dir);
                UnityIndirect indirect_light;
                indirect_light.diffuse = 0.1;
                indirect_light.specular = 0.1;

                float3 color = UNITY_BRDF_PBS(
                    albedo, specular_tint,
                    one_minus_reflectivity, _Smoothness,
                    i.normal, view_dir,
                    light, indirect_light
                );
                float3 barys;
                barys.xy = i.bary_coord;
                barys.z = 1 - barys.x - barys.y;
                float min_bary = min(barys.x, min(barys.y, barys.z));
                const float delta = fwidth(min_bary);
                min_bary = smoothstep(0.5 * delta, 1.5 * delta, min_bary);

                return color * min_bary;
            }
            ENDCG
        }
    }
}