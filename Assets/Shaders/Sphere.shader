Shader "Elektronik/Sphere"
{
    Properties
    {
        _GridThickness("Grid Thickness", Float) = 0.01
        _Color("Color", Color) = (1, 1, 0)
    }
    SubShader
    {
        Tags
        {
            "RenderType"="Opaque"
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
                    length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)),
                    length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)),
                    length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))
                );
                return output;
            }


            float3 frag(vertexOutput input) : COLOR
            {
                const float x = abs(input.localPos.x * input.worldScale.x);
                const float y = abs(input.localPos.y * input.worldScale.y);
                const float z = abs(input.localPos.z * input.worldScale.z);
                if (x < _GridThickness || y < _GridThickness || z < _GridThickness)
                {
                    return _Color.rgb;
                }
                return _Color.rgb * 0.6;
            }
            ENDCG
        }
    }
}