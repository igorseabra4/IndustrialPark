#define MAX_DIRECTIONAL_LIGHTS 8

struct DirectionalLight
{
    float4 Direction;
    float4 Color;
};

cbuffer data :register(b0)
{
	float4x4 WorldViewProj;
    float4x4 World;
	float4 Color;
    float4 MaterialColor;
	float4 UVAnim;
    float4 FogColor;
    float FogStart;
    float FogEnd;
    bool FogEnable;
    bool VertexColorEnable;
    DirectionalLight Lights[8];
    float4 AmbientColor;
    bool LightingEnable;
    float DiffuseMult;
    float AmbientMult;
    float4 SelectedColor;
    float AlphaDiscard;
};


struct VS_IN
{
	float4 position : POSITION;
	float4 color : COLOR;
	float4 texcoord : TEXCOORD;
    float3 normal : NORMAL;
};

struct PS_IN
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
	float4 texcoord : TEXCOORD;
    float3 normal : NORMAL;
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
	PS_IN output = (PS_IN)0;
	
    output.position = mul(WorldViewProj, input.position);
	output.texcoord = input.texcoord + UVAnim;
    output.normal = normalize(mul((float3x3)World, input.normal));
    output.fogfactor = saturate((FogEnd - output.position.z) / (FogEnd - FogStart));
    
    float4 finalColor;
    if (dot(SelectedColor, SelectedColor) == 0)
    {
        finalColor = VertexColorEnable ? input.color : (LightingEnable ? float4(0, 0, 0, 1) : float4(1, 1, 1, 1));
        
        if (LightingEnable)
        {
            finalColor.rgb += AmbientColor.rgb * AmbientMult;
        
            float3 lightColor = float3(0, 0, 0);
            for (int i = 0; i < MAX_DIRECTIONAL_LIGHTS; i++)
            {
                DirectionalLight light = Lights[i];
                lightColor += max(dot(input.normal, normalize(light.Direction.xyz)), 0.0) * light.Color.rgb;
            }
            lightColor = min(lightColor, float3(1, 1, 1));
            finalColor.rgb += lightColor * DiffuseMult;
        }
    
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
	
	// Only testing the alpha channel causes certain tikis in movie to disappear
	// We discard fully transparent pixels with no RGB values
    //if (dot(finalColor, finalColor) == 0)
    //    discard;
    if (!(finalColor.a > AlphaDiscard))
        discard;

    if (FogEnable)
        finalColor.rgb = input.fogfactor * finalColor.rgb + (1.0 - input.fogfactor) * FogColor.rgb;
	
    return finalColor;
}