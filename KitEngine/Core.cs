using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Windows;
using KitEngine.Common;
using KitEngine.InputSystem;
using KitEngine.Models;
using KitEngine.RenderSystem;
using SharpDX;
using Color = SharpDX.Color;

namespace KitEngine
{
    class Core: IDisposable
    {
        private bool _appPaused;          // Is the application paused?
        private bool _running;            // Is the application running?

        protected GameTimer Timer { get; } = new GameTimer();

        protected RenderForm window;
        protected Render render;
        private int width;
        private int height;

        private Voxel Voxel;
        private Camera Camera;
        public Core()
        {
            Log.Info("Start Application");

            width = 640;
            height = 640;

            InitializeWindow();
            render = new Render(window, RenderCallback);

            Input.Instance.KeyUp += OnKeyUp;
            Input.Instance.KeyDown += OnKeyDown;
            Input.Instance.MouseMove += OnMouseMove;

            Voxel = new Voxel(new Vector3(0, 0, 0), Color.Blue);
            Camera = new Camera(new Vector3(0, 0, -15), new Vector3(0f, 0f, 0f));
            render.SetCamera(Camera);
        }

        public void Run()
        {  
            render.Run();
        }

        public void RenderCallback()
        {
            render.SetVertecies(Voxel.Vertices);
            render.SetIndeces(Voxel.VertexIndices);
        }

        public void Dispose()
        {
            render?.Dispose();
            window?.Dispose();
        }

        private void InitializeWindow()
        {
            //create window
            Log.Info("Create Window");
            window = new RenderForm();
            window.ClientSize = new Size(width, height);
            window.MinimumSize = new Size(200, 200);
            window.IsFullscreen = false;
            window.Text = "3D11 Sample";
            window.Name = "MainWindow";
            window.StartPosition = FormStartPosition.CenterScreen;
            
            Log.Success("Create Window");
        }

        private float GetFPS()
        {
            return 60/Timer.DeltaTime;
        }

        private void OnKeyUp(Keys key)
        {
            if (key == Keys.Escape)
            {
                window.Close();
            }
            Log.Info($"On Up {key}");
            
            switch (key)
            {
                case Keys.Escape:
                {
                    window.Close();
                    break;
                }

                case Keys.A:
                {
                    Camera.Position.X -= 0.1f;
                    break;
                }
                case Keys.W:
                {
                    Camera.Position.Z += 0.1f;
                    break;
                }
                case Keys.S:
                {
                    Camera.Position.Z -= 0.1f;
                    break;
                }
                case Keys.D:
                {
                    Camera.Position.X += 0.1f;
                    break;
                }
            }
        }

        private void OnKeyDown(Keys key)
        {
            Log.Info($"On Down {key}");
        }

        private void OnMouseMove(MouseMoveEventArgs args)
        {
            Camera.Rotation.X += args.Vertical * 0.01f;
            Camera.Rotation.Y += args.Horizontal * 0.01f;
            Log.Info($"Mouse Move V:{args.Vertical} H:{args.Horizontal}");
        }
    }
}
