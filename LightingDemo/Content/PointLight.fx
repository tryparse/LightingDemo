#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler ColorRenderTarget;

float4 LightColor;
float2 LightWorldPosition;
float LightRadius;

float4x4 WorldMatrix;
float4x4 ViewProjection;

struct VertexShaderInput
{
    float4 Position: POSITION0;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

struct VertexShaderOutput
{
    float4 Position: POSITION0;
    float4 PosWorld: POSITION1;
    float2 TexCoords : TEXCOORD0;
    float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    float4 pos = mul(input.Position, WorldMatrix);
    output.PosWorld = pos; // handing over WorldSpace Coordinates to PS
    output.Position = mul(pos, ViewProjection);

    // fill other fields of output
    output.TexCoords = input.TexCoords;
    output.Color = input.Color;

    return output;
}

float4 PixelShaderLight(VertexShaderOutput input) : COLOR
{ 
	float4 texColor = tex2D(ColorRenderTarget, input.TexCoords);
	
	float d = distance(LightWorldPosition, input.PosWorld);

	if (d <= LightRadius)
	{
		float lightAmount = d / LightRadius;
		texColor.rgb *= LightColor.rgb * lightAmount;
	}

	return texColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL PixelShaderLight();
	}
};