attribute vec4 a_vertPos;
attribute vec4 a_vertColour;

uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;

varying vec4 v_tint;

void main()
{
	gl_Position = u_proj * u_view * u_world * a_vertPos;
	
	v_tint = a_vertColour * u_colour;
}