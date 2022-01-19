struct VSOut
{
    float4 position : SV_POSITION;
    float4 color : COLOR;
};

VSOut main(float4 position : POSITION, float4 color : COLOR)
{
    VSOut output;

    output.position = position;
    output.position.x = position.x / position.z;
    output.position.y = position.y / position.z;
    output.position.z = 0;
    output.color = color;

    return output;
}