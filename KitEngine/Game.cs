using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;

namespace KitEngine
{
    public class Game : GameWindow
    {
        private double _frameTime = 0.0f;
        private float _fps = 0.0f;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, VSyncMode vSyncMode = VSyncMode.Off) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            VSync = vSyncMode;
            CursorState = CursorState.Hidden;
            // CursorVisible = false;
        }
        
        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.Black);
        }

        protected override void OnUnload()
        {
            base.OnUnload();
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
         
            GL.Clear(ClearBufferMask.ColorBufferBit);

            GL.Begin(PrimitiveType.QuadStrip);

            GL.Color4(Color4.Red);
            GL.Vertex2(0.5f, 0.0f);
            GL.Color4(Color4.Blue);
            GL.Vertex2(0.5f, 0.5f);
            GL.Color4(Color4.Green);
            GL.Vertex2(0.0f, 0.0f);
            GL.Color4(Color4.Yellow);
            GL.Vertex2(0.0f, 0.5f);

            GL.Color4(Color4.Pink);
            GL.Vertex2(-0.5f, 0.0f);
            GL.Color4(Color4.Orange);
            GL.Vertex2(-0.5f, 0.5f);
            GL.End();

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
