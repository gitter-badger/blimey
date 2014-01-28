uniform sampler2D s_tex0;

varying vec2 v_texCoord;
varying vec4 v_tint;

void main()
{
	gl_FragColor = texture2D(s_tex0, v_texCoord) * v_tint;
}