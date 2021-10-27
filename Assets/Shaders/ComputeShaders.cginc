#define MAX_BRIGHTNESS 16

struct VertexInput
{
    uint vertex_id : SV_VertexID;
};

half3 decode_color(uint data)
{
    const half r = (data) & 0xff;
    const half g = (data >> 8) & 0xff;
    const half b = (data >> 16) & 0xff;
    const half a = (data >> 24) & 0xff;
    return half3(r, g, b) * a * MAX_BRIGHTNESS / (255 * 255);
}
