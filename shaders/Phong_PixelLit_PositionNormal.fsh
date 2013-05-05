uniform lowp vec3 u_EyePosition;
uniform lowp vec3 u_FogColour;

uniform lowp vec3 u_DirLight0Direction;
uniform lowp vec3 u_DirLight0DiffuseColour;
uniform lowp vec3 u_DirLight0SpecularColour;

uniform lowp vec3 u_DirLight1Direction;
uniform lowp vec3 u_DirLight1DiffuseColour;
uniform lowp vec3 u_DirLight1SpecularColour;

uniform lowp vec3 u_DirLight2Direction;
uniform lowp vec3 u_DirLight2DiffuseColour;
uniform lowp vec3 u_DirLight2SpecularColour;

uniform lowp vec4 u_MaterialColour;
uniform lowp vec3 u_AmbientLightColour;
uniform lowp vec3 u_EmissiveColour;
uniform lowp vec3 u_SpecularColour;
uniform lowp float u_SpecularPower;

varying lowp vec4 v_positionWS;
varying lowp vec3 v_normalWS;


//-----------------------------------------------------------------------------
// ComputePerPixelLights
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
void ComputePerPixelLights(in lowp vec3 E, in lowp vec3 N, out lowp vec3 diffuse, out lowp vec3 specular)
{
	diffuse = u_AmbientLightColour;
	specular = vec3(0.0, 0.0, 0.0);
	
	// Light0
	lowp vec3 L = -u_DirLight0Direction;
	lowp vec3 H = normalize(E + L);
	lowp float dt = max(0.0,dot(L,N));
	diffuse += u_DirLight0DiffuseColour * dt;
	if (dt != 0.0)
		specular += u_DirLight0SpecularColour * pow(max(0.0,dot(H,N)), u_SpecularPower);

	// Light1
	L = -u_DirLight1Direction;
	H = normalize(E + L);
	dt = max(0.0,dot(L,N));
	diffuse += u_DirLight1DiffuseColour * dt;
	if (dt != 0.0)
		specular += u_DirLight1SpecularColour * pow(max(0.0,dot(H,N)), u_SpecularPower);

	// Light2
	L = -u_DirLight2Direction;
	H = normalize(E + L);
	dt = max(0.0, dot(L,N));
	diffuse += u_DirLight2DiffuseColour * dt;
	if (dt != 0.0)
		specular += u_DirLight2SpecularColour * pow(max(0.0,dot(H,N)), u_SpecularPower);
	
	diffuse *= u_MaterialColour.rgb;
	diffuse += u_EmissiveColour;
	specular *= u_SpecularColour;	
	
}


void main()
{
	lowp vec3 posToEye = u_EyePosition - v_positionWS.xyz;
	
	lowp vec3 N = normalize(v_normalWS);
	lowp vec3 E = normalize(posToEye);
	
	lowp vec3 diffuseResult = vec3(0.0, 0.0, 0.0);
	lowp vec3 specularResult = vec3(0.0, 0.0, 0.0);
	ComputePerPixelLights(E, N, diffuseResult, specularResult);

	lowp vec4 diffuse = vec4(diffuseResult * u_MaterialColour.rgb, u_MaterialColour.a);
	lowp vec4 colour = diffuse + vec4(specularResult.x, specularResult.y, specularResult.z, 0);
	//colour.rgb = mix(colour.rgb, u_FogColour, v_positionWS);
	
	gl_FragColor = colour;
	
}