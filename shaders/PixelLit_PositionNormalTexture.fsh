uniform mediump vec3 u_eyePosition;
uniform mediump vec3 u_fogColour;

uniform mediump vec3 u_dirLight0Direction;
uniform mediump vec3 u_dirLight0DiffuseColour;
uniform mediump vec3 u_dirLight0SpecularColour;

uniform mediump vec3 u_dirLight1Direction;
uniform mediump vec3 u_dirLight1DiffuseColour;
uniform mediump vec3 u_dirLight1SpecularColour;

uniform mediump vec3 u_dirLight2Direction;
uniform mediump vec3 u_dirLight2DiffuseColour;
uniform mediump vec3 u_dirLight2SpecularColour;

uniform mediump vec3 u_ambientLightColour;
uniform mediump vec3 u_emissiveColour;
uniform mediump vec3 u_specularColour;
uniform mediump float u_specularPower;

uniform mediump sampler2D s_tex0;

varying mediump vec4 v_positionWS;
varying mediump vec3 v_normalWS;
varying mediump vec4 v_tint;
varying mediump vec2 v_texCoord;

//-----------------------------------------------------------------------------
// ComputePerPixelLights
// E: Eye-Vector
// N: Unit vector normal in world space
//-----------------------------------------------------------------------------
void ComputePerPixelLights(in mediump vec3 E, in mediump vec3 N, out mediump vec3 diffuse, out mediump vec3 specular)
{
	diffuse = u_ambientLightColour;
	specular = vec3(0.0, 0.0, 0.0);
	
	// Light0
	mediump vec3 L = -u_dirLight0Direction;
	mediump vec3 H = normalize(E + L);
	mediump float dt = max(0.0,dot(L,N));
	diffuse += u_dirLight0DiffuseColour * dt;
	if (dt != 0.0)
		specular += u_dirLight0SpecularColour * pow(max(0.0,dot(H,N)), u_specularPower);

	// Light1
	L = -u_dirLight1Direction;
	H = normalize(E + L);
	dt = max(0.0,dot(L,N));
	diffuse += u_dirLight1DiffuseColour * dt;
	if (dt != 0.0)
		specular += u_dirLight1SpecularColour * pow(max(0.0,dot(H,N)), u_specularPower);

	// Light2
	L = -u_dirLight2Direction;
	H = normalize(E + L);
	dt = max(0.0, dot(L,N));
	diffuse += u_dirLight2DiffuseColour * dt;
	if (dt != 0.0)
		specular += u_dirLight2SpecularColour * pow(max(0.0,dot(H,N)), u_specularPower);
	
	diffuse *= v_tint.rgb;
	diffuse += u_emissiveColour;
	specular *= u_specularColour;	
	
}


void main()
{
	mediump vec3 posToEye = u_eyePosition - v_positionWS.xyz;
	
	mediump vec3 N = normalize(v_normalWS);
	mediump vec3 E = normalize(posToEye);
	
	mediump vec3 diffuseResult = vec3(0.0, 0.0, 0.0);
	mediump vec3 specularResult = vec3(0.0, 0.0, 0.0);
	ComputePerPixelLights(E, N, diffuseResult, specularResult);

	mediump vec4 diffuse = vec4(diffuseResult * v_tint.rgb, v_tint.a);
	mediump vec4 colour = diffuse + vec4(specularResult.x, specularResult.y, specularResult.z, 0);
	
	colour.rgb = mix(colour.rgb, u_fogColour, v_positionWS.w);
	
	gl_FragColor = colour * texture2D(s_tex0, v_texCoord);
}