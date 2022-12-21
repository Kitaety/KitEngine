#version 460 core

in vec4 inPosition;
in vec4 inColor;
out vec4 inColorFrag;

uniform mat4 model;
uniform mat4 view;
uniform mat4 projection;

void main()
{
    inColorFrag = inColor;
    gl_Position =  inPosition * model * view * projection;
}