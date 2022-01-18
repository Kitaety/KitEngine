using System;
using System.Windows.Forms;
using KitEngine.Common;
using KitEngine.Models;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using Color = System.Drawing.Color;
using D3D11 = SharpDX.Direct3D11;
using ShaderBytecode = SharpDX.D3DCompiler.ShaderBytecode;
using Viewport = SharpDX.Viewport;

namespace KitEngine
{
    class Render: IDisposable
    {

        private RenderForm window;
        private RenderLoop.RenderCallback renderCallback;
        ModeDescription backBufferDesc;
        private D3D11.Device device;
        private SwapChain swapChain;

        private D3D11.DeviceContext deviceContext;
        //private D3D12.CommandQueue commandQueue;
        private D3D11.RenderTargetView renderTargetView;
        private VertexPositionColor[] vertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector3(-0.5f, -0.5f, 0.0f), SharpDX.Color.Red),
            new VertexPositionColor(new Vector3(0.0f, 0.5f, 0.0f), SharpDX.Color.Green),
            new VertexPositionColor(new Vector3(0.5f, -0.5f, 0.0f), SharpDX.Color.Blue)
        };
        private D3D11.Buffer triangleVertexBuffer;

        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;

        private D3D11.InputElement[] inputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, D3D11.InputClassification.PerVertexData, 0),
            new D3D11.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0, D3D11.InputClassification.PerVertexData, 0)
        };
        private ShaderSignature inputSignature;
        private D3D11.InputLayout inputLayout;
        private Viewport viewport;

        public Render(RenderForm renderForm, RenderLoop.RenderCallback renderCallback)
        {
            this.window = renderForm;
            this.renderCallback = renderCallback;
            
            backBufferDesc = new ModeDescription(window.Size.Width, window.Size.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
            
            InitializeDeviceResources();
            InitializeShaders();
            InitializeTriangle();
        }

        public void Run()
        {
            Log.Info("Run RenderLoop");
            RenderLoop.Run(window, RenderCallback);
        }

        public void RenderCallback()
        {
            renderCallback();
            Draw();
        }

        public void Dispose()
        {
            inputLayout?.Dispose();
            inputSignature?.Dispose();
            renderTargetView?.Dispose();
            swapChain?.Dispose();
            device?.Dispose();
            deviceContext?.Dispose();
            triangleVertexBuffer?.Dispose();
            pixelShader?.Dispose();
            vertexShader?.Dispose();
        }

        private void Draw()
        {
            deviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            deviceContext.ClearRenderTargetView(renderTargetView, RGBAToRaw4(32, 103, 178));

            deviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(triangleVertexBuffer, Utilities.SizeOf<VertexPositionColor>(), 0));
            deviceContext.Draw(vertices.Length, 0);

            swapChain.Present(1, PresentFlags.None);
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
            device = new D3D11.Device(adapter, D3D11.DeviceCreationFlags.None);
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

            // Set viewport
            Log.Info("Create ViewPort");
            viewport = new Viewport(0, 0, window.Size.Width, window.Size.Height);
            deviceContext.Rasterizer.SetViewport(viewport);
            Log.Success("Create ViewPort");

            Log.Success("Create Device Resources");
        }
        private void InitializeTriangle()
        {
            triangleVertexBuffer = D3D11.Buffer.Create<VertexPositionColor>(device, D3D11.BindFlags.VertexBuffer, vertices);
        }
        private void InitializeShaders()
        {
            using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile("Shaders/vertexShader.hlsl", "main", "vs_4_0", ShaderFlags.Debug))
            {
                inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
                vertexShader = new D3D11.VertexShader(device, vertexShaderByteCode);
            }
            using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile("Shaders/pixelShader.hlsl", "main", "ps_4_0", ShaderFlags.Debug))
            {
                pixelShader = new D3D11.PixelShader(device, pixelShaderByteCode);
            }

            // Set as current vertex and pixel shaders
            deviceContext.VertexShader.Set(vertexShader);
            deviceContext.PixelShader.Set(pixelShader);

            deviceContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;

            inputLayout = new D3D11.InputLayout(device, inputSignature, inputElements);
            deviceContext.InputAssembler.InputLayout = inputLayout;
        }
        
        private RawColor4 RGBAToRaw4(byte r, byte g, byte b, byte a = 255)
        {
            const float n = 255f;
            return new RawColor4(r / n, g / n, b / n, a / n);
        }
    }
}
