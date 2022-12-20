#version 460 core

in vec4 aPosition;
in vec4 aColor;
out vec4 inColorFrag;

layout (location = 20) uniform  mat4 transform;
layout (location = 21) uniform  mat4 view;
layout (location = 22) uniform  mat4 projection;

void main()
{
    inColorFrag = aColor;
    gl_Position = aPosition * transform * view * projection;
}