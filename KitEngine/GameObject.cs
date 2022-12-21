using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;

namespace KitEngine
{
    internal class GameObject:IDisposable
    {
        public string Name { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 Rotation { get; set; }
        public List<Voxel> Mesh { get; set; }

        private Matrix4 _rotationMatrix => Matrix4.CreateRotationX(MathHelper.DegreesToRadians(Rotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(Rotation.Y)) *
                                           Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(Rotation.Z));
        public GameObject(string name)
        {
            Name = name;
            Mesh = new List<Voxel>();
            Position = Vector3.Zero;
        }

        public void Dispose()
        {
            foreach (var voxel in Mesh)
            {
                voxel.Dispose();
            }
        }

        public void Render()
        {
            foreach (var voxel in Mesh)
            {
                voxel.Render(Position);
            }
        }
    }
}
