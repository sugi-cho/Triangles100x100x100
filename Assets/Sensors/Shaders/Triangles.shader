Shader "Custom/Triangles" {
	CGINCLUDE
		#define PI 3.141592
		#include "UnityCG.cginc"
		#include "Assets/CGINC/Quaternion.cginc"
		
		uniform sampler2D
			_ColTex,
			_VelTex,
			_PosTex,
			_RotTex;
		float _Offset;
		
		struct v2f {
			float4 pos : SV_POSITION;
			fixed4 color : COLOR;
			float3 normal : TEXCOORD0;
			float3 wPos : TEXCOORD1;
			float speed : TEXCOORD2;
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
				color = tex2Dlod(_ColTex,float4(uv,0,0)),
				rotation = tex2Dlod(_RotTex,float4(uv,0,0)),
				position = tex2Dlod(_PosTex,float4(uv,0,0));
			
			v.vertex.xyz = rotateWithQuaternion(v.vertex.xyz, rotation) + position.xyz;
			v.color = color;
			v.normal = rotateWithQuaternion(v.normal, rotation);
			
			v2f o;
			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			o.normal = normalize(mul((float3x3)_Object2World, normalize(v.normal)));
			o.wPos = mul(_Object2World, v.vertex);
			o.speed = length(tex2Dlod(_VelTex, float4(uv,0,0)));
			return o;
		}
		
		half4 frag (v2f i) : COLOR
		{
			float3 
				lDir = normalize(rotateAngleAxis(float3(1,1,-1),float3(0,1,0),_Time.y)),
				vDir = normalize(_WorldSpaceCameraPos-i.wPos),
				hDir = normalize(lDir + vDir);
			
			float
				diff = max(0,dot(i.normal, lDir)+0.75),
				nh = max(0, dot(i.normal, hDir)),
				spec = pow(nh, 64.0)*0.5;
			half4 c = lerp(i.color * diff + spec,i.color*(1+spec),i.color.a);
			c.a = i.speed;
			return c;
		}
	ENDCG
	
	SubShader {
		Pass {
			Cull Off
			CGPROGRAM
			#pragma glsl
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			ENDCG 
		}
	}
}