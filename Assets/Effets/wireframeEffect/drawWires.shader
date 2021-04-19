Shader "Unlit/drawWires"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _Zoffset("Z Offset", Float) = 0.1
        
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
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            float4 _Color;
            float _Zoffset;
            
            
            
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.z -= _Zoffset;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
}
