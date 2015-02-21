Shader "Custom/ShowBG" {
	Properties {
		_BC ("base color", Color) = (0.5,0.5,0.5,0)
		_P ("painting intencity",Float) = 0.2
		_Mask ("logo",2D) = "black"{}
	}
	CGINCLUDE
		#include "UnityCG.cginc"
 		
 		uniform sampler2D _PreFBO, _BlurTex,_Canvas;
		uniform float _MouseTime;
		sampler2D _Mask;
		half4 _MainTex_TexelSize, _BC;
		half _P;
			
		half4 frag(v2f_img i) : COLOR{
#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0) 
				i.uv.y = 1-i.uv.y;
#endif
			half4
				p = tex2D(_PreFBO, i.uv),
				c = tex2D(_Canvas, i.uv),
				m = tex2D(_Mask, i.uv);
			
			c.rgb = lerp(c.rgb,p.rgb,p.a*_P*unity_DeltaTime.z*saturate(_MouseTime*0.1)+m.r*saturate(_MouseTime*0.1-2.0));
			c.a = 0;
			return lerp(c,_BC,saturate(1-_MouseTime));
		}
	ENDCG
	
	SubShader {
		ZTest Always Cull Off ZWrite Off
		Fog { Mode off }
 
		pass{
			CGPROGRAM
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma vertex vert_img
			#pragma fragment frag
			ENDCG
		}
	} 
}