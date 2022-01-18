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
        }

        public void Run()
        {  
            render.Run();
            //Timer.Start();
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

        public void RenderCallback()
        {
            //Timer.Tick();
            //Log.Info(GetFPS());
        }

        private float GetFPS()
        {
            return 60/Timer.DeltaTime;
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Escape)
            {
                window.Close();
            }
        }

        public void Dispose()
        {
            render?.Dispose();
            window?.Dispose();
        }
    }
}
