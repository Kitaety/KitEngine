using OpenTK.Mathematics;
using System;
using System.Drawing;

namespace KitEngine
{
    public class Camera
    {
        public bool IsOrthographic { get; set; }
        public Vector2i ViewSize { get; set; }
        public float DepthNear { get; set; }
        public float DepthFar { get; set; }
        public Vector3 Position { get; set; }
        public float AspectRatio => ViewSize.X / (float)ViewSize.Y;
        public Vector3 Front { get; private set; } = -Vector3.UnitZ;
        public Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, Front));
        public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateFrontVector();
            }
        }
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateFrontVector();
            }
        }
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 1f, 120f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }
        public Matrix4 ViewMatrix => Matrix4.LookAt(Position, Position + Front, Up);
        public Matrix4 ProjectionMatrix => IsOrthographic ? Matrix4.CreateOrthographic(ViewSize.X, ViewSize.Y, DepthNear, DepthFar)
            : Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, DepthNear, DepthFar);

        private float _pitch;
        private float _yaw = -MathHelper.PiOver2;
        private float _fov = MathHelper.PiOver2;

        public Camera(Vector3 position, Vector2i size, bool isOrthographic = false, float depthNear = 0.01f, float depthFar = 1000.0f, float fov = 45.0f)
        {
            DepthNear = depthNear;
            DepthFar = depthFar;
            Position = position;
            ViewSize = size;
            IsOrthographic = isOrthographic;
            Fov = fov;
        }

        private void UpdateFrontVector()
        {
            Vector3 newFront = Front;
            newFront.X = MathF.Cos(_pitch) * MathF.Cos(_yaw);
            newFront.Y = MathF.Sin(_pitch);
            newFront.Z = MathF.Cos(_pitch) * MathF.Sin(_yaw);

            Front = Vector3.Normalize(newFront);
        }
    }
}