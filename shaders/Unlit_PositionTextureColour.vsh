attribute mediump vec4 a_vertPos;
attribute mediump vec2 a_vertTexcoord;
attribute mediump vec4 a_vertColour;

uniform mediump mat4 u_world;
uniform mediump mat4 u_view;
uniform mediump mat4 u_proj;
uniform mediump vec4 u_colour;

varying mediump vec2 v_texCoord;
varying mediump vec4 v_tint;

void main()
{
	gl_Position = u_proj * u_view * u_world * a_vertPos;
	
	v_texCoord = a_vertTexcoord;
	
	v_tint = a_vertColour * u_colour;
}