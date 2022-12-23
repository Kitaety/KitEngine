using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using KitEngine.GameObjects;
using KitEngine.Render;
using KitEngine.Input;

namespace KitEngine
{
    public class Game : GameWindow
    {
        public static Game Instance;

        private bool _isDebug = false;
        private double _frameTime = 0.0f;
        private float _fps = 0.0f;
        private float _deltaTime = 0.0f;

        public ShaderProgram ShaderProgram { private set; get; }
        private Camera _camera;

        public Game(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings, VSyncMode vSyncMode = VSyncMode.Off) : base(
            gameWindowSettings, nativeWindowSettings)
        {
            VSync = vSyncMode;
            CursorState = CursorState.Grabbed;
            Game.Instance = this;
        }

        private List<GameObject> GameObjects = new List<GameObject>();

        protected override void OnLoad()
        {
            base.OnLoad();
            GL.ClearColor(Color4.Black);
            GL.Enable(EnableCap.DepthTest);

            GL.Enable(EnableCap.CullFace);
            GL.CullFace(CullFaceMode.Back);
            ShaderProgram = new ShaderProgram(@"Shaders\base_shader.vert", @"Shaders\base_shader.frag");
            _camera = new Camera("Camera", Vector3.Zero, Size);
            CreateTestObjects();
        }

        protected override void OnUnload()
        {
            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Dispose();
            }
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

        private void Draw()
        {
            ShaderProgram.ActivateProgram();
            Matrix4 viewMatrix = _camera.ViewMatrix;
            Matrix4 projection = _camera.ProjectionMatrix;

            ShaderProgram.SetUniform(ShaderProgramUniforms.View, viewMatrix);
            ShaderProgram.SetUniform(ShaderProgramUniforms.Projection, projection);

            foreach (GameObject gameObject in GameObjects)
            {
                gameObject.Render();
            }

            ShaderProgram.DeactivateProgram();
        }

        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);
            _camera.ViewSize = Size;
            GL.Viewport(0, 0, e.Width, e.Height);
        }

        private void CalculateFps(double deltaTime)
        {
            _deltaTime = (float)deltaTime;
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
            InputManager.Update();

            switch (InputManager.GetKeyDown())
            {
                case Keys.Escape:
                    Close();
                    break;
                case Keys.F1:
                    ActivateDebugMode(!_isDebug);
                    break;
                case Keys.F2:
                    _camera.IsOrthographic = !_camera.IsOrthographic;
                    break;
            }

            UpdateCamera();
            UpdateRotationObject();
        }

        private void UpdateRotationObject()
        {

            var rot = Vector3.Zero;
            var speedRot = _deltaTime * 5f;

            switch (InputManager.GetKeyDown())
            {
                case Keys.Q:
                    rot.Y = speedRot;
                    break;
                case Keys.E:
                    rot.Y -= speedRot;
                    break;
                case Keys.R:
                    rot.X += speedRot;
                    break;
                case Keys.T:
                    rot.X -= speedRot;
                    break;
                case Keys.Y:
                    rot.Z += speedRot;
                    break;
                case Keys.U:
                    rot.Z -= speedRot;
                    break;
            }

            GameObjects[2].Rotate(rot);
        }

        private void ActivateDebugMode(bool isDebug)
        {
            _isDebug = isDebug;
            if (isDebug)
            {
                CursorState = CursorState.Normal;
            }
            else
            {
                CursorState = CursorState.Grabbed;
            }
        }

        private void UpdateCamera()
        {
            const float sensitivity = 0.025f;
            const float cameraSpeed = 2f;

            Vector3 camPos = _camera.Transform.Position;

            switch (InputManager.GetKeyDown())
            {
                case Keys.W:
                    camPos += _camera.Transform.Front * cameraSpeed * _deltaTime;
                    break;
                case Keys.S:
                    camPos -= _camera.Transform.Front * cameraSpeed * _deltaTime;
                    break;
                case Keys.A:
                    camPos -= _camera.Transform.Right * cameraSpeed * _deltaTime;
                    break;
                case Keys.D:
                    camPos += _camera.Transform.Right * cameraSpeed * _deltaTime;
                    break;
                case Keys.Space:
                    camPos += _camera.Transform.Up * cameraSpeed * _deltaTime;
                    break;
                case Keys.LeftShift:
                    camPos -= _camera.Transform.Up * cameraSpeed * _deltaTime;
                    break;
            }

            _camera.Transform.Translate(camPos);

            Vector2 mouse = InputManager.DeltaMousePosition;

            var mouseX = -mouse.X * sensitivity;
            var mouseY = -mouse.Y * sensitivity;

            _camera.Rotate(new Vector3(mouseY, mouseX, 0));
        }

        private void CreateTestObjects()
        {
            var obj = new GameObject("test", new Vector3(0f, 0, -6), Vector3.Zero);
            List<Voxel> voxels = new List<Voxel>
            {
                new Voxel(new Vector3(0.0f, 0.0f, 0.0f), new[] { 0.0f, 1.0f, 0.0f, 1.0f }, obj.Transform),
                new Voxel(new Vector3(1.0f, 0.0f, 0.0f), new[] { 1.0f, 0.0f, 0.0f, 1.0f }, obj.Transform),
                new Voxel(new Vector3(2.0f, 0.0f, 0.0f), new[] { 0.0f, 0.0f, 1.0f, 1.0f }, obj.Transform),
                new Voxel(new Vector3(2.0f, 0.0f, 1.0f), new[] { 0.0f, 0.0f, 1.0f, 1.0f }, obj.Transform)
            };
            obj.Mesh = voxels;

            var obj1 = new GameObject("test1", new Vector3(0f, 0, -4), Vector3.Zero);
            List<Voxel> voxels1 = new List<Voxel>
            {
                new Voxel(new Vector3(0.0f, 0.0f, 0.0f), new[] { 1.0f, 0.0f, 1.0f, 1.0f }, obj1.Transform),
                new Voxel(new Vector3(1.0f, 0.0f, 1.0f), new[] { 1.0f, 1.0f, 0.0f, 1.0f }, obj1.Transform),
                new Voxel(new Vector3(2.0f, 0.0f, 2.0f), new[] { 1.0f, 1.0f, 1.0f, 1.0f }, obj1.Transform)
            };
            obj1.Mesh = voxels1;

            var obj2 = new GameObject("test2", new Vector3(-1f, 0f, -2f), (0f, 1f, 0f));
            obj2.Mesh = new List<Voxel>
                { new Voxel(new Vector3(0.0f, 0.0f, 0.0f), new[] { 1f, 0f, 0f, 1.0f }, obj2.Transform) };
            
            var obj2_1 = new GameObject("test2_1", new Vector3(1f, 0f, 0f), new Vector3(0, 0.5f, 0), obj2.Transform);
            obj2_1.Mesh = new List<Voxel>
                { new Voxel(new Vector3(0.0f, 0.0f, 0.0f), new[] { 0f, 1f, 0f, 1.0f }, obj2_1.Transform) };
            
            //var obj2_2 = new GameObject("test2_2", new Vector3(1f, 1f, 0f), new Vector3(0, 15, 0));
            //obj2_2.Mesh = new List<Voxel>
            //    { new Voxel(new Vector3(0.0f, 0.0f, 0.0f), new[] { 0f, 0f, 1f, 1.0f }, obj2_2.Transform) };

            // obj2.AddChild(obj2_1);
            
            //obj2.AddChild(obj2_2.Copy());
            //obj2_1.AddChild(obj2_2);

            GameObjects.Add(obj);
            GameObjects.Add(obj1);
            GameObjects.Add(obj2);
        }
    }
}
