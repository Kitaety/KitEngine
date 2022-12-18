using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace KitEngine
{
    public class Game : GameWindow
    {
        private double _frameTime = 0.0f;
        private float _fps = 0.0f;
        
        private float[] _vertices = new float[]
        {
            0.5f, 0.0f, 0.0f,
            0.5f, 0.5f, 0.0f,
            0.0f, 0.0f, 0.0f,
            0.0f, 0.5f, 0.0f,
            -0.5f, 0.0f, 0.0f,
            -0.5f, 0.5f, 0.0f
        };

        private float[] _colors = new float[]
        {
            1.0f, 0.0f, 0.0f, 1.0f,
            0.0f, 0.0f, 1.0f, 1.0f,
            0.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f, 1.0f,
            1.0f, 0.75f, 0.80f, 1.0f,
            1.0f, 0.65f, 0.0f, 1.0f
        };

        private int _indexVAO = 0;

        private ShaderProgram _shaderProgram;


        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, VSyncMode vSyncMode = VSyncMode.Off) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            VSync = vSyncMode;
            CursorState = CursorState.Hidden;
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.Black);
            //TODO return culling face
            // GL.Enable(EnableCap.CullFace);
            // GL.CullFace(CullFaceMode.Back);
            GL.PolygonMode(MaterialFace.Front, PolygonMode.Fill);
            GL.PolygonMode(MaterialFace.Back, PolygonMode.Line);

            _indexVAO = CreateVAOShaders();
            _shaderProgram = new ShaderProgram(@"Shaders\base_shader.vert", @"Shaders\base_shader.frag");
        }

        protected override void OnUnload()
        {
            base.OnUnload();
            DeleteVAOShaders();
            _shaderProgram.DeleteProgram();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            CalculateFps(e.Time);
            Update();
        }

        private int CreateVBO(float[] data)
        {
            int vbo = GL.GenBuffer();

            GL.BindBuffer(BufferTarget.ArrayBuffer, vbo);
            GL.BufferData(BufferTarget.ArrayBuffer, data.Length * sizeof(float), data, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            return vbo;
        }

        private int CreateVAOShaders()
        {
            int vao = GL.GenVertexArray();

            GL.BindVertexArray(vao);

            int vboVertex = CreateVBO(_vertices);
            int vboColor = CreateVBO(_colors);

            int vertexArray = 0;

            GL.EnableVertexAttribArray(vertexArray);


            GL.BindBuffer(BufferTarget.ArrayBuffer, vboVertex);
            GL.VertexAttribPointer(vertexArray, 3, VertexAttribPointerType.Float, true, 3 * sizeof(float), 0);
            // GL.BindBuffer(BufferTarget.ArrayBuffer, vboColor);
            // GL.VertexAttribPointer(4, ColorPointerType.Float, 0, 0);

            GL.BindVertexArray(0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            
            GL.DisableVertexAttribArray(vertexArray);

            return vao;
        }

        private void DrawVAOShaders()
        {
            _shaderProgram.ActivateProgram();
            GL.BindVertexArray(_indexVAO);
            GL.DrawArrays(PrimitiveType.TriangleStrip, 0, _vertices.Length / 3);
            _shaderProgram.DeactivateProgram();
            GL.BindVertexArray(0);
        }

        private void DeleteVAOShaders()
        {
            GL.BindVertexArray(0);
            GL.DeleteVertexArray(_indexVAO);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
         
            GL.Clear(ClearBufferMask.ColorBufferBit);
            DrawVAOShaders();
            SwapBuffers();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, e.Width, e.Height);
        }

        private void CalculateFps(double deltaTime)
        {
            _frameTime += deltaTime;
            _fps++;

            if (_frameTime >= 1)
            {
                Title = $"KitEngine FPS - {_fps}";
                _fps = 0;
                _frameTime = 0;
            }
        }

        private void Update()
        {
            KeyboardState input = KeyboardState;

            if (input.IsKeyDown(Keys.Escape))
            {
                Close();
            }
            if (input.IsKeyPressed(Keys.LeftAlt))
            {
                CursorState = CursorState == CursorState.Hidden ? CursorState.Normal : CursorState.Hidden;
            }
        }
    }
}
