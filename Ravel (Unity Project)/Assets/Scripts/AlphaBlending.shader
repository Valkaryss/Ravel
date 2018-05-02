Shader "Alpha Effect/Alpha Blending"
{
	Properties
	{
		_BaseTex ("Base Texture", 2D) = "black" {}
        _ApplyTex("Application (RBG)", 2D) = "black" {}
	}
	SubShader
    {
        Pass {
             CGPROGRAM
             #pragma vertex vert_img
             #pragma fragment frag
             #include "UnityCG.cginc"
 
            uniform sampler2D _BaseTex;
            uniform sampler2D _ApplyTex;

            float4 frag (v2f_img i) : COLOR {
                 float4 base = tex2D(_BaseTex, i.uv);
                 float4 apply = tex2D(_ApplyTex, i.uv);

                 float a = apply.a;
                 float r = base.r * (1 - a) + apply.r * a;
                 float g = base.g * (1 - a) + apply.g * a;
                 float b = base.b * (1 - a) + apply.b * a;

                 return float4(r, g, b, 1);
             }
             ENDCG
        }
	}
}
