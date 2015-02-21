#ifndef TEXNOISE_INCLUDED
#define TEXNOISE_INCLUDED

uniform sampler3D _N3D;

half4 tnoise(half2 v, half t = 0)
{
	return tex3Dlod(_N3D, half4(v,t,0));
}

half4 tnoise(half3 v)
{ 
	return tex3Dlod(_N3D, half4(v,0));
}

half3 tnoise3D(half3 v, int i = 1, half t = 0){
	i = min(3.0,max(0.0,(float)i));
	half3 n = half3(
		tnoise(half3(v.x+t,v.y,v.z))[i],
		tnoise(half3(v.y,v.z+t,v.x))[i],
		tnoise(half3(v.z,v.x,v.y+t))[i]
	);
	return n;
}

half3 curlNoise(half3 v, half d = 0.01, int i = 1, half t = 0){
	half
		cx = (tnoise3D(v+half3(0,d,0),i,t)-tnoise3D(v-half3(0,d,0),i,t)).z
			- (tnoise3D(v+half3(0,0,d),i,t)-tnoise3D(v-half3(0,0,d),i,t)).y,
		cy = (tnoise3D(v+half3(0,0,d),i,t)-tnoise3D(v-half3(0,0,d),i,t)).x
			- (tnoise3D(v+half3(d,0,0),i,t)-tnoise3D(v-half3(d,0,0),i,t)).z,
		cz = (tnoise3D(v+half3(d,0,0),i,t)-tnoise3D(v-half3(d,0,0),i,t)).y
			- (tnoise3D(v+half3(0,d,0),i,t)-tnoise3D(v-half3(0,d,0),i,t)).x;
	return half3(cx,cy,cz) / (2.0*d);
}

#endif // TEXNOISE_INCLUDED