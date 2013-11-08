attribute mediump vec4 a_vertPos;
attribute mediump vec3 a_vertNormal;
attribute mediump vec4 a_vertColour;

// UNIFORMS

uniform mediump mat4 u_world;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
uniform mediump vec4 u_colour;

uniform mediump vec3 u_eyePosition;

// FOG
uniform mediump float u_fogEnabled;
uniform mediump float u_fogStart;
uniform mediump float u_fogEnd;

varying mediump vec4 v_positionWS;
varying mediump vec3 v_normalWS;
varying mediump vec4 v_tint;

void ComputeFogFactor(in mediump float d, out mediump float fogFactor)
{
	mediump float val = (d - u_fogStart) / (u_fogEnd - u_fogStart);
	fogFactor = clamp(val, 0.0, 1.0) * u_fogEnabled;
}

void main()
{
	mediump vec4 pos_ws = u_world * a_vertPos;
	mediump vec4 pos_vs = u_view * pos_ws;
	mediump vec4 pos_ps = u_proj * pos_vs;

	gl_Position = pos_ps;
	
	mediump float fog = 0.0;
	
	mediump vec3 popo = u_eyePosition - pos_ws.xyz;
	
	mediump float l = length(popo);
	
	ComputeFogFactor(l, fog);
	
	v_positionWS = vec4(pos_ws.xyz, fog);

	mediump vec4 temp = vec4(a_vertNormal.xyz, 0.0);
	
	temp = u_world * temp;
	
	v_normalWS = normalize(temp.xyz);
	
	v_tint = a_vertColour * u_colour;
}
