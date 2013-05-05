uniform lowp vec3 u_FogColour;

varying lowp vec4 v_diffuse;
varying lowp vec4 v_specular;

void main()
{
	lowp float fogFactor = v_specular.w;
	
	gl_FragColor = mix(
		v_diffuse + vec4(v_specular.rgb, v_diffuse.a),
		vec4(u_FogColour.rgb, v_diffuse.a),
		fogFactor
		);
	
}
