attribute vec4 a_Position;
attribute vec3 a_Normal;

// UNIFORMS

uniform mat4 u_World;
uniform mat4 u_View;
uniform mat4 u_Projection;
uniform lowp vec3 u_EyePosition;

// FOG
uniform lowp float u_FogEnabled;
uniform lowp float u_FogStart;
uniform lowp float u_FogEnd;

varying lowp vec4 v_positionWS;
varying lowp vec3 v_normalWS;

void ComputeFogFactor(in float d, out float fogFactor)
{
	float val = (d - u_FogStart) / (u_FogEnd - u_FogStart);
	fogFactor = clamp(val, 0.0, 1.0) * u_FogEnabled;
}

void main()
{
	vec4 pos_ws = u_World * a_Position;
	vec4 pos_vs = u_View * pos_ws;
	vec4 pos_ps = u_Projection * pos_vs;

	gl_Position = pos_ps;
	
	float fog = 0.0;
	ComputeFogFactor(length(u_EyePosition - pos_ws.xyz), fog);
	
	v_positionWS = vec4(pos_ws.xyz, fog);

	vec4 temp = vec4(a_Normal.xyz, 0.0);
	
	temp = u_World * temp;
	
	v_normalWS = normalize(temp.xyz);
}
