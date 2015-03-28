uniform mediump vec3 u_fogColour;

uniform mediump sampler2D s_tex0;

varying mediump vec4 v_diffuse;
varying mediump vec4 v_specular;
varying mediump vec2 v_texCoord;

void main()
{
	mediump float fogFactor = v_specular.w;
	
	gl_FragColor = mix(
		v_diffuse + vec4(v_specular.rgb, v_diffuse.a),
		vec4(u_fogColour.rgb, v_diffuse.a),
		fogFactor
		) * texture2D(s_tex0, v_texCoord);
	
}
