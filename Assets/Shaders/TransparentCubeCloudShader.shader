Shader "Elektronik/TransparentCubeCloudShader"
{
    Properties
    {
        _Scale("Scale", Float) = 1
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
            #pragma vertex MarkerVertexProgram
            #pragma geometry CubeGeometryProgram
            #pragma fragment Fragment
            #pragma multi_compile _ _COMPUTE_BUFFER

            #define VERTEX_COUNT 36

            #include "Cube.cginc"

            half4 Fragment(FragmentInput input) : SV_Target
            {
                clip(IsGrid(input) - 1);
                return half4(input.color, 1);
            }
            ENDCG
        }
        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma require geometry
            #pragma vertex MarkerVertexProgram
            #pragma geometry CubeGeometryProgram
            #pragma fragment Fragment
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "Cube.cginc"

            half4 Fragment(FragmentInput input) : SV_Target
            {
                return half4(input.color.rgb, 0.3);
            }
            ENDCG
        }
    }
}