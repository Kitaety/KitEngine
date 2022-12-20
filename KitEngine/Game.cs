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

        private double _frameTime = 0.0f;
        private float _fps = 0.0f;

        public ShaderProgram ShaderProgram { private set; get; }
        private Voxel _voxel;
        private Voxel _voxel1;
        private Voxel _voxel2;


        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, VSyncMode vSyncMode = VSyncMode.Off) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            VSync = vSyncMode;
            CursorState = CursorState.Hidden;
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
            _voxel = new Voxel(new Vector3(1.5f, 0.0f, -7.0f), new[] { 1.0f, 0.0f, 0.0f, 1.0f });
            _voxel1 = new Voxel(new Vector3(0.0f, 0.0f, -7.0f), new[] { 0.0f, 1.0f, 0.0f, 1.0f });
            _voxel2 = new Voxel(new Vector3(-1.5f, 0.0f, -7.0f), new[] { 0.0f, 0.0f, 1.0f, 1.0f });
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
        // Note that we're translating the scene in the reverse direction of where we want to move.
        Matrix4 _cameraPosition = Matrix4.CreateTranslation(0.0f, 0.0f, 0.0f);
        private void Draw()
        {
            ShaderProgram.ActivateProgram();
            _voxel.Render(ref projection, ref _cameraPosition);
            _voxel1.Render(ref projection, ref _cameraPosition);
            _voxel2.Render(ref projection, ref _cameraPosition);
            ShaderProgram.DeactivateProgram();
        }

        private Matrix4 projection;
        protected override void OnResize(ResizeEventArgs e)
        {
            projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(45.0f), (float)e.Width / e.Height,
                0.1f, 100.0f);
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
            if (input.IsKeyPressed(Keys.F1))
            {
                CursorState = CursorState == CursorState.Hidden ? CursorState.Normal : CursorState.Hidden;
            }
        }
    }
}
