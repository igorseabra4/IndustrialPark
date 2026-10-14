cbuffer data :register(b0)
{
	float4 color;
	// x = outline thickness in pixels
	float4 settings;
};

// Mask of the selected assets, rendered on its own. Any pixel with alpha > 0 belongs to a selected asset.
Texture2D selectionMask : register(t0);

// Fullscreen triangle generated from the vertex ID, no vertex buffer needed
float4 VS(uint id : SV_VertexID) : SV_POSITION
{
	float2 uv = float2((id << 1) & 2, id & 2);
	return float4(uv * float2(2, -2) + float2(-1, 1), 0, 1);
}

float4 PS(float4 position : SV_POSITION) : SV_Target
{
	int2 pixel = int2(position.xy);

	if (selectionMask.Load(int3(pixel, 0)).a > 0)
		discard;

	int radius = (int)settings.x;

	[loop]
	for (int y = -radius; y <= radius; y++)
	{
		[loop]
		for (int x = -radius; x <= radius; x++)
		{
			if (x * x + y * y > radius * radius)
				continue;
			// out of bounds Loads return 0
			if (selectionMask.Load(int3(pixel + int2(x, y), 0)).a > 0)
				return color;
		}
	}

	discard;
	return color;
}
