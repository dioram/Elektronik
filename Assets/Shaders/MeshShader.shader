Shader "Elektronik/MeshShader"
{
    Properties
    {
		_Tint ("Tint", Color) = (1, 1, 1, 1)
        _Smoothness ("Smoothness", Range(0, 1)) = 0.9
        [Gamma] _Metallic ("Metallic", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        ZWrite On
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma target 3.0
            
            #pragma vertex Vertex
            #pragma geometry MyGeometryProgram
            #pragma fragment Fragment
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "UnityCG.cginc"
            #include "UnityStandardBRDF.cginc"
            #include "UnityLightingCommon.cginc"
            #include "UnityPBSLighting.cginc"

            half _Size;
            StructuredBuffer<float4> _VertsBuffer;
            StructuredBuffer<float4> _NormalsBuffer;
            float _Metallic;
			float _Smoothness;
            float4 _Tint;

            struct VertexInput
            {
                uint vertexID : SV_VertexID;
            };

            struct VertexOutput
            {
                float4 position : SV_POSITION;
                float3 worldPos : TEXTCOORD1;
                half3 normal : TEXTCOORD0;
            };

            VertexOutput Vertex(VertexInput input)
            {
                VertexOutput o;
                float4 pt = _VertsBuffer[input.vertexID];
                o.worldPos = pt.xyz;
                o.position = UnityObjectToClipPos(float4(pt.xyz, 1));
                o.normal = -_NormalsBuffer[input.vertexID].xyz;
                return o;
            }

            [maxvertexcount(3)]
            void MyGeometryProgram(triangle VertexOutput i[3], inout TriangleStream<VertexOutput> stream)
            {
                float3 p0 = i[0].worldPos;
                float3 p1 = i[1].worldPos;
                float3 p2 = i[2].worldPos;
                float3 triangleNormal = normalize(cross(p1 - p0, p2 - p0));
                i[0].normal = UnityObjectToWorldNormal(triangleNormal);
                i[1].normal = UnityObjectToWorldNormal(triangleNormal);
                i[2].normal = UnityObjectToWorldNormal(triangleNormal);
                stream.Append(i[0]);
                stream.Append(i[1]);
                stream.Append(i[2]);
            }

            fixed3 Fragment(VertexOutput i) : SV_Target
            {
                i.normal = normalize(i.normal);
                float3 lightDir = _WorldSpaceLightPos0.xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - i.worldPos);

                float3 lightColor = _LightColor0.rgb;
                float3 albedo = _Tint.rgb;

                float3 specularTint;
                float oneMinusReflectivity;
                albedo = DiffuseAndSpecularFromMetallic(
                    albedo, _Metallic, specularTint, oneMinusReflectivity
                );

                UnityLight light;
                light.color = lightColor;
                light.dir = lightDir;
                light.ndotl = DotClamped(i.normal, lightDir);
                UnityIndirect indirectLight;
                indirectLight.diffuse = 0.1;
                indirectLight.specular = 0.1;

                return UNITY_BRDF_PBS(
                    albedo, specularTint,
                    oneMinusReflectivity, _Smoothness,
                    i.normal, viewDir,
                    light, indirectLight
                );
            }
            ENDCG
        }
    }
}