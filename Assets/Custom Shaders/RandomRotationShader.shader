Shader "Custom Shaders/NewSurfaceShader" {
     Properties {
        _Tex1 ("Texture", 2D) = "white" {}
        [Toggle] _UseRandMask ("Use Random Mask", Int) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        LOD 200
   
        CGPROGRAM
        #pragma surface surf Lambert
        #pragma shader_feature _USERANDMASK_ON
 
        sampler2D _Tex1;
        int _UseRandMask;
                     
        struct Input {
            half2 uv_Tex1;
        };
 
        // generic pseudo-random function
        half rand2 ( half2 coords ) {
            return frac(sin(dot(coords, half2(12.9898,78.233))) * 43758.5453);
        }
 
        void surf (Input IN, inout SurfaceOutput o) {
            // declare the rotation matrixes in array fo 0, 90, 180 and 270 degrees
            const half2x2 rotMx[4] = { half2x2(1, 0, 0, 1), half2x2(0, 1, -1, 0),
                                       half2x2(-1, 0, 0, -1), half2x2(0, -1, 1, 0) };      
 
            #ifdef _USERANDMASK_ON
                int x = rand2(floor(IN.uv_Tex1)) * 4; // random between 0-3
                half2 uvTex1 = mul(rotMx[x], IN.uv_Tex1); // apply random rotation based on rotation matrix
            #else
                half2 uvTex1 = IN.uv_Tex1; // use unmodifed UVs if _UseRandMask is Off
            #endif
 
            // use input texture with calculated UVs
            // and using texture derivates ddx and ddy to resolve mip mapping issues
            half4 tex1 = tex2D(_Tex1, uvTex1, ddx(IN.uv_Tex1.xy), ddy(IN.uv_Tex1.xy));
 
            // send texture data to shader output
            o.Albedo = tex1.rgb;
            o.Alpha = tex1.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
    FallBack "Diffuse"
}
