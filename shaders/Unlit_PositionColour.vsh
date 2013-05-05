attribute vec4 a_Position;
attribute vec4 a_Colour0;

uniform mat4 u_WorldViewProj;
uniform vec4 u_MaterialColour;

varying lowp vec4 v_tint;

void main()
{
	gl_Position = u_WorldViewProj * a_Position;
	
	v_tint = a_Colour0 * u_MaterialColour;
}