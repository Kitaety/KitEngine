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
        public static Game Instance = null;

        private bool _isDebug = false;
        private double _frameTime = 0.0f;
        private float _fps = 0.0f;
        private float _deltaTime = 0.0f;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        public ShaderProgram ShaderProgram { private set; get; }
        private Voxel _voxel;
        private Voxel _voxel1;
        private Voxel _voxel2;
        private Camera _camera;



        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, VSyncMode vSyncMode = VSyncMode.Off) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            VSync = vSyncMode;
            CursorState = CursorState.Grabbed;
            Game.Instance = this;
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            ShaderProgram = new ShaderProgram(@"Shaders\base_shader.vert", @"Shaders\base_shader.frag");
            _voxel = new Voxel(new Vector3(1.5f, 0.0f, -1.0f), new[] { 1.0f, 0.0f, 0.0f, 1.0f });
            _voxel1 = new Voxel(new Vector3(0.0f, 0.0f, -3.5f), new[] { 0.0f, 1.0f, 0.0f, 1.0f });
            _voxel2 = new Voxel(new Vector3(-1.5f, 0.0f, -7.0f), new[] { 0.0f, 0.0f, 1.0f, 1.0f });

            _camera = new Camera(Vector3.Zero, Size.X/(float)Size.Y);
        }

        protected override void OnUnload()
        {
            _voxel.Dispose();
            base.OnUnload();
            ShaderProgram.DeleteProgram();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
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

        private void Draw()
        {
            ShaderProgram.ActivateProgram();
            Matrix4 viewMatrix = _camera.ViewMatrix;
            Matrix4 projection = _camera.ProjectionMatrix;
            _voxel.Render(ref projection, ref viewMatrix);
            _voxel1.Render(ref projection, ref viewMatrix);
            _voxel2.Render(ref projection, ref viewMatrix);
            ShaderProgram.DeactivateProgram();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            _camera.AspectRatio = (float)e.Width / e.Height;
            base.OnResize(e);
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        private void CalculateFps(double deltaTime)
        {
            _deltaTime = (float)deltaTime;
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
            if (input.IsKeyPressed(Keys.F1))
            {
                ActivateDebugMode(!_isDebug);
            }

            //Camera Input
            const float sensitivity = 0.2f;
            const float cameraSpeed = 1.5f;

            if (input.IsKeyDown(Keys.W))
            {
                _camera.Position += _camera.Front * cameraSpeed * _deltaTime;
            }

            if (input.IsKeyDown(Keys.S))
            {
                _camera.Position -= _camera.Front * cameraSpeed * _deltaTime;
            }
            if (input.IsKeyDown(Keys.A))
            {
                _camera.Position -= _camera.Right * cameraSpeed * _deltaTime;
            }
            if (input.IsKeyDown(Keys.D))
            {
                _camera.Position += _camera.Right * cameraSpeed * _deltaTime;
            }
            if (input.IsKeyDown(Keys.Space))
            {
                _camera.Position += _camera.Up * cameraSpeed * _deltaTime;
            }
            if (input.IsKeyDown(Keys.LeftShift))
            {
                _camera.Position -= _camera.Up * cameraSpeed * _deltaTime;
            }

            var mouse = MouseState;

            if (_firstMove)
            {
                _lastPos = new Vector2(mouse.X, mouse.Y);
                _firstMove = false;
            }
            else
            {
                var deltaX = mouse.X - _lastPos.X;
                var deltaY = mouse.Y - _lastPos.Y;
                _lastPos = new Vector2(mouse.X, mouse.Y);

                _camera.Yaw += deltaX * sensitivity;
                _camera.Pitch -= deltaY * sensitivity;
            }
        }

        private void ActivateDebugMode(bool isDebug)
        {
            _isDebug = isDebug;
            if (isDebug)
            {
                CursorState = CursorState.Normal;
            }
            else
            {
                CursorState = CursorState.Grabbed;
            }
        }
    }
}
