Shader "Elektronik/WireframeShader"
{
    Properties
    {
        _Color ("Color", Color) = (1, 1, 1, 1)
        _WireColor ("Wire color", Color) = (1, 1, 1, 1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma target 4.0
            #pragma vertex vert
            #pragma geometry geometry_shader
            #pragma fragment frag
            
            #include "UnityCG.cginc"

            float4 _Color;
            float4 _WireColor;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 bary_coord: TEXTCOORD2;
            };
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.bary_coord = float2(0, 0);
                return o;
            }
            
            [maxvertexcount(3)]
            void geometry_shader(triangle v2f i[3], inout TriangleStream<v2f> stream)
            {
                i[0].bary_coord = float2(1, 0);
                i[1].bary_coord = float2(0, 1);
                stream.Append(i[0]);
                stream.Append(i[1]);
                stream.Append(i[2]);
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3 barys;
                barys.xy = i.bary_coord;
                barys.z = 1 - barys.x - barys.y;
                float min_bary = min(barys.x, min(barys.y, barys.z));
                const float delta = fwidth(min_bary);
                min_bary = smoothstep(0.5 * delta, 1.5 * delta, min_bary);
                
                return _Color * min_bary + _WireColor * (1 - min_bary);
            }
            ENDCG
        }
    }
}