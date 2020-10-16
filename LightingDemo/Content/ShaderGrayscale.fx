#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_4_0_level_9_1
	#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

sampler colorTexture;

struct PixelShaderInput
{
	float4 Position : SV_POSITION;
	float4 Color : COLOR0;
	float2 TextureCoordinate: TEXCOORD0;
};

float4 PixelShaderGrayscale(PixelShaderInput input) : COLOR
{ 
	float4 texColor = tex2D(colorTexture, input.TextureCoordinate);
	
	texColor.gb = texColor.r;
	return texColor;
}

technique BasicColorDrawing
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL PixelShaderGrayscale();
	}
};