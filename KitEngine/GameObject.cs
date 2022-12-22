using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Mathematics;
using Quaternion = OpenTK.Mathematics.Quaternion;
using Vector3 = OpenTK.Mathematics.Vector3;

namespace KitEngine
{
    public sealed class GameObject:IDisposable
    {
        public string Name { get; set; }
        public Transform Transform { get; set; }
        public List<Voxel> Mesh { get; set; }
        
        public GameObject(string name, Transform? parent = null)
        :this(name, Vector3.Zero, Quaternion.FromEulerAngles(Vector3.Zero), parent)
        {}
        public GameObject(string name, Vector3 position, Quaternion rotation, Transform? parent = null)
        {
            Name = name;
            Mesh = new List<Voxel>();
            Transform = new Transform(parent, position, rotation);
        }

        public void Rotate(Vector3 rotation)
        {
            Rotate(Quaternion.FromEulerAngles(rotation));
        }

        public void Rotate(Quaternion rotation)
        {
            Transform.Rotation *= rotation;
            Mesh.ForEach(voxel => voxel.Transform.Rotation *= rotation);
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
