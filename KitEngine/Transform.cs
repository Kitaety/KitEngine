using OpenTK.Mathematics;
using System;

namespace KitEngine
{
    public class Transform
    {
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Transform? Parent { get; set; }

        public Transform(Transform? parent) 
            : this(parent, Vector3.Zero, Quaternion.FromEulerAngles(Vector3.Zero))
        {}

        public Transform(Transform? parent, Vector3 position, Quaternion rotation)
        {
            Parent = parent;
            Position = position;
            Rotation = rotation;
        }

        public Matrix4 GetModelMatrix()
        {
            Vector3 parentPosition = Parent?.Position ?? Vector3.Zero;
            Matrix4 parentRotationMatrix = Matrix4.CreateFromQuaternion(Parent?.Rotation ?? Quaternion.FromEulerAngles(Vector3.Zero));


            Vector3 globalPosition = Vector3.TransformPosition(Position, parentRotationMatrix);
            Matrix4 rotationMatrix = Matrix4.CreateFromQuaternion(Rotation);
            Matrix4 translationMatrix = Matrix4.CreateTranslation(parentPosition + globalPosition);

            return rotationMatrix * translationMatrix;
        }
    }
}
