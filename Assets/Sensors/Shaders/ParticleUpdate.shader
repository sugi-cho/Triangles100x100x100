Shader "Custom/ParticleUpdate" {
	SubShader {
		Tags { "RenderType"="Opaque" }
		ZTest Always
		ZWrite On
		Cull Back

		CGINCLUDE
		#include "/Assets/CGINC/TexNoise.cginc"
		#include "/Assets/CGINC/Quaternion.cginc"

		uniform sampler2D
			_ColTex,
			_VelTex,
			_RotTex,
			_PosTex;
		uniform float4x4 _MVP;
		uniform float4 _MousePos;
		uniform float _MouseTime;
			
		struct appdata
		{
			float4 vertex : POSITION;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		struct pOut{
			float4 color : COLOR0;
			float4 velocity : COLOR1;
			float4 rotation : COLOR2;
			float4 position : COLOR3;
		};


		v2f vert (appdata v)
		{
			v2f o;
			o.vertex = v.vertex;
			o.uv = (v.vertex.xy/v.vertex.w+1.0)*0.5;
			return o;
		}
		
		float3 cubePos(float2 uv){
			float
				px = frac(uv.x*10)*100-50,
				py = floor(uv.x*10)+floor(uv.y*10)*10-50+0.5,
				pz = frac(uv.y*10)*100-50;
			return float3(px,py,pz);
		}
		float3 spherePos(float2 uv){
			float3 pos = cubePos(uv);
			pos = normalize(pos) * max(abs(pos.x),max(abs(pos.y), abs(pos.z)));
			return pos;
		}
		
		pOut frag_initialize(v2f i){
			float4
				color = float4(0.5,0.5,0.5,0),
				velocity = 0,
				rotation = float4(0,0,0,1),
				position = float4(spherePos(i.uv),0);
			
			rotation = fromToRotation(float3(0,1,0), position.xyz);
			
			pOut o;
			o.color = color;
			o.velocity = velocity;
			o.rotation = rotation;
			o.position = position;
			return o;
		}

		pOut frag_update (v2f i)
		{
			float4
				color = tex2D(_ColTex, i.uv),
				velocity = tex2D(_VelTex, i.uv),
				rotation = tex2D(_RotTex, i.uv),
				position = tex2D(_PosTex, i.uv);
			
			float3 to = (_MousePos.xyz - position.xyz);
			velocity.xyz = velocity.xyz * 0.5 + to * unity_DeltaTime.z * saturate(_MouseTime*0.1);
			
			float3 velocity2 = velocity.xyz + min(20,10+_MouseTime) * (tnoise3D(position.xyz*0.01, 0, _Time.x)-0.49);
			rotation = lerp(rotation,fromToRotation(float3(0,0,1), velocity2), unity_DeltaTime.z);
			velocity2 = rotateWithQuaternion(float3(0,0,1), rotation)*length(velocity2)*unity_DeltaTime.z*30.0;
			position.xyz += velocity2;
			
			color = lerp(color, float4(0.5+velocity2*0.5,1), saturate(_MouseTime*0.02));
			
			pOut o;
			o.color = color;
			o.velocity = velocity;
			o.rotation = rotation;
			o.position = position;
			return o;
		}
		
		pOut frag_show(v2f i){
			float4
				color = tex2D(_ColTex, i.uv),
				velocity = 0,
				rotation = tex2D(_RotTex, i.uv),
				position = tex2D(_PosTex, i.uv);
			
			
			pOut o;
			o.color = color;
			o.velocity = velocity;
			o.rotation = rotation;
			o.position = position;
			return o;
		}
		ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_initialize
			#pragma target 3.0
			#pragma glsl
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_update
			#pragma target 3.0
			#pragma glsl
			ENDCG
		}
		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_show
			#pragma target 3.0
			#pragma glsl
			ENDCG
		}
	}
}