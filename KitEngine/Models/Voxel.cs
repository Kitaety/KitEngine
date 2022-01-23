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

        public Voxel(Vector3 position, SharpDX.Color color)
        {
            _position = position;
            Color = color;

            for (int i = 0; i < Vertices.Length; i++)
            {
                Vertices[i].Color = Color;
            }
        }
        
        public readonly VertexPositionColor[] Vertices = new VertexPositionColor[]
        {
            new VertexPositionColor(new Vector4(-1f, -1f, -1f, 1f), SharpDX.Color.White),//front
            new VertexPositionColor(new Vector4(-1f, 1f, -1f, 1f), SharpDX.Color.White),
            new VertexPositionColor(new Vector4(1f, 1f, -1f, 1f), SharpDX.Color.White),
            new VertexPositionColor(new Vector4(1f, -1f, -1f, 1f), SharpDX.Color.White),

            new VertexPositionColor(new Vector4(-1f, -1f, 1f, 1f), SharpDX.Color.White),//back
            new VertexPositionColor(new Vector4(1f, 1f, 1f, 1f), SharpDX.Color.White),
            new VertexPositionColor(new Vector4(-1f, 1f, 1f, 1f), SharpDX.Color.White),
            new VertexPositionColor(new Vector4(1f, -1f, 1f, 1f), SharpDX.Color.White),
        };

        public SharpDX.Color Color;

        public readonly int[] VertexIndices = new int[]
        {
            0,1,2, 0,2,3, //front
            4,5,6, 4,7,5, //back
            3,2,5, 3,5,7, //right
            0,6,1, 0,4,6, //left
            0,7,4, 0,3,7, //bottom
            1,6,5, 2,1,5, //top
        };

        private Vector3 _position;
    }
}
