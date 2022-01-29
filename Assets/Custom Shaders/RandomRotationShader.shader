Shader "Custom Shaders/NewSurfaceShader" {
    Properties {
        _Tex1 ("Texture", 2D) = "white" {}
        [Toggle] _UseRandMask ("Use Random Mask", Int) = 0
        [Toggle] _ShowMask ("Show Mask", Int) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
     
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _Tex1;
          float _UseRandMask, _ShowMask;
       
        struct Input {
            float2 uv_Tex1;
            float3 worldPos;
        };
 
        // generic pseudo-random function
        float rand2 ( float2 coords ){
            return frac(sin(dot(coords, float2(12.9898,78.233))) * 43758.5453);
        }
 
        void surf (Input IN, inout SurfaceOutput o) {
            half4 tex1 = tex2D (_Tex1, IN.uv_Tex1);
                     
            // generate random value based on UV mapping
            float x = (round(rand2(floor(IN.uv_Tex1))*3))/1;
            float4 mask;
         
            // transform the random value to RGBA mask
            if ( x == 0 ) {
                mask = float4(1, 0, 0, 0);
            } else if ( x == 1 ) {
                mask = float4(0, 1, 0, 0);
            } else if ( x == 2 ) {
                mask = float4(0, 0, 1, 0);
            } else {
                mask = float4(0, 0, 0, 1);
            }
         
            if ( _ShowMask == 1 ) {
                // show the RGBA mask
                o.Albedo.rgb = mask.rgb;
             
            } else if ( _UseRandMask == 1 ) {
                // create rotated textures using the original texture data rotating its UVs:
                // tex1 - 0 degrees, the original texture
                // tex2 - rotated 90 degrees clockwise
                // tex3 - rotated 180 degrees clockwise
                // tex4 - rotated 270 degrees clockwise
             
                // texture rotated 90 degrees
                float2 uvTexRot90 = IN.uv_Tex1 - 0.5;
                uvTexRot90 = mul(uvTexRot90, float2x2(  0,  1, -1,  0)); // rotate 90 degrees
                uvTexRot90.xy += 0.5;
                half4 tex2 = tex2D(_Tex1, uvTexRot90);
 
                // texture rotated 180 degrees
                float2 uvTexRot180 = IN.uv_Tex1 - 0.5;
                uvTexRot180 = mul(uvTexRot180, float2x2( -1,  0,  0, -1)); // rotate 180 degrees
                uvTexRot180.xy += 0.5;
                half4 tex3 = tex2D(_Tex1, uvTexRot180);
                                         
                // texture rotated 270 degrees
                float2 uvTexRot270 = IN.uv_Tex1 - 0.5;
                uvTexRot270 = mul(uvTexRot270, float2x2(  0, -1,  1,  0)); // rotate 270 degrees
                uvTexRot270.xy += 0.5;
                half4 tex4 = tex2D(_Tex1, uvTexRot270);
 
                // mask the textures using the RGBA mask                                          
                o.Albedo = float3(0,0,0);
                o.Albedo = lerp(o.Albedo, tex1.rgb, mask.r);
                o.Albedo = lerp(o.Albedo, tex2.rgb, mask.g);
                o.Albedo = lerp(o.Albedo, tex3.rgb, mask.b);
                o.Albedo = lerp(o.Albedo, tex4.rgb, mask.a);
             
            } else {
                // _UseRandMask == 0
                // show the original texture without random mask
                o.Albedo = tex1.rgb;
            }
         
            o.Alpha = 1;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
