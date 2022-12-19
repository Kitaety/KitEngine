#version 460 core

in vec3 aPosition;
in vec4 aColor;
out vec4 inColorFrag;

void main()
{
    inColorFrag = aColor;
    gl_Position = vec4(aPosition, 1.0);
}