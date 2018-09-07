// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Sprite Outline" {
	Properties{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_Outline ("Outline width", Range (1.0, 10.0)) = 5.0
	}
		SubShader{
		Tags{ "Queue" = "Transparent" "RenderType" = "Transparent" }
		Cull Off
		Blend One OneMinusSrcAlpha

		Pass{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#include "UnityCG.cginc"

		sampler2D _MainTex;

	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};

	v2f vert(appdata_base v) {
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = v.texcoord;
		return o;
	}

	float _Outline;
	fixed4 _Color;
	float4 _MainTex_TexelSize;

	fixed4 frag(v2f i) : COLOR
	{
		half4 c = tex2D(_MainTex, i.uv);
		c.rgb *= c.a;
		half4 outlineC = _Color;
		//outlineC.a *= ceil(c.a);		// Ensures the highlight maintains the base pixel transparency e.g. if a pix is transparent, you cant see it highlighted either
		outlineC.rgb *= outlineC.a;

		fixed alpha_up = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y*_Outline)).a;
		fixed alpha_down = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y*_Outline)).a;
		fixed alpha_right = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x*_Outline, 0)).a;
		fixed alpha_left = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x*_Outline, 0)).a;

		// the highlight is made of the pixels after outside of the sprite
		return lerp(c, outlineC, c.a == 0 && alpha_up + alpha_down + alpha_right + alpha_left > 0);
		// the highlight is made from the last pixels of the sprite
		//return lerp(outlineC, c, ceil(alpha_up * alpha_down * alpha_right * alpha_left));
	}

		ENDCG
	}
	}
		FallBack "Diffuse"
}
