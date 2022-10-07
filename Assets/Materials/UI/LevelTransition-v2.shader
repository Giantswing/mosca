// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LevelTransition-v2"
{
	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Tint", Color) = (1,1,1,1)
		
		_StencilComp ("Stencil Comparison", Float) = 8
		_Stencil ("Stencil ID", Float) = 0
		_StencilOp ("Stencil Operation", Float) = 0
		_StencilWriteMask ("Stencil Write Mask", Float) = 255
		_StencilReadMask ("Stencil Read Mask", Float) = 255

		_ColorMask ("Color Mask", Float) = 15

		[Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
		_TextureSample0("Texture Sample 0", 2D) = "white" {}
		_BGColor("BGColor", Color) = (0.2197157,0.01650944,0.5,1)
		_CompareValue("CompareValue", Range( 0 , 0.55)) = 0.2466688
		_Offset("Offset", Range( 0 , 2)) = 1.041719
		_TextureSample1("Texture Sample 1", 2D) = "white" {}

	}

	SubShader
	{
		LOD 0

		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" "CanUseSpriteAtlas"="True" }
		
		Stencil
		{
			Ref [_Stencil]
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
			CompFront [_StencilComp]
			PassFront [_StencilOp]
			FailFront Keep
			ZFailFront Keep
			CompBack Always
			PassBack Keep
			FailBack Keep
			ZFailBack Keep
		}


		Cull Off
		Lighting Off
		ZWrite Off
		ZTest [unity_GUIZTestMode]
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask [_ColorMask]

		
		Pass
		{
			Name "Default"
		CGPROGRAM
			
			#ifndef UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX
			#define UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input)
			#endif
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0

			#include "UnityCG.cginc"
			#include "UnityUI.cginc"

			#pragma multi_compile __ UNITY_UI_CLIP_RECT
			#pragma multi_compile __ UNITY_UI_ALPHACLIP
			
			#include "UnityShaderVariables.cginc"

			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
				float4 worldPosition : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				
			};
			
			uniform fixed4 _Color;
			uniform fixed4 _TextureSampleAdd;
			uniform float4 _ClipRect;
			uniform sampler2D _MainTex;
			uniform float4 _BGColor;
			uniform sampler2D _TextureSample0;
			uniform float _CompareValue;
			uniform float _Offset;
			uniform sampler2D _TextureSample1;

			
			v2f vert( appdata_t IN  )
			{
				v2f OUT;
				UNITY_SETUP_INSTANCE_ID( IN );
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);
				UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
				OUT.worldPosition = IN.vertex;
				
				
				OUT.worldPosition.xyz +=  float3( 0, 0, 0 ) ;
				OUT.vertex = UnityObjectToClipPos(OUT.worldPosition);

				OUT.texcoord = IN.texcoord;
				
				OUT.color = IN.color * _Color;
				return OUT;
			}

			fixed4 frag(v2f IN  ) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX( IN );

				float4 color34 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float2 texCoord10 = IN.texcoord.xy * float2( 0.7,0.7 ) + float2( 0.14,0.14 );
				float cos12 = cos( _Time.y );
				float sin12 = sin( _Time.y );
				float2 rotator12 = mul( texCoord10 - float2( 0.5,0.5 ) , float2x2( cos12 , -sin12 , sin12 , cos12 )) + float2( 0.5,0.5 );
				float4 tex2DNode3 = tex2D( _TextureSample0, rotator12 );
				float4 color16 = IsGammaSpace() ? float4(1,1,1,1) : float4(1,1,1,1);
				float4 lerpResult32 = lerp( color34 , _BGColor , ( tex2DNode3.r < ( _CompareValue * _Offset ) ? color16.r : 0.0 ));
				float4 color6 = IsGammaSpace() ? float4(0,0,0,0) : float4(0,0,0,0);
				float4 color1 = IsGammaSpace() ? float4(1,1,1,0) : float4(1,1,1,0);
				float4 lerpResult2 = lerp( color6 , color1 , ( tex2DNode3.r < _CompareValue ? color16.r : 0.0 ));
				float4 blendOpSrc36 = lerpResult32;
				float4 blendOpDest36 = lerpResult2;
				float cos40 = cos( _Time.z );
				float sin40 = sin( _Time.z );
				float2 rotator40 = mul( texCoord10 - float2( 0.5,0.5 ) , float2x2( cos40 , -sin40 , sin40 , cos40 )) + float2( 0.5,0.5 );
				float4 blendOpSrc39 = ( saturate(  (( blendOpSrc36 > 0.5 ) ? ( 1.0 - ( 1.0 - 2.0 * ( blendOpSrc36 - 0.5 ) ) * ( 1.0 - blendOpDest36 ) ) : ( 2.0 * blendOpSrc36 * blendOpDest36 ) ) ));
				float4 blendOpDest39 = tex2D( _TextureSample1, rotator40 );
				
				half4 color = ( saturate( ( blendOpSrc39 * blendOpDest39 ) ));
				
				#ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(IN.worldPosition.xy, _ClipRect);
                #endif
				
				#ifdef UNITY_UI_ALPHACLIP
				clip (color.a - 0.001);
				#endif

				return color;
			}
		ENDCG
		}
	}
	CustomEditor "ASEMaterialInspector"
	
	
}
/*ASEBEGIN
Version=18935
2560;150;1920;1058;2651.606;-56.57316;1.012869;True;True
Node;AmplifyShaderEditor.TimeNode;14;-2143.13,533.4797;Inherit;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;10;-2122.969,157.42;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;0.7,0.7;False;1;FLOAT2;0.14,0.14;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RotatorNode;12;-1763.969,345.42;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-1407.244,1040.891;Inherit;False;Property;_Offset;Offset;3;0;Create;True;0;0;0;False;0;False;1.041719;1.469885;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;18;-1420.035,602.2468;Inherit;False;Property;_CompareValue;CompareValue;2;0;Create;True;0;0;0;False;0;False;0.2466688;1;0;0.55;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;16;-1369.035,691.2468;Inherit;False;Constant;_Color2;Color 2;1;0;Create;True;0;0;0;False;0;False;1,1,1,1;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-1382.732,391.9112;Inherit;True;Property;_TextureSample0;Texture Sample 0;0;0;Create;True;0;0;0;False;0;False;-1;558754d1a8760f44883f2deae0ab88c0;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;24;-1001.644,856.2915;Inherit;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;34;-793.6688,1051.938;Inherit;False;Constant;_Color3;Color 3;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;33;-790.7748,1245.728;Inherit;False;Property;_BGColor;BGColor;1;0;Create;True;0;0;0;False;0;False;0.2197157,0.01650944,0.5,1;0.2197157,0.01650944,0.5,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;15;-727.8785,460.23;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;1;-871.175,220.2092;Inherit;False;Constant;_Color0;Color 0;0;0;Create;True;0;0;0;False;0;False;1,1,1,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;6;-864.9691,-25.57996;Inherit;False;Constant;_Color1;Color 1;1;0;Create;True;0;0;0;False;0;False;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Compare;23;-742.9437,758.7914;Inherit;True;4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;2;-336.4059,220.007;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;32;-276.4758,723.6105;Inherit;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RotatorNode;40;-1774.917,755.5007;Inherit;True;3;0;FLOAT2;0,0;False;1;FLOAT2;0.5,0.5;False;2;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.BlendOpsNode;36;26.06467,375.0915;Inherit;True;HardLight;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;38;49.56847,1103.071;Inherit;True;Property;_TextureSample1;Texture Sample 1;4;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.BlendOpsNode;39;402.4485,552.085;Inherit;True;Multiply;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;1;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;708.5999,329.6;Float;False;True;-1;2;ASEMaterialInspector;0;6;LevelTransition-v2;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;12;0;10;0
WireConnection;12;2;14;2
WireConnection;3;1;12;0
WireConnection;24;0;18;0
WireConnection;24;1;22;0
WireConnection;15;0;3;1
WireConnection;15;1;18;0
WireConnection;15;2;16;1
WireConnection;23;0;3;1
WireConnection;23;1;24;0
WireConnection;23;2;16;1
WireConnection;2;0;6;0
WireConnection;2;1;1;0
WireConnection;2;2;15;0
WireConnection;32;0;34;0
WireConnection;32;1;33;0
WireConnection;32;2;23;0
WireConnection;40;0;10;0
WireConnection;40;2;14;3
WireConnection;36;0;32;0
WireConnection;36;1;2;0
WireConnection;38;1;40;0
WireConnection;39;0;36;0
WireConnection;39;1;38;0
WireConnection;0;0;39;0
ASEEND*/
//CHKSM=CCBECF14274334AA709334303EE0070DF210F8B5