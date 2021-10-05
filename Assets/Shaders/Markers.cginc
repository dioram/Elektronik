half _Scale;
StructuredBuffer<float4x4> _TransformsBuffer;
StructuredBuffer<float3> _ScalesBuffer;
StructuredBuffer<float4> _ColorsBuffer;

struct VertexInput
{
    uint vertexID : SV_VertexID;
};

struct Marker
{
    float4x4 transform: TEXCOORD8;
    float3 scale: TEXCOORD7;
    half3 color : COLOR1;
};

struct FragmentInput
{
    float4 position : SV_POSITION;
    half3 color : COLOR0;
    float3 localPos: TEXCOORD5;
    float3 scale: TEXCOORD6;
};
            
bool IsInvalid(const in float3 f)
{
    return f[0] == 0 && f[1] == 0 && f[2] == 0;
}

Marker MarkerVertexProgram(VertexInput input)
{
    Marker o;
    o.transform = _TransformsBuffer[input.vertexID];
    o.scale = _ScalesBuffer[input.vertexID];
    o.color = _ColorsBuffer[input.vertexID].rgb;
    return o;
}