Shader "Custom/addWireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColor("Line color", Color) = (1,1,1,1)
        _Width ("Width", Int) = 10
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 position : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            sampler2D _WireframeTex;
            float4 _WireframeTex_TexelSize;
            float4 _LineColor;
            
            int _Width;
            
            v2f vert (appdata_img v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                
                return o;
            }
            
            bool isOnWire( float2 uv ) {
                return tex2D(_WireframeTex, uv ).a > 0.1;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                
                
                float opacity = 0.0;
                int x = 0;
                for( int x=0; x<_Width; x++ ) {
                    for( int y=0; y<_Width; y++ ) {
                        float weight = 1.0 - sqrt( x*x + y*y ) / _Width;
                        
                        
                        float2 offset = float2( _WireframeTex_TexelSize.x * x, _WireframeTex_TexelSize.y * y );
                        if( isOnWire(i.uv + offset) ) {
                            opacity = max( opacity, weight );
                        }
                        
                        offset.y = -offset.y;
                        if( isOnWire(i.uv + offset) ) {
                            opacity = max( opacity, weight );
                        }
                        
                        offset.x = -offset.x;
                        if( isOnWire(i.uv + offset) ) {
                            opacity = max( opacity, weight );
                        }
                        
                        offset.y = -offset.y;
                        if( isOnWire(i.uv + offset) ) {
                            opacity = max( opacity, weight );
                        }
                        
                    }//for y
                }//for x
                
                fixed4 originalColor = tex2D( _MainTex, i.uv );
                return originalColor + opacity * _LineColor;
            }
            ENDCG
        }
    }
}
