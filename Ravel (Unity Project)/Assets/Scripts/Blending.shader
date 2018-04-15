Shader "Alpha Mask/Blending" {
 Properties {
 _BaseTex ("Base (RGB)", 2D) = "white" {}
 _ApplyTex("Application (RBG)", 2D) = "white" {}
 _MaskTex ("Mask texture", 2D) = "white" {}
 }
 SubShader {
 Pass {
 CGPROGRAM
 #pragma vertex vert_img
 #pragma fragment frag
 #include "UnityCG.cginc"
 
 uniform sampler2D _BaseTex;
 uniform sampler2D _MaskTex;
 uniform sampler2D _ApplyTex;
 
 
 float4 frag (v2f_img i) : COLOR {
		 float4 mask = tex2D(_MaskTex, i.uv);
		 float4 base = tex2D(_BaseTex, i.uv);
		 float4 apply = tex2D(_ApplyTex, i.uv);

		 float a = mask.r*.3 + mask.g*.59 + mask.b*.11;

		 float r = base.r * a + (apply.r * (1-a));
		 float g = base.g * a + (apply.g * (1-a));
		 float b = base.b * a + (apply.b * (1-a));


 		 float3 target = float3(r, g, b); 
 

		 float4 result = base;
		 result.rgb = target.rgb;
		 return result;
	 }
	 ENDCG
	 }
 }
}