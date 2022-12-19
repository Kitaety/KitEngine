#version 460 core

in vec4 aPosition;
in vec4 aColor;
out vec4 inColorFrag;

layout (location = 20) uniform  mat4 modelView;

void main()
{
    inColorFrag = aColor;
    gl_Position = modelView * aPosition;
}