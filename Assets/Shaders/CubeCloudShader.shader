Shader "Elektronik/CubeCloudShader"
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
        ZWrite On
        Pass
        {
            Tags
            {
                "LightMode"="ForwardBase"
            }
            CGPROGRAM
            #pragma require geometry
            #pragma vertex MarkerVertexProgram
            #pragma geometry CubeGeometryProgram
            #pragma fragment Fragment
            #pragma multi_compile _ _COMPUTE_BUFFER

            #include "Cube.cginc"

            half3 Fragment(FragmentInput input) : SV_Target
            {
                if (IsGrid(input))
                {
                    return input.color;
                }
                return input.color * 0.7;
            }
            ENDCG
        }
    }
}