using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace KitEngine
{
    internal class Voxel:IDisposable
    {
        public Vector3 Position;
        public float[] Vertexes { get; } = new float[]
        {
            -0.5f, 0.5f, 0.5f, 1.0f,
            -0.5f, -0.5f, 0.5f, 1.0f,
            0.5f, -0.5f, 0.5f, 1.0f,
            0.5f, 0.5f, 0.5f, 1.0f,
            -0.5f, 0.5f, -0.5f, 1.0f,
            -0.5f, -0.5f, -0.5f, 1.0f,
            0.5f, -0.5f, -0.5f, 1.0f,
            0.5f, 0.5f, -0.5f, 1.0f,
        };
        public float[] Color { get; set; }

        private uint[] _indexes { get; } = new uint[]
        {
            //front
            0, 1, 2,
            0, 2, 3,

            //up
            0, 3, 4,
            3, 7, 4,

            //down
            1, 5, 2,
            5, 6, 2,

            //right
            3, 2, 7,
            2, 6, 7,

            //left
            0, 4, 1,
            4, 5, 1,

            //back
            4, 6, 5,
            4, 7, 6,
        };

        private ArrayObject _vao;

        public Voxel(Vector3 position, float[] color)
        {
            Position = position;
            Color = color;
            _vao = CreateVAO(GetArrayVertexColor(), _indexes);
        }

        public void Render(ref Matrix4 projection, ref Matrix4 view)
        {
            Matrix4 scale = Matrix4.CreateScale(1f, 1f, 1f);
            Matrix4.CreateTranslation(Position, out Matrix4 position);
            Matrix4 trans = scale * position;
            
            GL.UniformMatrix4(20,
                true,
                ref trans);
            GL.UniformMatrix4(21,
                true,
                ref view);
            GL.UniformMatrix4(22,
                true,
                ref projection);
            _vao.Activate();
            _vao.DrawElements(0, _indexes.Length, DrawElementsType.UnsignedInt);
        }
        
        public void Dispose()
        {
            _vao.Dispose();
        }

        private float[] GetArrayVertexColor()
        {
            List<float> result = new List<float>();

            for (int i = 0; i < Vertexes.Length; i+=4)
            {
                result.Add(Vertexes[i]);
                result.Add(Vertexes[i + 1]);
                result.Add(Vertexes[i + 2]);
                result.Add(Vertexes[i + 3]);
                result.AddRange(Color);
            }

            return result.ToArray();
        }

        private ArrayObject CreateVAO(float[] vertices, uint[] indexes)
        {
            int vertexArray = Game.Instance.ShaderProgram.GetAttribProgram("aPosition");
            int colorArray = Game.Instance.ShaderProgram.GetAttribProgram("aColor");

            BufferObject vbo = new BufferObject(BufferTarget.ArrayBuffer);
            vbo.SetData(BufferUsageHint.StaticDraw, vertices);
            BufferObject ebo = new BufferObject(BufferTarget.ElementArrayBuffer);
            ebo.SetData(BufferUsageHint.StaticDraw, indexes);

            ArrayObject vao = new ArrayObject();
            vao.Activate();

            vao.AttachBuffer(ebo);
            vao.AttachBuffer(vbo);

            vao.AttribPointer(vertexArray, 4, VertexAttribPointerType.Float, 8 * sizeof(float), 0);
            vao.AttribPointer(colorArray, 4, VertexAttribPointerType.Float, 8 * sizeof(float), 4 * sizeof(float));

            vao.Deactivate();
            vao.DisableAttribAll();

            return vao;
        }
    }
}
