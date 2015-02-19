Shader "Custom/debug/cube" {
	Properties {
		_MainTex ("texture", 2D) = "white" {}
	}
 
	CGINCLUDE
		#define PI 3.1415
		#include "UnityCG.cginc"
		#include "Assets/CGINC/TexNoise.cginc"
		
		sampler2D _MainTex;
		float _Offset;
		
		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float3 normal : TEXCOORD3;
		};
		
		v2f vert (appdata_full v)
		{
			float 
				id = v.texcoord1.x + int(_Offset),
				py = floor(id/(100*100)),
				px = floor(frac(id/(100*100))*100),
				pz = floor(frac(id/(100))*100);
			float3 pos = float3(px,py,pz)-50.0;
			
			v.vertex.xyz += pos + (tnoise3D(pos*0.004,0,_Time.y*0.5)-0.5)*150.0;
			v.color.rgb = fixed3(px,py,pz)/100;
			
			if(py > 100)
				v.vertex = 0;
			
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			o.normal = mul((float3x3)_Object2World, v.normal);
			return o;
		}
			
		float4 frag (v2f i) : COLOR
		{
			float4 c = i.color;
			return c;
		}
	ENDCG
	
	SubShader {
		Cull Off
		Pass {
			CGPROGRAM
			#pragma glsl target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG 
		}
	}
}