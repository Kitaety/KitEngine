using System;
using System.Diagnostics;
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

        private ModeDescription backBufferDesc;
        private SwapChainDescription swapChainDescription;
        private D3D11.Device device;
        private SwapChain swapChain;
        private Factory factory;
        private D3D11.DeviceContext deviceContext;
        private D3D11.RenderTargetView renderTargetView;

        private VertexPositionColor[] vertices;
        private int[] indices;

        private Matrix viewProj;
        private D3D11.Buffer vertexBuffer;
        private D3D11.Buffer indexBuffer;
        private D3D11.Buffer constantBuffer;
        private D3D11.VertexShader vertexShader;
        private D3D11.PixelShader pixelShader;

        private D3D11.InputElement[] inputElements = new D3D11.InputElement[]
        {
            new D3D11.InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0, D3D11.InputClassification.PerVertexData, 0),
            new D3D11.InputElement("COLOR", 0, Format.R32G32B32A32_Float, 16, 0, D3D11.InputClassification.PerVertexData, 0),
        };

        private ShaderSignature inputSignature;
        private D3D11.InputLayout inputLayout;
        private Viewport viewport;

        bool userResized = true;
        D3D11.Texture2D backBuffer = null;
        D3D11.Texture2D depthBuffer = null;
        D3D11.DepthStencilView depthView = null;

        Matrix view = Matrix.LookAtLH(new Vector3(0, 0, 0), new Vector3(0, 0, 0), Vector3.UnitY);
        Matrix proj = Matrix.Identity;

        private Stopwatch clock = new Stopwatch();
        public Render(RenderForm renderForm, RenderLoop.RenderCallback renderCallback)
        {
            this.window = renderForm;
            this.renderCallback = renderCallback;

            InitializeDeviceResources();
            InitializeBuffers();
            InitializeShaders();

            // Prepare matrices
            view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            proj = Matrix.Identity;

            // Setup handler on resize form
            window.UserResized += (sender, args) => userResized = true;

            clock.Start();
        }

        public void Run()
        {
            Log.Info("Run RenderLoop");
            RenderLoop.Run(window, RenderCallback);
        }

        public void RenderCallback()
        {
            renderCallback();

            if (userResized)
            {
                OnResizeWindows();
            }

            Draw();
        }

        private void OnResizeWindows()
        {
            // Dispose all previous allocated resources
            Utilities.Dispose(ref backBuffer);
            Utilities.Dispose(ref renderTargetView);
            Utilities.Dispose(ref depthBuffer);
            Utilities.Dispose(ref depthView);

            // Resize the backbuffer
            swapChain.ResizeBuffers(swapChainDescription.BufferCount, window.ClientSize.Width, window.ClientSize.Height, Format.Unknown, SwapChainFlags.None);

            // Get the backbuffer from the swapchain
            backBuffer = D3D11.Texture2D.FromSwapChain<D3D11.Texture2D>(swapChain, 0);

            // Renderview on the backbuffer
            renderTargetView = new D3D11.RenderTargetView(device, backBuffer);

            // Create the depth buffer
            depthBuffer = new D3D11.Texture2D(device, new D3D11.Texture2DDescription()
            {
                Format = Format.D32_Float_S8X24_UInt,
                ArraySize = 1,
                MipLevels = 1,
                Width = window.ClientSize.Width,
                Height = window.ClientSize.Height,
                SampleDescription = new SampleDescription(1, 0),
                Usage = D3D11.ResourceUsage.Default,
                BindFlags = D3D11.BindFlags.DepthStencil,
                CpuAccessFlags = D3D11.CpuAccessFlags.None,
                OptionFlags = D3D11.ResourceOptionFlags.None
            });

            // Create the depth buffer view
            depthView = new D3D11.DepthStencilView(device, depthBuffer);

            // Setup targets and viewport for rendering
            deviceContext.Rasterizer.SetViewport(new Viewport(0, 0, window.ClientSize.Width, window.ClientSize.Height, 0.0f, 1.0f));
            deviceContext.OutputMerger.SetTargets(depthView, renderTargetView);

            // Setup new projection matrix with correct aspect ratio
            proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, window.ClientSize.Width / (float)window.ClientSize.Height, 0.1f, 100.0f);

            // We are done resizing
            userResized = false;
        }

        public void Dispose()
        {
            clock.Stop();
            inputSignature?.Dispose();
            vertexShader?.Dispose();
            pixelShader?.Dispose();
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose(); 
            inputLayout?.Dispose();
            constantBuffer.Dispose();
            depthBuffer.Dispose();
            depthView.Dispose();
            renderTargetView?.Dispose();
            backBuffer.Dispose();
            swapChain?.Dispose();
            deviceContext?.ClearState();
            deviceContext?.Flush();
            device?.Dispose();
            deviceContext?.Dispose();
            swapChain?.Dispose();
            factory?.Dispose();
        }

        private void Draw()
        {
            var time = clock.ElapsedMilliseconds / 1000.0f;
            viewProj = Matrix.Multiply(view, proj);

            //// Clear views
            deviceContext.ClearDepthStencilView(depthView, D3D11.DepthStencilClearFlags.Depth, 1.0f, 0);
            deviceContext.ClearRenderTargetView(renderTargetView, Color.Black);

            deviceContext.InputAssembler.SetVertexBuffers(0, new D3D11.VertexBufferBinding(vertexBuffer, Utilities.SizeOf<VertexPositionColor>(), 0));
            deviceContext.VertexShader.SetConstantBuffers(0, constantBuffer);
            deviceContext.InputAssembler.SetIndexBuffer(indexBuffer, Format.R32_UInt, 0);

            // Update WorldViewProj Matrix
            var worldViewProj = Matrix.RotationX(time) * Matrix.RotationY(time * 2) * Matrix.RotationZ(time * .7f) * viewProj;
            //var worldViewProj = Matrix.RotationX(0.002f) * viewProj;
            worldViewProj.Transpose();
            deviceContext.UpdateSubresource(ref worldViewProj, constantBuffer);

            if(indices != null)
            {
                deviceContext.DrawIndexed(indices.Length, 0, 0);
            }
            swapChain.Present(1, PresentFlags.None);
        }

        private void InitializeDeviceResources()
        {
            Log.Info("Create Device Resources");

            backBufferDesc = new ModeDescription(window.ClientSize.Width, window.ClientSize.Height, new Rational(60, 1), Format.R8G8B8A8_UNorm);
            swapChainDescription = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = backBufferDesc,
                IsWindowed = true,
                OutputHandle = window.Handle,
                SampleDescription = new SampleDescription(1, 0),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            D3D11.Device.CreateWithSwapChain(DriverType.Hardware, D3D11.DeviceCreationFlags.None, swapChainDescription, out device, out swapChain);
            deviceContext = device.ImmediateContext;
            factory = swapChain.GetParent<Factory>();

            // Set viewport
            viewport = new Viewport(0, 0, window.Size.Width, window.Size.Height);
            deviceContext.Rasterizer.SetViewport(viewport);
            Log.Success("Create Device Resources");
        }
        private void InitializeBuffers()
        {

            constantBuffer = new D3D11.Buffer(device, Utilities.SizeOf<Matrix>(), D3D11.ResourceUsage.Default, D3D11.BindFlags.ConstantBuffer,
                D3D11.CpuAccessFlags.None, D3D11.ResourceOptionFlags.None, 0);
            UpdateVertexBuffer();
            UpdateIndexBuffer();
            deviceContext.VertexShader.SetConstantBuffer(0, constantBuffer);
        }
        private void InitializeShaders()
        {
            using (var vertexShaderByteCode = ShaderBytecode.CompileFromFile(Path.ShadersFolderPath + "MiniCube.fx", "VS", "vs_4_0", ShaderFlags.Debug))
            {
                inputSignature = ShaderSignature.GetInputSignature(vertexShaderByteCode);
                vertexShader = new D3D11.VertexShader(device, vertexShaderByteCode);
            }
            using (var pixelShaderByteCode = ShaderBytecode.CompileFromFile(Path.ShadersFolderPath + "MiniCube.fx", "PS", "ps_4_0", ShaderFlags.Debug))
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

        public void SetVertecies(VertexPositionColor[] voxelVertices)
        {
            vertices = voxelVertices;
            UpdateVertexBuffer();
        }

        public void SetIndeces(int[] voxelVertexIndices)
        {
            indices = voxelVertexIndices;
            UpdateIndexBuffer();
        }

        private void UpdateVertexBuffer()
        {
            if(vertices != null && vertices.Length > 0)
            {
                vertexBuffer?.Dispose();
                vertexBuffer = D3D11.Buffer.Create<VertexPositionColor>(device, D3D11.BindFlags.VertexBuffer, vertices);
            }
        }

        private void UpdateIndexBuffer()
        {
            if (indices != null && indices.Length > 0)
            {
                indexBuffer?.Dispose();
                indexBuffer = D3D11.Buffer.Create<int>(device, D3D11.BindFlags.IndexBuffer, indices);
            }
        }
    }
}
