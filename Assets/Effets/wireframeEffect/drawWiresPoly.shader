Shader "Unlit/drawWiresPoly"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_Zoffset("Z Offset", Float) = 0.1
		
		_Width("Width", Int) = 10
		
		_Zfocus("Z focus", Float) = 1
		_ZfocusDepth("Z focus depth", Float) = 1
		
		_StencilRef("Stencil UI Ref", Float) = 1
		_StencilComp("Stencil comp", Float) = 6
	}
	SubShader
	{
		//Tags { "RenderType"="Opaque" }
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		// Blend SrcAlpha OneMinusSrcAlpha
		Blend SrcColor DstColor
		// Blend SrcAlpha Zero
		BlendOp Max
		LOD 100

		Pass
		{
			Cull Off
			ZWrite Off
			
			Stencil {
				Ref [_StencilRef]
				ReadMask [_StencilRef]
				
				Comp [_StencilComp]
			}
			
			CGPROGRAM
			
			
			#pragma vertex vert alpha
			#pragma fragment frag alpha
			#pragma geometry geom

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};

			struct v2g
			{
				float4 position : SV_POSITION;
				fixed4 color : COLOR;
			};
			
			
			struct g2f
			{
				float4 position : SV_POSITION;
				fixed4 color : COLOR;
				fixed2 uv : TEXCOORD0;
			};
			

			float4 _Color;
			float _Zoffset;
			
			float _Width;
			
			float _Zfocus;
			float _ZfocusDepth;
			
			
			float getDepthBrightness( float z ) {
				return 1.0 - clamp(abs(z - _Zfocus) / _ZfocusDepth, 0.0, 1.0);
			}
			
			
			v2g vert (appdata v)
			{
				v2g o;
				o.position = UnityObjectToClipPos(v.vertex);
				o.position.z -= _Zoffset;
				o.color = v.color * _Color;
				return o;
			}
			
			[maxvertexcount(6)]
			void geom( line v2g input[2], inout TriangleStream<g2f> OutputStream  ) {
				
				//float4 offset = float4(0.01, 0, 0, 0);
				float aspect = _ScreenParams.x / _ScreenParams.y;
				float relativeWidthHalf = 0.5 * _Width / _ScreenParams.y;
				
				float4 edgeDir = input[0].position - input[1].position;
				edgeDir.x *= aspect;
				edgeDir = normalize(edgeDir);
				float4 offsetDir = float4( -edgeDir.y, edgeDir.x, edgeDir.zw );
				float4 offset = offsetDir * relativeWidthHalf;
				offset.x /= aspect;
				
				float4 vertex_0 = input[0].position + offset;
				float4 vertex_1 = input[0].position - offset;
				float4 vertex_2 = input[1].position + offset;
				float4 vertex_3 = input[1].position - offset;
				
				fixed2 uv_0 = fixed2(0, 0);
				fixed2 uv_1 = fixed2(0, 1);
				fixed2 uv_2 = fixed2(1, 0);
				fixed2 uv_3 = fixed2(1, 1);
				
				g2f output;
				output.color = input[0].color;
				
				
				{
					output.position = vertex_0;
					output.uv = uv_0;
					OutputStream.Append(output);
					
					output.position = vertex_1;
					output.uv = uv_1;
					OutputStream.Append(output);
					
					output.position = vertex_2;
					output.color = input[1].color;
					output.uv = uv_2;
					OutputStream.Append(output);
				}
				OutputStream.RestartStrip();
				
				{
					output.position = vertex_2;
					output.uv = uv_2;
					OutputStream.Append(output);
					
					output.position = vertex_3;
					output.uv = uv_3;
					OutputStream.Append(output);
					
					output.position = vertex_1;
					output.color = input[0].color;
					output.uv = uv_1;
					OutputStream.Append(output);
				}
				OutputStream.RestartStrip();
			}
			
			
			
			
			fixed4 frag (g2f i) : SV_Target
			{
				float distToCenter = abs( float(i.uv.y) - 0.5 ) * 2;
				
				// fixed4 wireColor = i.color * pow(1-distToCenter, 2);
				fixed4 wireColor = i.color * (1.0 - distToCenter) * (1.0 - distToCenter);
				// wireColor.a = pow(1.0 - distToCenter, 2);
				// wireColor.a = 0.5;
				
				// return getDepthBrightness(i.position.z) * wireColor;
				return wireColor;
			}
			ENDCG
		}
	}
}
