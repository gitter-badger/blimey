attribute vec4 a_Position;
attribute vec2 a_Texcoord0;

uniform mat4 u_WorldViewProj;
uniform vec4 u_MaterialColour;

varying lowp vec2 v_texCoord;
varying lowp vec4 v_tint;

void main()
{
	gl_Position = u_WorldViewProj * a_Position;
	
	v_texCoord = a_Texcoord0;
	
	v_tint = u_MaterialColour;
}