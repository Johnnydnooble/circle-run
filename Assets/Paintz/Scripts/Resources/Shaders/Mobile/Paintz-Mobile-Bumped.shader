// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Simplified Bumped shader. Differences from regular Bumped one:
// - no Main Color
// - Normalmap uses Tiling/Offset of the Base texture
// - fully supports only 1 directional light. Other lights can affect it, but it will be per-vertex/SH.

Shader "Paint/Mobile/Bumped Diffuse" {
Properties {
    _MainTex ("Base (RGB)", 2D) = "white" {}
    _BumpMap ("Normalmap", 2D) = "bump" {}
		_BumpPower("Normal Scale", Range(.001,10)) = 1.0

		_SplatColor1("Splat Color 1", Color) = (1,.5,0,1)
		_SplatColor2("Splat Color 2", Color) = (1,0,0,1)
		_SplatColor3("Splat Color 3", Color) = (0,1,0,1)
		_SplatColor4("Splat Color 4", Color) = (0,0,1,1)

		_SplatTex("Splat Texture", 2D) = "black" {}
		_SplatTileNormalTex("Splat Normal", 2D) = "bump" {}
		_SplatTileBump("Splat Normal Scale", Range(0.001,10)) = 1.0
		_SplatEdgeBump("Splat Edge Scale", Range(0.001,10)) = 1.0
		_SplatEdgeBumpWidth("Splat Edge Width", Range(0,10)) = 1.0
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 250

CGPROGRAM
#pragma surface surf Lambert noforwardadd

static const float _Clip = 0.5;

sampler2D _MainTex;
sampler2D _BumpMap;
sampler2D _SplatTex;
sampler2D _SplatTileNormalTex;
sampler2D _WorldTangentTex;
sampler2D _WorldBinormalTex;

float _SplatEdgeBump;
float _SplatEdgeBumpWidth;
float _SplatTileBump;

fixed4 _SplatColor1;
fixed4 _SplatColor2;
fixed4 _SplatColor3;
fixed4 _SplatColor4;

half _BumpPower;

float4 _SplatTex_TexelSize;
float4 _SplatTileNormalTex_ST;
float4 _BumpMap_ST;

struct Input {
    float2 uv_MainTex;
	float2 uv2_SplatTex;
	float3 worldNormal;
	float3 worldTangent;
	float3 worldBinormal;
	float3 worldPos;
	INTERNAL_DATA
};

void surf (Input IN, inout SurfaceOutput o) {


	// Sample splat map texture with offsets
	float4 splatSDF = tex2D(_SplatTex, IN.uv2_SplatTex);
	float4 splatSDFx = tex2D(_SplatTex, IN.uv2_SplatTex + float2(_SplatTex_TexelSize.x, 0));
	float4 splatSDFy = tex2D(_SplatTex, IN.uv2_SplatTex + float2(0, _SplatTex_TexelSize.y));

	// Use ddx ddy to figure out a max clip amount to keep edge aliasing at bay when viewing from extreme angles or distances
	half splatDDX = length(ddx(IN.uv2_SplatTex * _SplatTex_TexelSize.zw));
	half splatDDY = length(ddy(IN.uv2_SplatTex * _SplatTex_TexelSize.zw));
	half clipDist = sqrt(splatDDX * splatDDX + splatDDY * splatDDY);
	half clipDistHard = max(clipDist * 0.01, 0.01);
	half clipDistSoft = 0.01 * _SplatEdgeBumpWidth;

	// Smoothstep to make a soft mask for the splats
	float4 splatMask = smoothstep((_Clip - 0.01) - clipDistHard, (_Clip - 0.01) + clipDistHard, splatSDF);
	float splatMaskTotal = max(max(splatMask.x, splatMask.y), max(splatMask.z, splatMask.w));

	// Smoothstep to make the edge bump mask for the splats
	float4 splatMaskInside = smoothstep(_Clip - clipDistSoft, _Clip + clipDistSoft, splatSDF);
	splatMaskInside = max(max(splatMaskInside.x, splatMaskInside.y), max(splatMaskInside.z, splatMaskInside.w));

	// Create normal offset for each splat channel
	float4 offsetSplatX = splatSDF - splatSDFx;
	float4 offsetSplatY = splatSDF - splatSDFy;

	// Combine all normal offsets into single offset
	float2 offsetSplat = lerp(float2(offsetSplatX.x, offsetSplatY.x), float2(offsetSplatX.y, offsetSplatY.y), splatMask.y);
	offsetSplat = lerp(offsetSplat, float2(offsetSplatX.z, offsetSplatY.z), splatMask.z);
	offsetSplat = lerp(offsetSplat, float2(offsetSplatX.w, offsetSplatY.w), splatMask.w);
	offsetSplat = normalize(float3(offsetSplat, 0.0001)).xy; // Normalize to ensure parity between texture sizes
	offsetSplat = offsetSplat * (1.0 - splatMaskInside) * _SplatEdgeBump;

	// Add some extra bump over the splat areas
	float2 splatTileNormalTex = tex2D(_SplatTileNormalTex, TRANSFORM_TEX(IN.uv2_SplatTex, _SplatTileNormalTex) * 10.0).xy;
	//offsetSplat += (splatTileNormalTex.xy - 0.5) * _SplatTileBump  * 0.2;
	offsetSplat += (splatTileNormalTex.xy - 0.5) * _SplatTileBump;

	// Create the world normal of the splats
#if 0
		// Use tangentless technique to get world normals
	float3 worldNormal = WorldNormalVector(IN, float3(0, 0, 1));
	float3 offsetSplatLocal2 = normalize(float3(offsetSplat, sqrt(1.0 - saturate(dot(offsetSplat, offsetSplat)))));
	float3 offsetSplatWorld = perturb_normal(offsetSplatLocal2, worldNormal, normalize(IN.worldPos - _WorldSpaceCameraPos), IN.uv2_SplatTex);
#else
		// Sample the world tangent and binormal textures for texcoord1 (the second uv channel)
		// you could skip the binormal texture and cross the vertex normal with the tangent texture to get the bitangent
	float3 worldTangentTex = tex2D(_WorldTangentTex, IN.uv2_SplatTex).xyz * 2.0 - 1.0;
	float3 worldBinormalTex = tex2D(_WorldBinormalTex, IN.uv2_SplatTex).xyz * 2.0 - 1.0;

	// Create the world normal of the splats
	float3 offsetSplatWorld = offsetSplat.x * worldTangentTex + offsetSplat.y * worldBinormalTex;
#endif

	// Get the tangent and binormal for the texcoord0 (this is just the actual tangent and binormal that comes in from the vertex shader)
	float3 worldTangent = WorldNormalVector(IN, float3(1, 0, 0));
	float3 worldBinormal = WorldNormalVector(IN, float3(0, 1, 0));

	// Convert the splat world normal to tangent normal for texcood0
	float2 offsetSplatLocal = 0;
	offsetSplatLocal.x = dot(worldTangent, offsetSplatWorld);
	offsetSplatLocal.y = dot(worldBinormal, offsetSplatWorld);

	// sample the normal map from the main material
	float4 normalMap = tex2D(_BumpMap, TRANSFORM_TEX(IN.uv_MainTex, _BumpMap));
	normalMap.xyz = UnpackNormal(normalMap);
	normalMap.z = normalMap.z / _BumpPower;

	float3 tanNormal = normalMap.xyz;

	// Add the splat normal to the tangent normal
	tanNormal.xy += offsetSplatLocal * splatMaskTotal;
	tanNormal = normalize(tanNormal);

	// Albedo comes from a texture tinted by color
	//float4 MainTex = tex2D(_MainTex, IN.uv_MainTex);
	//fixed4 c = MainTex * _Color;
	float4 c = tex2D(_MainTex, IN.uv_MainTex);

	// Lerp the color with the splat colors based on the splat mask channels
	c.xyz = lerp(c.xyz, _SplatColor1.xyz, splatMask.x);
	c.xyz = lerp(c.xyz, _SplatColor2.xyz, splatMask.y);
	c.xyz = lerp(c.xyz, _SplatColor3.xyz, splatMask.z);
	c.xyz = lerp(c.xyz, _SplatColor4.xyz, splatMask.w);

	o.Albedo = c.rgb;
	o.Normal = tanNormal;
	o.Alpha = c.a;

    //fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
    //o.Albedo = c.rgb;
    //o.Alpha = c.a;
    //o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_MainTex));
}
ENDCG
}

FallBack "Mobile/Diffuse"
}
