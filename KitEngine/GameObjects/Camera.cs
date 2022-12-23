using OpenTK.Mathematics;

namespace KitEngine.GameObjects
{
    public class Camera: BaseObject
    {
        public bool IsOrthographic { get; set; }
        public Vector2i ViewSize { get; set; }
        public float DepthNear { get; set; }
        public float DepthFar { get; set; }
        public float AspectRatio => ViewSize.X / (float)ViewSize.Y;
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 120f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }
        public Matrix4 ViewMatrix => Matrix4.LookAt(Transform.Position, Transform.Position + Transform.Front, Transform.Up);
        public Matrix4 ProjectionMatrix => IsOrthographic ? Matrix4.CreateOrthographic(ViewSize.X, ViewSize.Y, DepthNear, DepthFar)
            : Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, DepthNear, DepthFar);
        private float _fov = MathHelper.PiOver2;

        public Camera(string name, Vector3 position, Vector2i size, bool isOrthographic = false, float depthNear = 0.01f, float depthFar = 1000.0f, float fov = 45.0f)
        :base(name, new Transform(null, position, Quaternion.FromEulerAngles(Vector3.Zero)))
        {
            DepthNear = depthNear;
            DepthFar = depthFar;
            ViewSize = size;
            IsOrthographic = isOrthographic;
            Fov = fov;
        }
    }
}