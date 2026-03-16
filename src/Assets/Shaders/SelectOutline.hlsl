//UNITY_SHADER_NO_UPGRADE
#ifndef SELECT_OUTLINE_INCLUDED
#define SELECT_OUTLINE_INCLUDED


void selectOutline_float(
    UnityTexture2D mainTexture,
    float4 uv,
    float4 outlineColor,
    float outlineThickness,
    
    out float3 colorOut,
    out float alphaOut
    )
{
    float4 color = mainTexture.tex.Sample(mainTexture.samplerstate, uv.xy);
    
    if (color.a < 0.5)
    {
        float2 leftUV = uv.xy + float2(-outlineThickness, 0) * mainTexture.texelSize.xy;
        float2 rightUV = uv.xy + float2(outlineThickness, 0) * mainTexture.texelSize.xy;
        float2 downUV = uv.xy + float2(0, -outlineThickness) * mainTexture.texelSize.xy;
        float2 upUV = uv.xy + float2(0, outlineThickness) * mainTexture.texelSize.xy;
        float2 leftDownUV = uv.xy + float2(-outlineThickness, -outlineThickness) * mainTexture.texelSize.xy;
        float2 righDowntUV = uv.xy + float2(outlineThickness, -outlineThickness) * mainTexture.texelSize.xy;
        float2 leftUpUV = uv.xy + float2(-outlineThickness, outlineThickness) * mainTexture.texelSize.xy;
        float2 righUpUV = uv.xy + float2(outlineThickness, outlineThickness) * mainTexture.texelSize.xy;
    
        if (
            mainTexture.tex.Sample(mainTexture.samplerstate, leftUV).a != 0 ||
            mainTexture.tex.Sample(mainTexture.samplerstate, rightUV).a != 0 ||
            mainTexture.tex.Sample(mainTexture.samplerstate, downUV).a != 0 ||
            mainTexture.tex.Sample(mainTexture.samplerstate, upUV).a != 0 ||
            mainTexture.tex.Sample(mainTexture.samplerstate, leftDownUV).a != 0 ||
            mainTexture.tex.Sample(mainTexture.samplerstate, righDowntUV).a != 0 ||
            mainTexture.tex.Sample(mainTexture.samplerstate, leftUpUV).a != 0 ||
            mainTexture.tex.Sample(mainTexture.samplerstate, righUpUV).a != 0)
        {
            color = outlineColor;
        }
    }
    
    colorOut = color.rgb;
    alphaOut = color.a;
}
#endif
