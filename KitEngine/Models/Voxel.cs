using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;

namespace KitEngine.Models
{
    public class Voxel
    {
        public Transform Transform;

        public Vector3[] Mesh = new Vector3[]
        {
            new Vector3(-1f, -1f, 0.5f),
            new Vector3(1f, 1f, 0.5f),
            new Vector3(1f, -1f, 0.5f),
            new Vector3(-1f, 1f, 0.5f),
            new Vector3(-1f, -1f, -1f),
            new Vector3(1f, 1f, -0.5f),
            new Vector3(1f, -1f, -0.5f),
            new Vector3(-1f, 1f, -0.5f)
        };

        public readonly int[] Indices = new int[]
        {
            0, 1, 2, 0, 3, 1,
        };

        public Color Color = Color.Red;
    }
}
