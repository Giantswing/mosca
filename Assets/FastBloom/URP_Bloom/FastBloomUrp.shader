Shader "RufatShaderlab/FastBloomURP"
{
	Properties
	{
		 _MainTex("Source", 2D) = "white" {}
	}
	
	HLSLINCLUDE
	#pragma multi_compile_local _ _USE_RGBM
	#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
	TEXTURE2D_X(_BloomTemp);
	TEXTURE2D_X(_BloomTex);
	SAMPLER(sampler_BloomTex);

	half2 _Offset;
	half _BlurAmount;
	half4 _BloomColor;
	half4 _BloomData;

	struct appdata {
		half4 pos : POSITION;
		half2 uv : TEXCOORD0;
		UNITY_VERTEX_INPUT_INSTANCE_ID
	};

	struct v2f {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	struct v2fb {
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half4 uv1 : TEXCOORD1;
		UNITY_VERTEX_OUTPUT_STEREO
	};

	half3 Unpack(half4 c)
	{
#if UNITY_COLORSPACE_GAMMA
		c.rgb *= c.rgb;
#endif
#if _USE_RGBM
		return c.rgb * c.a * 8.0;
#else
		return c.rgb;
#endif
	}

	half4 Pack(half3 c)
	{
#if _USE_RGBM
		c /= 8.0;
		half m = max(max(c.x, c.y), max(c.z, 1e-5));
		m = ceil(m * 255) / 255;
		half4 o = half4(c / m, m);
#else
		half4 o = half4(c, 1.0);
#endif
#if UNITY_COLORSPACE_GAMMA
		return half4(sqrt(o.rgb), o.a);
#else
		return o;
#endif
	}

	v2f vert(appdata i)
	{
		v2f o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.uv = UnityStereoTransformScreenSpaceTex(i.uv);
		o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, half4(i.pos.xyz, 1.0)));
		return o;
	}

	v2fb vertBlur(appdata i)
	{
		v2fb o;
		UNITY_SETUP_INSTANCE_ID(i);
		UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
		o.uv = UnityStereoTransformScreenSpaceTex(i.uv);
		o.pos = mul(unity_MatrixVP, mul(unity_ObjectToWorld, half4(i.pos.xyz, 1.0)));
		o.uv1 = half4(o.uv - _Offset, o.uv + _Offset);
		return o;
	}

	half4 fragBloom(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half3 c = SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv).xyz;
#if UNITY_COLORSPACE_GAMMA
		c.rgb *= c.rgb;
#endif
		half br = max(c.r, max(c.g, c.b));
		half soft = clamp(br - _BloomData.y, 0.0, _BloomData.z);
		half a = max(soft * soft * _BloomData.w, br - _BloomData.x) / max(br, 1e-4);
		return Pack(c * a);
	}

	half4 fragBlur(v2fb i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half3 c = Unpack(SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv));
		c += Unpack(SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv1.xy));
		c += Unpack(SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv1.xw));
		c += Unpack(SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv1.zy));
		c += Unpack(SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv1.zw));
		return Pack(c * 0.2);
	}
	
	half4 fragUp(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		return Pack(lerp(Unpack(SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv)), Unpack(SAMPLE_TEXTURE2D_X(_BloomTemp, sampler_BloomTex, i.uv)), _BlurAmount));
	}

	half4 frag(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		half4 c = SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv);

#if UNITY_COLORSPACE_GAMMA
		c.rgb *= c.rgb;
#endif
		half3 bloom = Unpack(SAMPLE_TEXTURE2D_X(_BloomTemp, sampler_BloomTex, i.uv));
		c.rgb += bloom * _BloomColor.rgb;
#if UNITY_COLORSPACE_GAMMA
		return half4(sqrt(c.rgb), c.a);
#else
		return c;
#endif
	}

	half4 fragBlit(v2f i) : SV_Target
	{
		UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
		return SAMPLE_TEXTURE2D_X(_BloomTex, sampler_BloomTex, i.uv);
	}

	ENDHLSL

	Subshader
	{
		Tags{ "RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" }
		LOD 100
		ZTest Always ZWrite Off Cull Off
		Pass //0
		{
		  HLSLPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBloom
		  ENDHLSL
		}

		Pass //1
		{
		  HLSLPROGRAM
		  #pragma vertex vertBlur
		  #pragma fragment fragBlur
		  ENDHLSL
		}

		Pass //2
		{
		  HLSLPROGRAM
		  #pragma vertex vert
		  #pragma fragment fragUp
		  ENDHLSL
		}

		Pass //3
		{
		  HLSLPROGRAM
		  #pragma vertex vert
		  #pragma fragment frag
		  ENDHLSL
		}

		Pass //4
		{
		  HLSLPROGRAM
		  #pragma vertex vert
		  #pragma fragment fragBlit
		  ENDHLSL
		}
	}
	Fallback off
}