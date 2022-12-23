using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using KitEngine.Render;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace KitEngine.GameObjects
{
    public sealed class GameObject : BaseObject, IDisposable
    {
        public List<Voxel> Mesh { get; set; }

        public GameObject(string name, Transform? parent = null)
        : this(name, Vector3.Zero, Quaternion.FromEulerAngles(Vector3.Zero), parent)
        { }
        public GameObject(string name, Vector3 position, Quaternion rotation, Transform? parent = null) 
            : this(name, new Transform(parent, position, rotation), parent)
        {}

        public GameObject(string name, Transform transform, Transform? parent = null)
            : base(name, transform)
        {
            transform.Parent = parent;
            Mesh = new List<Voxel>();
        }

        public override void Rotate(Quaternion rotation)
        {
            Transform.Rotate(rotation);
            Mesh.ForEach(voxel => voxel.Transform.Rotate(rotation));
        }

        public void Dispose()
        {
            foreach (Voxel voxel in Mesh)
            {
                voxel.Dispose();
            }
        }

        public void Render()
        {
            foreach (Voxel voxel in Mesh)
            {
                voxel.Render();
            }
        }

    }
}
