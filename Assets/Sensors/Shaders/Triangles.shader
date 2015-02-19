Shader "Custom/Triangles" {
	CGINCLUDE
		#define PI 3.141592
		#include "UnityCG.cginc"
		#include "Assets/CGINC/Quaternion.cginc"
		
		uniform sampler2D
			_TarTex,
			_VelTex,
			_PosTex,
			_RotTex;
		float _Offset;
		
		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float3 normal : TEXCOORD0;
		};
		struct pOut{
			float4 color : COLOR0; //color.rgb,emission
			float4 normal : COLOR1; //normal.xyz,wire
			float4 position : COLOR2; //pos.xyz,depth
		};
		
		
		v2f vert (appdata_full v)
		{
			float 
				id = v.texcoord1.x + int(_Offset),
				x = frac(id/1000.0),
				y = floor(id/1000.0)/1000.0;
			float2
				uv = float2(x,y);
			float4
				rotation = tex2Dlod(_RotTex,float4(uv,0,0)),
				position = tex2Dlod(_PosTex,float4(uv,0,0));
			
			v.vertex.xyz = rotateWithQuaternion(v.vertex.xyz, rotation) + position.xyz;
			v.color.rgb = position.xyz * 0.01 +0.5;
			
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			o.normal = mul((float3x3)_Object2World, v.normal);
			return o;
		}
			
		half4 frag (v2f i) : COLOR
		{
			return i.color;
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