using OpenTK.Graphics.OpenGL4;

namespace KitEngine
{
    internal class ShaderProgram
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

        public int GetAttribProgram(string name) => GL.GetAttribLocation(_program, name);

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
}
