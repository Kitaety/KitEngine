using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Windows;
using KitEngine.Common;
using KitEngine.InputSystem;
using KitEngine.Models;
using KitEngine.RenderSystem;

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

        public Core()
        {
            Log.Info("Start Application");

            //setup properties
            width = 640;
            height = 640;

            InitializeWindow();
            render = new Render(window, RenderCallback);

            Input.Instance.KeyUp += OnKeyUp;
            Input.Instance.KeyDown += OnKeyDown;
            Input.Instance.MouseMove += OnMouseMove;

            Voxel = CreateVoxel();
        }

        public void Run()
        {  
            render.Run();
            //Timer.Start();
        }

        public void RenderCallback()
        {
            //Timer.Tick();
            //Log.Info(GetFPS());
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
            window.AllowUserResizing = false;
            window.Text = "3D11 Sample";
            window.Name = "MainWindow";
            window.StartPosition = FormStartPosition.CenterScreen;
            
            Log.Success("Create Window");
        }

        private Voxel CreateVoxel()
        {
            Transform transform = new Transform();
            transform.Position = new SharpDX.Vector3(0, 0, 3);
            transform.Rotation = new SharpDX.Vector3(0, 0, 0);
            transform.Scale = new SharpDX.Vector3(1, 1, 1);

            Voxel voxel = new Voxel();
            voxel.Transform = transform;
            return voxel;
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
        }

        private void OnKeyDown(Keys key)
        { 
            Log.Info($"On Down {key}");
        }

        private void OnMouseMove(MouseMoveEventArgs args)
        {
            Log.Info($"Mouse Move V:{args.Vertical} H:{args.Horizontal}");
        }
    }
}
