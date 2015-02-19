Shader "Custom/MotionBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" {}
	}
	CGINCLUDE
		#include "UnityCG.cginc"
 		
 		uniform sampler2D _PreFBO;
		sampler2D _MainTex;
		half4 _MainTex_TexelSize;
			
		half4 frag(v2f_img i) : COLOR{
#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0) 
				i.uv.y = 1-i.uv.y;
#endif
			half4 p = tex2D(_PreFBO, i.uv);
			half4 c = tex2D(_MainTex, i.uv);
			
			return c*0.4+p*0.6;
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