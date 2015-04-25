attribute vec4 a_vertPos;
attribute vec3 a_vertNormal;
attribute vec4 a_vertColour;

// UNIFORMS

uniform mat4 u_world;
uniform mat4 u_view;
uniform mat4 u_proj;
uniform vec4 u_colour;

varying vec4 v_positionWS;
varying vec3 v_normalWS;
varying vec4 v_tint;

void main()
{
    vec4 pos_ws = u_world * a_vertPos;
    vec4 pos_vs = u_view * pos_ws;
    vec4 pos_ps = u_proj * pos_vs;

    gl_Position = pos_ps;

    v_positionWS = pos_ws;
    v_normalWS = normalize ((u_world * vec4 (a_vertNormal.xyz, 0.0)).xyz);
    v_tint = a_vertColour * u_colour;
}
