uniform mediump vec3 u_fogColour;
uniform mediump sampler2D s_tex0;

varying mediump vec2 v_texCoord;
varying mediump vec4 v_diffuse;
varying mediump vec4 v_specular;

void main()
{
	mediump vec4 texColour = texture2D(s_tex0, v_texCoord);
	
	mediump vec4 colour = (v_diffuse * texColour) + vec4(v_specular.xyz, 0.0);
	
	//colour.rgb = mix(colour.rgb, u_fogColour, v_specular.w);

	gl_FragColor = colour;
}
