uniform lowp vec3 u_FogColour;
uniform sampler2D s_tex0;

varying lowp vec2 v_texCoord;
varying lowp vec4 v_diffuse;
varying lowp vec4 v_specular;

void main()
{
	lowp vec4 texColour = texture2D(s_tex0, v_texCoord);
	
	lowp vec4 colour = (v_diffuse * texColour) + vec4(v_specular.xyz, 0.0);
	
	colour.rgb = mix(colour.rgb, u_FogColour, v_specular.w);

	gl_FragColor = colour;
}
