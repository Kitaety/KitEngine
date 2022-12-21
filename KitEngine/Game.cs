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
        public static Game Instance;

        private bool _isDebug = false;
        private double _frameTime = 0.0f;
        private float _fps = 0.0f;
        private float _deltaTime = 0.0f;
        private bool _firstMove = true;
        private Vector2 _lastPos;

        public ShaderProgram ShaderProgram { private set; get; }
        private Camera _camera;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, VSyncMode vSyncMode = VSyncMode.Off) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            VSync = vSyncMode;
            CursorState = CursorState.Grabbed;
            Game.Instance = this;
        }

        private GameObject obj;
        private GameObject obj1;

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            ShaderProgram = new ShaderProgram(@"Shaders\base_shader.vert", @"Shaders\base_shader.frag");
            
            obj = new GameObject("test");
            obj.Position = new Vector3(0f, 0, -6);
            obj.Rotation = new Vector3(0, 0, 0);
            List<Voxel> voxels = new List<Voxel>
            {
                new Voxel(new Vector3(0.0f, 0.0f, 0.0f), new[] { 0.0f, 1.0f, 0.0f, 1.0f }),
                new Voxel(new Vector3(1.0f, 0.0f, 0.0f), new[] { 1.0f, 0.0f, 0.0f, 1.0f }),
                new Voxel(new Vector3(2.0f, 0.0f, 0.0f), new[] { 0.0f, 0.0f, 1.0f, 1.0f }),
                new Voxel(new Vector3(2.0f, 0.0f, 1.0f), new[] { 0.0f, 0.0f, 1.0f, 1.0f })
            };
            obj.Mesh = voxels;

            obj1 = new GameObject("test");
            obj1.Position = new Vector3(0f, 0, -4);
            obj1.Rotation = new Vector3(0, 0, 0);
            List<Voxel> voxels1 = new List<Voxel>
            {
                new Voxel(new Vector3(0.0f, 0.0f, 0.0f), new[] { 1.0f, 0.0f, 1.0f, 1.0f }),
                new Voxel(new Vector3(1.0f, 0.0f, 1.0f), new[] { 1.0f, 1.0f, 0.0f, 1.0f }),
                new Voxel(new Vector3(2.0f, 0.0f, 2.0f), new[] { 1.0f, 1.0f, 1.0f, 1.0f })
            };
            obj1.Mesh = voxels1;

            _camera = new Camera(Vector3.Zero, Size);
        }

        protected override void OnUnload()
        {
            obj.Dispose();
            obj1.Dispose();
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

            ShaderProgram.SetUniform(ShaderProgramUniforms.View, viewMatrix);
            ShaderProgram.SetUniform(ShaderProgramUniforms.Projection, projection);

            obj.Render();
            obj1.Render();

            ShaderProgram.DeactivateProgram();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            _camera.ViewSize = Size;
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
            if (input.IsKeyPressed(Keys.F2))
            {
                _camera.IsOrthographic = !_camera.IsOrthographic;
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

            var rot = obj.Rotation;

            if (input.IsKeyDown(Keys.Q))
            {
                rot.Y += _deltaTime * 15f;
            }
            if (input.IsKeyDown(Keys.E))
            {
                rot.Y -= _deltaTime * 15f;
            }

            if (input.IsKeyDown(Keys.R))
            {
                rot.X += _deltaTime * 15f;
            }
            if (input.IsKeyDown(Keys.T))
            {
                rot.X -= _deltaTime * 15f;
            }

            if (input.IsKeyDown(Keys.Y))
            {
                rot.Z += _deltaTime * 15f;
            }
            if (input.IsKeyDown(Keys.U))
            {
                rot.Z -= _deltaTime * 15f;
            }
            obj.Rotation = rot;
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
