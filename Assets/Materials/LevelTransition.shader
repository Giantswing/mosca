// Upgrade NOTE: upgraded instancing buffer 'LevelTransition' to new syntax.

// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "LevelTransition"
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
		_screenshot("screenshot", 2D) = "white" {}
		_mask("mask", 2D) = "white" {}
		_maskSize("maskSize", Float) = 1
		_YPos("YPos", Range( 0 , 1)) = 0.5
		_XPos("XPos", Range( 0 , 1)) = 0.2088261
		[HideInInspector] _texcoord( "", 2D ) = "white" {}

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
			
			#pragma multi_compile_instancing

			
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
			uniform sampler2D _screenshot;
			uniform sampler2D _mask;
			UNITY_INSTANCING_BUFFER_START(LevelTransition)
				UNITY_DEFINE_INSTANCED_PROP(float4, _screenshot_ST)
#define _screenshot_ST_arr LevelTransition
				UNITY_DEFINE_INSTANCED_PROP(float, _maskSize)
#define _maskSize_arr LevelTransition
				UNITY_DEFINE_INSTANCED_PROP(float, _XPos)
#define _XPos_arr LevelTransition
				UNITY_DEFINE_INSTANCED_PROP(float, _YPos)
#define _YPos_arr LevelTransition
			UNITY_INSTANCING_BUFFER_END(LevelTransition)

			
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

				float4 _screenshot_ST_Instance = UNITY_ACCESS_INSTANCED_PROP(_screenshot_ST_arr, _screenshot_ST);
				float2 uv_screenshot = IN.texcoord.xy * _screenshot_ST_Instance.xy + _screenshot_ST_Instance.zw;
				float2 texCoord14 = IN.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
				float _maskSize_Instance = UNITY_ACCESS_INSTANCED_PROP(_maskSize_arr, _maskSize);
				float temp_output_21_0 = ( _maskSize_Instance * 1.0 );
				float temp_output_17_0 = ( 1.0 - temp_output_21_0 );
				float _XPos_Instance = UNITY_ACCESS_INSTANCED_PROP(_XPos_arr, _XPos);
				float _YPos_Instance = UNITY_ACCESS_INSTANCED_PROP(_YPos_arr, _YPos);
				float2 appendResult26 = (float2(( temp_output_17_0 * _XPos_Instance ) , ( temp_output_17_0 * _YPos_Instance )));
				
				half4 color = ( tex2D( _screenshot, uv_screenshot ) * tex2D( _mask, ( ( texCoord14 * temp_output_21_0 ) + appendResult26 ) ) );
				
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
2560;149;1920;1059;1909.47;352.7115;1.173743;True;True
Node;AmplifyShaderEditor.RangedFloatNode;22;-1347.366,433.3315;Inherit;False;Constant;_Float0;Float 0;4;0;Create;True;0;0;0;False;0;False;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-1364.292,269.386;Inherit;False;InstancedProperty;_maskSize;maskSize;2;0;Create;True;0;0;0;False;0;False;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;21;-1038.366,352.3315;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;17;-859.3215,352.6817;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;19;-998.5459,497.535;Inherit;False;InstancedProperty;_XPos;XPos;4;0;Create;True;0;0;0;False;0;False;0.2088261;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;24;-1017.557,612.4387;Inherit;False;InstancedProperty;_YPos;YPos;3;0;Create;True;0;0;0;False;0;False;0.5;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;14;-1081.419,-12.3032;Inherit;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;23;-643.4576,492.8388;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-644.8067,333.8232;Inherit;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;26;-460.6472,417.2389;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-721.758,70.0839;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;20;-299.6516,298.7182;Inherit;True;2;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;2;-510.837,-159.9593;Inherit;True;Property;_screenshot;screenshot;0;0;Create;True;0;0;0;False;0;False;-1;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;3;-41.94672,257.1375;Inherit;True;Property;_mask;mask;1;0;Create;True;0;0;0;False;0;False;-1;73841d0a7edf78e4097f4ed1ac2240ff;73841d0a7edf78e4097f4ed1ac2240ff;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;8;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;6;FLOAT;0;False;7;SAMPLERSTATE;;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;794.2576,39.56893;Inherit;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.TemplateMultiPassMasterNode;0;1241.719,45.35719;Float;False;True;-1;2;ASEMaterialInspector;0;6;LevelTransition;5056123faa0c79b47ab6ad7e8bf059a4;True;Default;0;0;Default;2;False;True;2;5;False;-1;10;False;-1;0;1;False;-1;0;False;-1;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;-1;False;True;True;True;True;True;0;True;-9;False;False;False;False;False;False;False;True;True;0;True;-5;255;True;-8;255;True;-7;0;True;-4;0;True;-6;1;False;-1;1;False;-1;7;False;-1;1;False;-1;1;False;-1;1;False;-1;False;True;2;False;-1;True;0;True;-11;False;True;5;Queue=Transparent=Queue=0;IgnoreProjector=True;RenderType=Transparent=RenderType;PreviewType=Plane;CanUseSpriteAtlas=True;False;False;0;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;2;False;0;;0;0;Standard;0;0;1;True;False;;False;0
WireConnection;21;0;16;0
WireConnection;21;1;22;0
WireConnection;17;0;21;0
WireConnection;23;0;17;0
WireConnection;23;1;24;0
WireConnection;18;0;17;0
WireConnection;18;1;19;0
WireConnection;26;0;18;0
WireConnection;26;1;23;0
WireConnection;15;0;14;0
WireConnection;15;1;21;0
WireConnection;20;0;15;0
WireConnection;20;1;26;0
WireConnection;3;1;20;0
WireConnection;4;0;2;0
WireConnection;4;1;3;0
WireConnection;0;0;4;0
ASEEND*/
//CHKSM=695DC55436D842A7626A72B8B4B1B85CA7FAFC54