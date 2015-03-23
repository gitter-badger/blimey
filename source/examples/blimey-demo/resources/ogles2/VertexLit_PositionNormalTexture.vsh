attribute mediump vec4 a_vertPos;
attribute mediump vec3 a_vertNormal;
attribute mediump vec2 a_vertTexcoord;

uniform mediump mat4 u_world;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
uniform mediump vec4 u_colour;

uniform mediump vec3 u_eyePosition;
uniform mediump vec3 u_emissiveColour;
uniform mediump vec3 u_specularColour;

uniform mediump vec3 u_li0Dir;
uniform mediump vec3 u_li0Diffuse;
uniform mediump vec3 u_li0Spec;

uniform mediump vec3 u_li1Dir;
uniform mediump vec3 u_li1Diffuse;
uniform mediump vec3 u_li1Spec;

uniform mediump vec3 u_li2Dir;
uniform mediump vec3 u_li2Diffuse;
uniform mediump vec3 u_li2Spec;

uniform mediump vec3 u_liAmbient;
uniform mediump float u_specularPower;

uniform mediump float u_fogEnabled;
uniform mediump float u_fogStart;
uniform mediump float u_fogEnd;
uniform mediump vec3 u_fogColour;


varying mediump vec4 v_diffuse;
varying mediump vec4 v_specular;
varying mediump vec2 v_texCoord;

vec4 lit(in mediump float NdotL, in mediump float NdotH, in mediump float m)
{

	mediump float ambient = 1.0;
	mediump float diffuse = max(NdotL, 0.0);
	mediump float specular = step(0.0, NdotL) * max(NdotH * m, 0.0);

	return vec4(ambient, diffuse, specular, 1.0);
}


//-----------------------------------------------------------------------------
// Compute lighting
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
void ComputeLights(in mediump vec3 E, in mediump vec3 N, out mediump vec3 diffuse, out mediump vec3 specular)
{

	diffuse = u_liAmbient;
	specular = vec3(0.0, 0.0, 0.0);
	
	// Directional Light 0
	mediump vec3 L = -u_li0Dir;
	mediump vec3 H = normalize(E + L);
	mediump vec2 ret = lit(dot(N, L), dot(N, H), u_specularPower).yz;
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
	
	mediump vec4 temp = vec4(a_vertNormal.xyz, 0.0);
	
	temp = u_world * temp;
	
	mediump vec3 N = normalize(temp.xyz);
	mediump vec3 posToEye = u_eyePosition - pos_ws.xyz;
	mediump vec3 E = normalize(posToEye);
	
	mediump vec3 diffuse = vec3(0.0, 0.0, 0.0);
	mediump vec3 specular = vec3(0.0, 0.0, 0.0);
	
	mediump float fogFactor = 0.0;
	
	ComputeLights(E, N, diffuse, specular);
	ComputeFogFactor(length(posToEye), fogFactor);
	
	v_diffuse	= vec4(diffuse.rgb * u_colour.rgb, u_colour.a);
	
	v_specular = vec4(0.0, 0.0, 0.0, 0.0);
	v_specular.xyz = specular;
	v_specular.w = fogFactor;

	v_texCoord = a_vertTexcoord;
}
