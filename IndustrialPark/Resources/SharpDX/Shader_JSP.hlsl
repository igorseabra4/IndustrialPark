cbuffer data : register(b0)
{
    float4x4 WorldViewProj;
    float4 MaterialColor;
    float4 FogColor;
    float FogStart;
    float FogEnd;
    bool FogEnable;
    bool VertexColorEnable;
    float4 SelectedColor;
};

struct VS_IN
{
    float4 position : POSITION;
    float4 color : COLOR;
    float4 texcoord : TEXCOORD;
};

struct PS_IN
{
    float4 position : SV_POSITION;
    float4 color : COLOR;
    float4 texcoord : TEXCOORD;
    float fogfactor : FOG;
};

Texture2D textureMap;
SamplerState textureSampler
{
    Filter = ANISOTROPIC;
    AddressU = Wrap;
    AddressV = Wrap;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN) 0;
	
    output.position = mul(WorldViewProj, input.position);
    output.texcoord = input.texcoord;
    output.fogfactor = saturate((FogEnd - output.position.z) / (FogEnd - FogStart));
    
    float4 finalColor;
    if (dot(SelectedColor, SelectedColor) == 0)
    {
        finalColor = VertexColorEnable ? input.color : float4(1, 1, 1, 1);
        finalColor *= MaterialColor;
    }
    else
    {
        finalColor = SelectedColor;
        finalColor.a = 1.0;
    }
    
    output.color = finalColor;

    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    float4 finalColor = input.color;
    finalColor *= textureMap.Sample(textureSampler, input.texcoord);
	
    if (dot(finalColor, finalColor) == 0)
        discard;

    if (FogEnable)
        finalColor.rgb = input.fogfactor * finalColor.rgb + (1.0 - input.fogfactor) * FogColor.rgb;
	
    return finalColor;
}