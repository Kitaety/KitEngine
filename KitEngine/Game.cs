using System.Collections;
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
            -0.25f, 0.25f, 0.5f, 1.0f,
            1.0f, 0.0f, 0.0f, 1.0f,

            -0.25f, -0.25f, 0.5f, 1.0f,
            1.0f, 0.0f, 0.0f, 1.0f,

            0.25f, -0.25f, 0.5f, 1.0f,
            1.0f, 0.0f, 0.0f, 1.0f,

            0.25f, 0.25f, 0.5f, 1.0f,
            1.0f, 0.0f, 0.0f, 1.0f,

            -0.25f, 0.25f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f, 1.0f,

            -0.25f, -0.25f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f, 1.0f,

            0.25f, -0.25f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f, 1.0f,

            0.25f, 0.25f, 0.0f, 1.0f,
            1.0f, 1.0f, 0.0f, 1.0f,
        };

        private uint[] _indexes = new uint[]
        {
            //front
            0, 1, 2,
            0, 2, 3,
            
            //up
            0, 4, 3,
            4, 7, 3,

            //down
            1, 2, 5,
            2, 6, 5,

            //right
            3, 7, 2,
            7, 6, 2,

            //left
            0, 1, 4,
            1, 5, 4,
            
            //back
            4, 5, 6,
            4, 6, 7,
        };

        private ShaderProgram _shaderProgram;
        private ArrayObject _vao;
        private BufferObject _vbo;
        private BufferObject _ebo;

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
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            
            //Remove in future
            //GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);

            _shaderProgram = new ShaderProgram(@"Shaders\base_shader.vert", @"Shaders\base_shader.frag");
            CreateVAO();
        }

        protected override void OnUnload()
        {
            _vao.Dispose();
            base.OnUnload();
            _shaderProgram.DeleteProgram();
        }

        private double _time = 0.0f;
        private Matrix4 _modelView;

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _time += e.Time;
            var k = (float)_time * 0.05f;
            var r1 = Matrix4.CreateRotationX(k * 13.0f);
            var r2 = Matrix4.CreateRotationY(k * 13.0f);
            var r3 = Matrix4.CreateRotationZ(k * 3.0f);
            _modelView = r1 * r2 * r3;
            
            CalculateFps(e.Time);
            Update();
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Draw();
            SwapBuffers();
        }

        private void CreateVAO()
        {
            int vertexArray = _shaderProgram.GetAttribProgram("aPosition");
            int colorArray = _shaderProgram.GetAttribProgram("aColor");

            _vbo = new BufferObject(BufferTarget.ArrayBuffer);
            _vbo.SetData(BufferUsageHint.StaticDraw, _vertices);
            _ebo = new BufferObject(BufferTarget.ElementArrayBuffer);
            _ebo.SetData(BufferUsageHint.StaticDraw, _indexes);

            _vao = new ArrayObject();
            _vao.Activate();

            _vao.AttachBuffer(_ebo);
            _vao.AttachBuffer(_vbo);

            _vao.AttribPointer(vertexArray, 4, VertexAttribPointerType.Float, 8 * sizeof(float), 0);
            _vao.AttribPointer(colorArray, 4, VertexAttribPointerType.Float, 8 * sizeof(float), 4 * sizeof(float));

            _vao.Deactivate();
            _vao.DisableAttribAll();
        }


        private void Draw()
        {
            _shaderProgram.ActivateProgram();
            _vao.Activate();

            GL.UniformMatrix4(20,
                false,
                ref _modelView);

            _vao.DrawElements(0, _indexes.Length, DrawElementsType.UnsignedInt);
            _shaderProgram.DeactivateProgram();
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
