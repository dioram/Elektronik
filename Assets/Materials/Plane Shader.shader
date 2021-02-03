Shader "Elektronik/Plane Shader"
{
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
        }
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct input
            {
                float4 vertex: POSITION;
                float3 color: COLOR;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float3 worldPos : TEXTCOORD;
                float3 color : COLOR;
                float3 normal : TEXCOORD0;
            };

            v2f vert(input v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = UnityObjectToViewPos(v.vertex);
                o.color = v.color;
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float camDist = length(i.worldPos);
                return float4(i.color, 1) / camDist;
            }
            ENDCG
        }
    }
}