using OpenTK.Mathematics;
using System;

namespace KitEngine.GameObjects
{
    public class Transform
    {
        public GameObject GameObject { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Transform? Parent { get; set; }

        public Vector3 GetGlobalPosition()
        {
            var parentGlobalPos = Parent?.GetGlobalPosition() ?? Vector3.Zero;
            var parentGlobalRot =  Matrix4.CreateFromQuaternion(Parent?.Rotation ?? Quaternion.FromEulerAngles(Vector3.Zero));
            var myGlobalPos = parentGlobalPos + Vector3.TransformPosition(Position, parentGlobalRot);

            return myGlobalPos;
        }

        public Quaternion GetGlobalRotation() =>
            Rotation * (Parent?.Rotation ?? Quaternion.FromEulerAngles(Vector3.Zero));

        public Vector3 Front => Rotation * -Vector3.UnitZ;
        public Vector3 Up => Vector3.Normalize(Vector3.Cross(Right, Front));
        public Vector3 Right => Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));

        public Transform(Transform? parent)
            : this(parent, Vector3.Zero, Quaternion.FromEulerAngles(Vector3.Zero))
        { }

        public Transform(Transform? parent, Vector3 position, Quaternion rotation)
        {
            Parent = parent;
            Position = position;
            Rotation = rotation;
        }

        public Matrix4 GetModelMatrix()
        {
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
            Matrix4 translationMatrix = Matrix4.CreateTranslation(GetGlobalPosition());

            return rotationMatrix * translationMatrix;
        }

        public void Rotate(Quaternion rotation)
        {
            Rotation *= rotation;
        }

        public void Translate(Vector3 newPosition)
        {
            Position = newPosition;
        }
    }
}
