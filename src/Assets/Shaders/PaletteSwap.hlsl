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
    color = sqrt(color);
    float x = color.r / 8.0 + floor(frac(color.b * 8.0) * 8.0) / 8.0;
    float y = 1 - (color.g / 8.0 + floor(color.b * 8.0) / 8.0);
    
    if (time < 0.5)
    {
        float3 day = dayDayLUT.tex.Sample(dayDayLUT.samplerstate, float2(x, y));
        float3 dawn = dayDawnLUT.tex.Sample(dayDawnLUT.samplerstate, float2(x, y));
        colorOut = lerp(day, dawn, time * 2.0);
    } else
    {
        float3 dawn = dayDawnLUT.tex.Sample(dayDawnLUT.samplerstate, float2(x, y));
        float3 night = dayNightLUT.tex.Sample(dayNightLUT.samplerstate, float2(x, y));
        colorOut = lerp(dawn, night, time * 2.0 - 1.0);
    }
    
    alphaOut = color.a;
}
#endif
