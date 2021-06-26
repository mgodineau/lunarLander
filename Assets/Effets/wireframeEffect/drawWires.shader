Shader "Unlit/drawWires"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Zoffset("Z Offset", Float) = 0.1
        
        _Zfocus("Z focus", Float) = 1
        _ZfocusDepth("Z focus depth", Float) = 1
        
        _StencilRef("Stencil UI Ref", Float) = 1
        _StencilComp("Stencil comp", Float) = 6
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
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
            
            
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                fixed4 color : COLOR;
            };

            float4 _Color;
            float _Zoffset;
            
            float _Zfocus;
            float _ZfocusDepth;
            
            
            float getDepthBrightness( float z ) {
                // return 1.0 - clamp(abs(z - _Zfocus) / _ZfocusDepth, 0.0, 1.0);
                return 1.0f;
            }
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.position.z -= _Zoffset;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color * getDepthBrightness(i.position.z);
            }
            ENDCG
        }
    }
}
