attribute vec4 a_vertPos;
attribute vec4 a_vertColour;

uniform mat4 u_WorldViewProj;
uniform vec4 u_MaterialColour;

varying lowp vec4 v_tint;

void main()
{
	gl_Position = u_WorldViewProj * a_vertPos;
	
	v_tint = a_vertColour * u_MaterialColour;
}