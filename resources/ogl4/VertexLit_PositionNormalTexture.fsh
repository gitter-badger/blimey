uniform vec3 u_fogColour;

uniform sampler2D s_tex0;

varying vec4 v_diffuse;
varying vec4 v_specular;
varying vec2 v_texCoord;

void main()
{
	float fogFactor = v_specular.w;
	
	gl_FragColor = mix(
		v_diffuse + vec4(v_specular.rgb, v_diffuse.a),
		vec4(u_fogColour.rgb, v_diffuse.a),
		fogFactor
		) * texture2D(s_tex0, v_texCoord);
	
}
