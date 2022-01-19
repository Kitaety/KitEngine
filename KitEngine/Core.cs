using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Windows;
using KitEngine.Common;

namespace KitEngine
{
    class Core: IDisposable
    {
        private bool _appPaused;          // Is the application paused?
        private bool _running;            // Is the application running?

        protected GameTimer Timer { get; } = new GameTimer();

        protected RenderForm window;
        protected Render render;
        protected Input input;
        private int width;
        private int height;

        public Core()
        {
            Log.Info("Start Application");

            //setup properties
            width = 640;
            height = 640;

            InitializeWindow();
            render = new Render(window, RenderCallback);
            input = new Input();

            input.OnKeyUp += OnKeyUp;
            input.OnKeyDown += OnKeyDown; ;
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
        }

        private void OnKeyDown(Keys key)
        {
            
        }

    }
}
