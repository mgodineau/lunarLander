Shader "Custom/addWireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColor("Line color", Color) = (1,1,1,1)
        _Width ("Width", Int) = 10
        _BrightnessThreshold("Brightness threshold", Float) = 0.2
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZWrite Off
            
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
            float4 _MainTex_TexelSize;
            
            sampler2D _WireframeTex;
            float4 _WireframeTex_TexelSize;
            float4 _LineColor;
            
            int _Width;
            float _BrightnessThreshold;
            
            v2f vert (appdata_img v)
            {
                v2f o;
                o.position = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                
                return o;
            }
            
            
            float brightness( float3 color ) {
                return sqrt( 
                    color.r * color.r * 0.241 +
                    color.g * color.g * 0.691 +
                    color.b * color.b * 0.068
                 );
            }
            
            float maxBrightness( float2 uv ) {
                float maxBr = 0;
                [unroll]
                for( int x2=-1; x2<=1; x2++ ) {
                    [unroll]
                    for( int y2=-1; y2<=1; y2++ ) {
                        float2 offset = float2( _MainTex_TexelSize.x * x2, _MainTex_TexelSize.y * y2 );
                        maxBr = max( maxBr, brightness( tex2D( _MainTex, uv + offset ).rgb ) );
                    }
                }
                return maxBr;
            }
            
            bool isOnWire( float2 uv ) {
                return tex2D(_WireframeTex, uv ).a > 0.1 && maxBrightness(uv) < _BrightnessThreshold;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                
                
                float maxWeight = 0.0;
                float4 wireColor;
                
                for( int x=-_Width; x<=_Width; x++ ) {
                    for( int y=-_Width; y<=_Width; y++ ) {
                        float weight = 1.0 - sqrt( x*x + y*y ) / _Width;
                        
                        float2 offset = float2( _WireframeTex_TexelSize.x * x, _WireframeTex_TexelSize.y * y );
                        
                        if( isOnWire(i.uv + offset) && weight > maxWeight) {
                            maxWeight = weight;
                            wireColor = tex2D( _WireframeTex, i.uv + offset );
                        }
                        
                    }//for y
                }//for x
                
                fixed4 originalColor = tex2D( _MainTex, i.uv );
                return originalColor + maxWeight * wireColor;
            }
            ENDCG
        }//Pass
        
        
    }//SubShader
}//Shader
