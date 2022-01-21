using System;
using System.Windows.Forms;
using KitEngine.Common;
using KitEngine.Common.Constants;
using KitEngine.Models;
using SharpDX;
using SharpDX.D3DCompiler;
using SharpDX.Direct3D;
using SharpDX.DXGI;
using SharpDX.Mathematics.Interop;
using SharpDX.Windows;
using D3D11 = SharpDX.Direct3D11;
using Viewport = SharpDX.Viewport;

namespace KitEngine.RenderSystem
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
            new VertexPositionColor(new Vector3(-1f, -1f, 5f), SharpDX.Color.Red),
            new VertexPositionColor(new Vector3(1f, 1f, 5f), SharpDX.Color.Green),
            new VertexPositionColor(new Vector3(1f, -1f, 5f), SharpDX.Color.Blue),
            new VertexPositionColor(new Vector3(-1f, 1f, 5f), SharpDX.Color.Yellow),

            new VertexPositionColor(new Vector3(-1f, -1f, 6f), SharpDX.Color.Red),
            new VertexPositionColor(new Vector3(1f, 1f, 6f), SharpDX.Color.Green),
            new VertexPositionColor(new Vector3(1f, -1f, 6f), SharpDX.Color.Blue),
            new VertexPositionColor(new Vector3(-1f, 1f, 6f), SharpDX.Color.Yellow),
        };
        private D3D11.Buffer vertexBuffer;
        private D3D11.Buffer indexBuffer;
        private D3D11.Buffer constantBuffer;

        private int[] indices = new int[]
        {
            0,1,2, 0,3,1, //front
            4,5,6, 4,7,5, //back
            3,5,1, 3,7,5, //top
            0,6,2, 0,4,6, //bottom
            0,7,4, 0,3,7, //left
            2,5,6, 2,1,5, //right
        };

        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;

        private D3D11.InputElement[] inputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, D3D11.InputClassification.PerVertexData, 0),
            new D3D11.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 12, 0, D3D11.InputClassification.PerVertexData, 0),
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
            InitializeBuffers();
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
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            pixelShader?.Dispose();
            vertexShader?.Dispose();
        }

        private void Draw()
        {
            Matrix view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            Matrix proj = Matrix.Identity;

            deviceContext.OutputMerger.SetRenderTargets(renderTargetView);
            deviceContext.ClearRenderTargetView(renderTargetView, RGBAToRaw4(32, 103, 178));

            deviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<VertexPositionColor>(), 0));
            deviceContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);
            deviceContext.DrawIndexed(indices.Length, 0,0);

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
        private void InitializeBuffers()
        {
            constantBuffer = new D3D11.Buffer(device, Utilities.SizeOf<Matrix>(), D3D11.ResourceUsage.Default, D3D11.BindFlags.ConstantBuffer,
                D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
            vertexBuffer = D3D11.Buffer.Create<VertexPositionColor>(device, D3D11.BindFlags.VertexBuffer, vertices);
            indexBuffer = D3D11.Buffer.Create<int>(device, D3D11.BindFlags.IndexBuffer, indices);
        }
        private void InitializeShaders()
        {
            using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile(Path.ShadersFolderPath + "cube.fx", "VS", "vs_4_0", ShaderFlags.Debug))
            {
                inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
                vertexShader = new D3D11.VertexShader(device, vertexShaderByteCode);
            }
            using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile(Path.ShadersFolderPath + "cube.fx", "PS", "ps_4_0", ShaderFlags.Debug))
            {
                pixelShader = new D3D11.PixelShader(device, pixelShaderByteCode);
            }

            // Set as current vertex and pixel shaders
            deviceContext.VertexShader.SetConstantBuffer(0, constantBuffer);
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
