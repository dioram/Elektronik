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
            #pragma vertex vert
            #pragma geometry geometry_shader
            #pragma fragment frag
            
            #include "UnityCG.cginc"
            #include "Wireframe.cginc"

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
                return Wireframe(i.bary_coord, _Color, _WireColor);
            }
            ENDCG
        }
    }
}