attribute vec4 a_vertPos;
attribute vec2 a_vertTexcoord;

uniform mat4 u_WorldViewProj;
uniform vec4 u_MaterialColour;

varying lowp vec2 v_texCoord;
varying lowp vec4 v_tint;

void main()
{
	gl_Position = u_WorldViewProj * a_vertPos;
	
	v_texCoord = a_vertTexcoord;
	
	v_tint = u_MaterialColour;
}