uniform vec3 u_fogColour;

varying vec4 v_diffuse;
varying vec4 v_specular;

void main()
{
	float fogFactor = v_specular.w;
	
	gl_FragColor = mix(
		v_diffuse + vec4(v_specular.rgb, v_diffuse.a),
		vec4(u_fogColour.rgb, v_diffuse.a),
		fogFactor
		);
	
}
