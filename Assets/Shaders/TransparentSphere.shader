Shader "Elektronik/TransparentSphere"
{
    Properties
    {
        _GridThickness("Grid Thickness", Float) = 0.01
        _Color("Color", Color) = (1, 1, 0, 1)
    }
    SubShader
    {
        Tags
        {
            "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"
        }
        Pass
        {
            Cull Off
            ZWrite On
            CGPROGRAM
            #pragma require geometry
            #pragma vertex vert
            #pragma fragment frag
            
            uniform float _GridThickness;
            uniform float4 _Color;

            struct vertexInput
            {
                float4 vertex : POSITION;
            };

            struct vertexOutput
            {
                float4 pos: SV_POSITION;
                float4 worldPos: TEXCOORD0;
                float3 localPos: POSITION1;
                float3 worldScale: SCALE;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                output.pos = UnityObjectToClipPos(input.vertex);
                output.worldPos = mul(unity_ObjectToWorld, input.vertex);
                output.localPos = input.vertex.xyz;
                output.worldScale = float3(
                    length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
                    length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
                    length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
                );
                return output;
            }


            float4 frag(vertexOutput input) : COLOR
            {
                const float x = abs(input.localPos.x * input.worldScale.x);
                const float y = abs(input.localPos.y * input.worldScale.y);
                const float z = abs(input.localPos.z * input.worldScale.z);
                clip((x < _GridThickness || y < _GridThickness || z < _GridThickness)-1);
                return _Color;
            }
            ENDCG
        }
        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            
            uniform float4 _Color;

            struct vertexInput
            {
                float4 vertex : POSITION;
            };

            struct vertexOutput
            {
                float4 pos: SV_POSITION;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                output.pos = UnityObjectToClipPos(input.vertex);
                return output;
            }

            float4 frag(vertexOutput input) : COLOR
            {
                return float4(_Color.rgb, 0.3);
            }
            ENDCG
        }
    }
}