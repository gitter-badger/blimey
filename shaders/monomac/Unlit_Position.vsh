// About Attributes, Uniforms And Varyings
//
// There are three types of inputs and outputs in a shader: uniforms, attributes and varyings.
//
// Uniforms are values which do not change during a rendering, 
// for example the light position or the light color. Uniforms 
// are available in vertex and fragment shaders. Uniforms are read-only.
//
// Attributes are only available in vertex shader and they are 
// input values which change every vertex, for example the vertex 
// position or normals. Attributes are read-only.
//
// Varyings are used for passing data from a vertex shader to a 
// fragment shader. Varyings are (perspective correct) interpolated 
// across the primitive. Varyings are read-only in fragment shader 
//  but are read- and writeable in vertex shader (but be careful, reading
//  a varying type before writing to it will return an undefined value). 
// If you want to use varyings you have to declare the same varying 
// in your vertex shader and in your fragment shader.
//
// All uniform, attribute and varying types HAVE to be global. 
// You are not allowed to specify a uniform/attribute/varying type in a function or a void.


attribute vec4 a_vertPos;

uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;

varying vec4 v_tint;

void main()
{
	gl_Position = u_proj * u_view * u_world * a_vertPos;
	
	v_tint = u_colour;
}
