using System;
using System.Drawing;
using System.Windows.Forms;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using D3D12 = SharpDX.Direct3D12;
using D3D11 = SharpDX.Direct3D11;


namespace KitEngine
{
    class Core: IDisposable
    {
        private bool _appPaused;          // Is the application paused?
        private bool _running;            // Is the application running?

        protected GameTimer Timer { get; } = new GameTimer();

        private RenderForm window;
        private int width;
        private int height;
        ModeDescription backBufferDesc;
        private D3D11.Device device;
        private SwapChain swapChain;

        private D3D11.DeviceContext deviceContext;
        //private D3D12.CommandQueue commandQueue;
        private D3D11.RenderTargetView renderTargetView;
        private RawVector3[] vertices = new RawVector3[] { new RawVector3(-0.5f, 0.5f, 0.0f), new RawVector3(0.5f, 0.5f, 0.0f), new RawVector3(0.0f, -0.5f, 0.0f) };
        private D3D11.Buffer triangleVertexBuffer;

        public Core()
        {
            Log.Info("Start Application");

            //setup properties
            width = 640;
            height = 640;
            backBufferDesc = new ModeDescription(width, height, new Rational(60, 1), Format.R8G8B8A8_UNorm);


            InitializeWindow();
            InitializeDeviceResources();
            InitializeTriangle();
        }
        public void Run()
        {  
            Log.Info("Run RenderLoop");
            RenderLoop.Run(window,RenderCallback);
        }

        private void InitializeWindow()
        {
            Log.Info("Create Window");
            //create window
            window = new RenderForm();
            window.ClientSize = new Size(width, height);
            window.MinimumSize = new Size(200, 200);
            window.IsFullscreen = false;
            window.AllowUserResizing = false;
            window.Text = "3D12 Sample";
            window.Name = "MainWindow";
            window.StartPosition = FormStartPosition.CenterScreen;

            //initialization events
            window.KeyUp += OnKeyUp;
            Log.Success("Create Window");
        }
        private void InitializeDeviceResources()
        {
            Log.Info("Create Device Resources");

            Log.Info("Create Factory");
            Factory4 factory = new Factory4();
            Log.Success("Create Factory");

            Log.Info("Create Device");
            Adapter adapter = factory.GetAdapter(0);
            Log.Info($"Usage {adapter.Description.Description}");
            device = new D3D11.Device(adapter, D3D11.DeviceCreationFlags.Debug);
            Log.Success("Create Device");

            //Describe and create the command queue.
            //Хз чего не работает, возможно неподдерживается на моем ноуте.
            //TODO: Проверить на ноуте Риты
            //Log.Info("Create Command Queue");
            //D3D12.CommandQueueDescription commandQueueDescription = new D3D12.CommandQueueDescription();
            //commandQueueDescription.Flags = D3D12.CommandQueueFlags.None;
            //commandQueueDescription.Type = D3D12.CommandListType.Direct;
            //commandQueue = device.CreateCommandQueue(commandQueueDescription);
            //Log.Success("Create Command Queue");

            //Describe and create the swap chain.
            Log.Info("Create Swap Chain");
            SwapChainDescription swapChainDescription = new SwapChainDescription()
            {
                ModeDescription = backBufferDesc,
                SampleDescription = new SampleDescription(1, 0),
                Usage = Usage.RenderTargetOutput,
                BufferCount = 1,
                OutputHandle = window.Handle,
                IsWindowed = true,
                Flags = SwapChainFlags.AllowModeSwitch
            };

            swapChain = new SwapChain(factory, device, swapChainDescription);
            deviceContext = device.ImmediateContext;
            Log.Success("Create Swap Chain");

            renderTargetView = new D3D11.RenderTargetView(device, swapChain.GetBackBuffer<D3D11.Texture2D>(0));

            Log.Success("Create Device Resources");
        }
        private void InitializeTriangle()
        {
            triangleVertexBuffer = D3D11.Buffer.Create<RawVector3>(device, D3D11.BindFlags.VertexBuffer, vertices);
        }
        public void RenderCallback()
        {
            Draw();
        }
        private void Draw()
        {
            deviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            deviceContext.ClearRenderTargetView(renderTargetView, RGBAToRaw4(32, 103, 178));
            swapChain.Present(1, PresentFlags.None);
        }


        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            if (args.KeyCode == Keys.Escape)
            {
                window.Close();
            }
        }
        private RawColor4 ColorToRaw4(Color color)
        {
            const float n = 255f;
            return new RawColor4(color.R / n, color.G / n, color.B / n, color.A / n);
        }
        private RawColor4 RGBAToRaw4(byte r, byte g, byte b, byte a = 255)
        {
            const float n = 255f;
            return new RawColor4(r / n, g / n, b / n, a / n);
        }

        public void Dispose()
        {
            window?.Dispose();
            renderTargetView?.Dispose();
            swapChain?.Dispose();
            device?.Dispose();
            deviceContext?.Dispose();
            triangleVertexBuffer?.Dispose();
        }
    }
}
