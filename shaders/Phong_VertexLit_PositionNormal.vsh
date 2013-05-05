attribute vec4 a_Position;
attribute vec3 a_Normal;

uniform mat4 u_World;
uniform mat4 u_View;
uniform mat4 u_Projection;
uniform vec4 u_MaterialColour;

uniform vec3 u_EyePosition;
uniform vec3 u_EmissiveColour;
uniform vec3 u_SpecularColour;

uniform vec3 u_DirLight0Direction;
uniform vec3 u_DirLight0DiffuseColour;
uniform vec3 u_DirLight0SpecularColour;

uniform vec3 u_DirLight1Direction;
uniform vec3 u_DirLight1DiffuseColour;
uniform vec3 u_DirLight1SpecularColour;

uniform vec3 u_DirLight2Direction;
uniform vec3 u_DirLight2DiffuseColour;
uniform vec3 u_DirLight2SpecularColour;

uniform vec3 u_AmbientLightColour;
uniform float u_SpecularPower;

uniform float u_FogEnabled;
uniform float u_FogStart;
uniform float u_FogEnd;
uniform lowp vec3 u_FogColour;


varying lowp vec4 v_diffuse;
varying lowp vec4 v_specular;

vec4 lit(in float NdotL, in float NdotH, in float m)
{

	float ambient = 1.0;
	float diffuse = max(NdotL, 0.0);
	float specular = step(0.0, NdotL) * max(NdotH * m, 0.0);

	return vec4(ambient, diffuse, specular, 1.0);
}


//-----------------------------------------------------------------------------
// Compute lighting
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
void ComputeLights(in vec3 E, in vec3 N, out vec3 diffuse, out vec3 specular)
{

	diffuse = u_AmbientLightColour;
	specular = vec3(0.0, 0.0, 0.0);
	
	// Directional Light 0
	vec3 L = -u_DirLight0Direction;
	vec3 H = normalize(E + L);
	vec2 ret = lit(dot(N, L), dot(N, H), u_SpecularPower).yz;
	diffuse += u_DirLight0DiffuseColour * ret.x;
	specular += u_DirLight0SpecularColour * ret.y;

	// Directional Light 1
	L = -u_DirLight1Direction;
	H = normalize(E + L);
	ret = lit(dot(N, L), dot(N, H), u_SpecularPower).yz;
	diffuse += u_DirLight1DiffuseColour * ret.x;
	specular += u_DirLight1SpecularColour * ret.y;
	
	// Directional Light 2
	L = -u_DirLight2Direction;
	H = normalize(E + L);
	ret = lit(dot(N, L), dot(N, H), u_SpecularPower).yz;
	diffuse += u_DirLight2DiffuseColour * ret.x;
	specular += u_DirLight2SpecularColour * ret.y;

	diffuse *= u_MaterialColour.rgb;
	diffuse	+= u_EmissiveColour;
	specular	*= u_SpecularColour;
		
	
}

//-----------------------------------------------------------------------------
// Compute fog factor
//-----------------------------------------------------------------------------
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
	
	vec4 temp = vec4(a_Normal.xyz, 0.0);
	
	temp = u_World * temp;
	
	vec3 N = normalize(temp.xyz);
	vec3 posToEye = u_EyePosition - pos_ws.xyz;
	vec3 E = normalize(posToEye);
	
	vec3 diffuse = u_AmbientLightColour;
	vec3 specular = vec3(0.0, 0.0, 0.0);
	
	float fogFactor = 0.0;
	
	ComputeLights(E, N, diffuse, specular);
	ComputeFogFactor(length(posToEye), fogFactor);
	
	v_diffuse	= vec4(diffuse.rgb, u_MaterialColour.a);
	
	v_specular = vec4(0.0, 0.0, 0.0, 0.0);
	v_specular.xyz	= specular;
	v_specular.w = fogFactor;


}
