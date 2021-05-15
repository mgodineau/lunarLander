Shader "Custom/addWireframe"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LineColor("Line color", Color) = (1,1,1,1)
        _Width ("Width", Int) = 10
        _BrightnessThreshold("Brightness threshold", Float) = 0.2
        
        _AfterImageFactor("After image fade factor", Float) = 0.9
        _AfterImageThreshold("After image thresholf", Float) = 0.01
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            ZTest Always Cull Off ZWrite Off   
            
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
            
            struct f2a {
                fixed4 color : COLOR0;
                fixed4 afterImageColor : COLOR1;
            };
            
            
            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            
            sampler2D _WireframeTex;
            float4 _WireframeTex_TexelSize;
            float4 _LineColor;
            
            sampler2D _AfterImageTex;
            
            int _Width;
            float _BrightnessThreshold;
            
            
            float _AfterImageFactor;
            float _AfterImageThreshold;
            
            float _DetlaTime;
            
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
            
            float getMaxBrightness( float2 uv ) {
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
                return tex2D(_WireframeTex, uv ).a > 0.1 && getMaxBrightness(uv) < _BrightnessThreshold;
            }
            
            f2a frag (v2f i)
            {
                
                
                float maxWeight = 0.0;
                float maxBrightness = 0.0;
                float4 wireColor;
                
                for( int x=-_Width; x<=_Width; x++ ) {
                    for( int y=-_Width; y<=_Width; y++ ) {
                        float weight = 1.0 - sqrt( x*x + y*y ) / _Width;
                        
                        float2 offset = float2( _WireframeTex_TexelSize.x * x, _WireframeTex_TexelSize.y * y );
                        
                        float currentBrightness = getMaxBrightness(i.uv + offset);
                        if( isOnWire(i.uv + offset) && weight > maxWeight) {
                            maxBrightness = max(currentBrightness, maxBrightness);
                            maxWeight = weight;
                            wireColor = tex2D( _WireframeTex, i.uv + offset );
                        }
                        
                    }//for y
                }//for x
                
                fixed4 originalColor = tex2D( _MainTex, i.uv );
                
                float brightnessFactor = (1 - maxBrightness/_BrightnessThreshold);
                
                float4 wireFinalColor = maxWeight * wireColor;
                wireFinalColor = max(wireFinalColor, tex2D(_AfterImageTex, i.uv));
                
                float4 afterImageColor = wireFinalColor - _AfterImageFactor * _DetlaTime;
                afterImageColor = brightness( afterImageColor.rgb ) > _AfterImageThreshold ? afterImageColor : 0;
                
                f2a o;
                o.color = originalColor + wireFinalColor * brightnessFactor;
                o.afterImageColor = afterImageColor;
                
                return o;
            }
            ENDCG
        }//Pass
        
        
    }//SubShader
}//Shader
