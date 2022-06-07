#define BARYCENTRIC_COORDINATES float2 barycentricCoordinates: TEXTCOORD2;

float4 Wireframe(float2 barycentricCoords, float4 fillColor, float4 wireColor)
{
    float3 barys;
    barys.xy = barycentricCoords;
    barys.z = 1 - barys.x - barys.y;
    float min_bary = min(barys.x, min(barys.y, barys.z));
    const float delta = fwidth(min_bary);
    min_bary = smoothstep(0.5 * delta, 1.5 * delta, min_bary);
    return fillColor * min_bary + wireColor * (1 - min_bary);
}
