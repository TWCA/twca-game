//UNITY_SHADER_NO_UPGRADE
#ifndef PALETTE_SWAP_INCLUDED
#define PALETTE_SWAP_INCLUDED

void paletteSwap_float(
    float4 color, 
    float time,
    UnityTexture2D dayDayLUT,
    UnityTexture2D dayDawnLUT,
    UnityTexture2D dayNightLUT,
    
    out float3 colorOut,
    out float alphaOut
    )
{
    float3 raw = sqrt(color.rgb);
    
    int x = int(raw.r * 64.0) + (int(raw.b * 64.0) & 7) * 64;
    int y = 511 - int(raw.g * 64.0) - (int(raw.b * 64.0) >> 3) * 64;
    
    if (time < 0.5)
    {
        float3 day = dayDayLUT.tex.Load(int3(x, y, 0));
        float3 dawn = dayDawnLUT.tex.Load(int3(x, y, 0));
        colorOut = lerp(day, dawn, time * 2.0);
    } else
    {
        float3 dawn = dayDawnLUT.tex.Load(int3(x, y, 0));
        float3 night = dayNightLUT.tex.Load(int3(x, y, 0));
        colorOut = lerp(dawn, night, time * 2.0 - 1.0);
    }
    
    alphaOut = color.a;
}
#endif
