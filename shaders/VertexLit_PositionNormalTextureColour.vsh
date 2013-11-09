attribute vec4 a_vertPos;
attribute vec3 a_vertNormal;
attribute vec2 a_vertTexcoord;
attribute vec4 a_vertColour;

uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;

uniform vec3 u_eyePosition;
uniform vec3 u_emissiveColour;
uniform vec3 u_specularColour;

uniform vec3 u_li0Dir;
uniform vec3 u_li0Diffuse;
uniform vec3 u_li0Spec;

uniform vec3 u_li1Dir;
uniform vec3 u_li1Diffuse;
uniform vec3 u_li1Spec;

uniform vec3 u_li2Dir;
uniform vec3 u_li2Diffuse;
uniform vec3 u_li2Spec;

uniform vec3 u_liAmbient;
uniform float u_specularPower;

uniform float u_fogEnabled;
uniform float u_fogStart;
uniform float u_fogEnd;
uniform vec3 u_fogColour;


varying vec4 v_diffuse;
varying vec4 v_specular;
varying vec2 v_texCoord;

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

	diffuse = u_liAmbient;
	specular = vec3(0.0, 0.0, 0.0);
	
	// Directional Light 0
	vec3 L = -u_li0Dir;
	vec3 H = normalize(E + L);
	vec2 ret = lit(dot(N, L), dot(N, H), u_specularPower).yz;
	diffuse += u_li0Diffuse * ret.x;
	specular += u_li0Spec * ret.y;

	// Directional Light 1
	L = -u_li1Dir;
	H = normalize(E + L);
	ret = lit(dot(N, L), dot(N, H), u_specularPower).yz;
	diffuse += u_li1Diffuse * ret.x;
	specular += u_li1Spec * ret.y;
	
	// Directional Light 2
	L = -u_li2Dir;
	H = normalize(E + L);
	ret = lit(dot(N, L), dot(N, H), u_specularPower).yz;
	diffuse += u_li2Diffuse * ret.x;
	specular += u_li2Spec * ret.y;

	diffuse	+= u_emissiveColour;
	specular	*= u_specularColour;
		
	
}

//-----------------------------------------------------------------------------
// Compute fog factor
//-----------------------------------------------------------------------------
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
	
	vec4 temp = vec4(a_vertNormal.xyz, 0.0);
	
	temp = u_world * temp;
	
	vec3 N = normalize(temp.xyz);
	vec3 posToEye = u_eyePosition - pos_ws.xyz;
	vec3 E = normalize(posToEye);
	
	vec3 diffuse = vec3(0.0, 0.0, 0.0);
	vec3 specular = vec3(0.0, 0.0, 0.0);
	
	float fogFactor = 0.0;
	
	ComputeLights(E, N, diffuse, specular);
	ComputeFogFactor(length(posToEye), fogFactor);
	
	v_diffuse	= vec4(diffuse.rgb * u_colour.rgb, u_colour.a) * a_vertColour;
	
	v_specular = vec4(0.0, 0.0, 0.0, 0.0);
	v_specular.xyz = specular;
	v_specular.w = fogFactor;

	v_texCoord = a_vertTexcoord;
}
