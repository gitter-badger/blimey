attribute vec4 a_vertPos;
attribute vec3 a_vertNormal;
attribute vec4 a_vertColour;

// UNIFORMS

uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;

uniform vec3 u_eyePosition;

// FOG
uniform float u_fogEnabled;
uniform float u_fogStart;
uniform float u_fogEnd;

varying vec4 v_positionWS;
varying vec3 v_normalWS;
varying vec4 v_tint;

void ComputeFogFactor(in float d, out float fogFactor)
{
	float val = (d - u_fogStart) / (u_fogEnd - u_fogStart);
	fogFactor = clamp(val, 0.0, 1.0) * u_fogEnabled;
}

void main()
{
	vec4 pos_ws = u_world * a_vertPos;
	vec4 pos_vs = u_view * pos_ws;
	vec4 pos_ps = u_proj * pos_vs;

	gl_Position = pos_ps;
	
	float fog = 0.0;
	
	vec3 popo = u_eyePosition - pos_ws.xyz;
	
	float l = length(popo);
	
	ComputeFogFactor(l, fog);
	
	v_positionWS = vec4(pos_ws.xyz, fog);

	vec4 temp = vec4(a_vertNormal.xyz, 0.0);
	
	temp = u_world * temp;
	
	v_normalWS = normalize(temp.xyz);
	
	v_tint = a_vertColour * u_colour;
}
