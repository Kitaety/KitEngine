using KitEngine.Render;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace KitEngine.GameObjects
{
    public class Voxel: IDisposable
    {
        private static readonly uint[] Indexes = {
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
        public static readonly float[] Vertexes = {
            -0.5f, 0.5f, 0.5f, 1.0f,
            -0.5f, -0.5f, 0.5f, 1.0f,
            0.5f, -0.5f, 0.5f, 1.0f,
            0.5f, 0.5f, 0.5f, 1.0f,
            -0.5f, 0.5f, -0.5f, 1.0f,
            -0.5f, -0.5f, -0.5f, 1.0f,
            0.5f, -0.5f, -0.5f, 1.0f,
            0.5f, 0.5f, -0.5f, 1.0f,
        };
        public Transform Transform { get; set; }
        public float[] Color { get; set; }
        private readonly ArrayObject _vertexArrayObject;

        public Voxel(Vector3 position, float[] color, Transform parent)
        {
            Transform = new Transform(parent, position, parent.Rotation);
            Color = color;
            _vertexArrayObject = CreateVAO(GetArrayVertexColor(), Indexes);
        }

        public void Render()
        {
            Game.Instance.ShaderProgram.SetUniform(ShaderProgramUniforms.Model, Transform.GetModelMatrix());

            _vertexArrayObject.Activate();
            _vertexArrayObject.DrawElements(0, Indexes.Length, DrawElementsType.UnsignedInt);
        }

        public void Dispose()
        {
            _vertexArrayObject.Dispose();
        }

        private float[] GetArrayVertexColor()
        {
            List<float> result = new List<float>();

            for (int i = 0; i < Vertexes.Length; i += 4)
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
            int vertexArray = Game.Instance.ShaderProgram.GetAttributeProgram(ShaderProgramAttributes.Position);
            int colorArray = Game.Instance.ShaderProgram.GetAttributeProgram(ShaderProgramAttributes.Color);

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
