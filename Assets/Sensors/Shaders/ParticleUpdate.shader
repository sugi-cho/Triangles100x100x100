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
			_TarTex,
			_VelTex,
			_RotTex,
			_PosTex;
			
		struct appdata
		{
			float4 vertex : POSITION;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			float2 uv : TEXCOORD0;
		};

		struct pOut{
			float4 target : COLOR0;
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
		
		float3 cube(float2 uv){
			float
				px = frac(uv.x*10)*100-50,
				py = floor(uv.x*10)+floor(uv.y*10)*10-50,
				pz = frac(uv.y*10)*100-50;
			return float3(px,py,pz);
		}

		pOut frag (v2f i)
		{
			float4
				target = tex2D(_TarTex, i.uv),
				velocity = tex2D(_VelTex, i.uv),
				rotation = tex2D(_RotTex, i.uv),
				position = tex2D(_PosTex, i.uv);
			
			
			position.xyz = cube(i.uv);
			rotation = float4(0,0,0,1);
			
			rotation = normalize(rotation);
			
			pOut o;
			o.target = target;
			o.velocity = velocity;
			o.rotation = rotation;
			o.position = position;
			return o;
		}
		ENDCG

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma glsl
			ENDCG
		}
	}
}