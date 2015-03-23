uniform vec3 u_eyePosition;
uniform vec3 u_fogColour;

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
uniform vec3 u_emissiveColour;
uniform vec3 u_specularColour;
uniform float u_specularPower;

varying vec4 v_positionWS;
varying vec3 v_normalWS;
varying vec4 v_tint;

//-----------------------------------------------------------------------------
// ComputePerPixelLights
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
void ComputePerPixelLights(in vec3 E, in vec3 N, out vec3 diffuse, out vec3 specular)
{
	diffuse = u_liAmbient;
	specular = vec3(0.0, 0.0, 0.0);
	
	// Light0
	vec3 L = -u_li0Dir;
	vec3 H = normalize(E + L);
	float dt = max(0.0,dot(L,N));
	diffuse += u_li0Diffuse * dt;
	if (dt != 0.0)
		specular += u_li0Spec * pow(max(0.0,dot(H,N)), u_specularPower);

	// Light1
	L = -u_li1Dir;
	H = normalize(E + L);
	dt = max(0.0,dot(L,N));
	diffuse += u_li1Diffuse * dt;
	if (dt != 0.0)
		specular += u_li1Spec * pow(max(0.0,dot(H,N)), u_specularPower);

	// Light2
	L = -u_li2Dir;
	H = normalize(E + L);
	dt = max(0.0, dot(L,N));
	diffuse += u_li2Diffuse * dt;
	if (dt != 0.0)
		specular += u_li2Spec * pow(max(0.0,dot(H,N)), u_specularPower);
	
	diffuse += u_emissiveColour;
	specular *= u_specularColour;	
	
}


void main()
{
	vec3 posToEye = u_eyePosition - v_positionWS.xyz;
	
	vec3 N = normalize(v_normalWS);
	vec3 E = normalize(posToEye);
	
	vec3 diffuseResult = vec3(0.0, 0.0, 0.0);
	vec3 specularResult = vec3(0.0, 0.0, 0.0);
	ComputePerPixelLights(E, N, diffuseResult, specularResult);

	vec4 diffuse = vec4(diffuseResult * v_tint.rgb, v_tint.a);
	vec4 colour = diffuse + vec4(specularResult.x, specularResult.y, specularResult.z, 0);
	
	colour.rgb = mix(colour.rgb, u_fogColour, v_positionWS.w);
	
	gl_FragColor = colour;
}