using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace KitEngine.Render
{
    public class ShaderProgram
    {
        private readonly int _program = 0;

        public ShaderProgram(string vertexShaderFile, string fragmentShaderFile)
        {
            int vertexShader = CreateShader(ShaderType.VertexShader, vertexShaderFile);
            int fragmentShader = CreateShader(ShaderType.FragmentShader, fragmentShaderFile);

            _program = GL.CreateProgram();
            GL.AttachShader(_program, vertexShader);
            GL.AttachShader(_program, fragmentShader);

            GL.LinkProgram(_program);
            GL.GetProgram(_program, GetProgramParameterName.LinkStatus, out int code);

            if (code != (int)All.True)
            {
                string errorMessage = GL.GetProgramInfoLog(_program);
                throw new Exception($"Program #{_program} Link is failed. \n\n {errorMessage}");
            }

            DeleteShader(vertexShader);
            DeleteShader(fragmentShader);
        }

        public void ActivateProgram() => GL.UseProgram(_program);

        public void DeactivateProgram() => GL.UseProgram(0);

        public void DeleteProgram() => GL.DeleteProgram(_program);

        public int GetAttributeProgram(string name) => GL.GetAttribLocation(_program, name);
        public int GetUniformProgram(string name) => GL.GetUniformLocation(_program, name);

        public void SetUniform(string name, Matrix4 data)
        {
            GL.UniformMatrix4(GetUniformProgram(name),
                true,
                ref data);
        }

        private int CreateShader(ShaderType type, string filePath)
        {
            string shaderStr = File.ReadAllText(filePath);
            int shader = GL.CreateShader(type);
            GL.ShaderSource(shader, shaderStr);
            GL.CompileShader(shader);

            GL.GetShader(shader, ShaderParameter.CompileStatus, out int code);

            if (code != (int)All.True)
            {
                string errorMessage = GL.GetShaderInfoLog(shader);
                throw new Exception($"Shader #{shader} Compilation is failed. \n\n {errorMessage}");
            }

            return shader;
        }

        private void DeleteShader(int shader)
        {
            GL.DetachShader(_program, shader);
            GL.DeleteShader(shader);
        }
    }

    public static class ShaderProgramAttributes
    {
        public const string Position = "inPosition";
        public const string Color = "inColor";
    }

    public static class ShaderProgramUniforms
    {
        public const string View = "view";
        public const string Projection = "projection";
        public const string Model = "model";
    }
}
