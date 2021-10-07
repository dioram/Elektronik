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