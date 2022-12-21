using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace KitEngine
{
    internal class Voxel:IDisposable
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

        public Vector3 Position;
        public float[] Color { get; set; }
        private readonly ArrayObject _vertexArrayObject;

        private Matrix4 GetTransformMatrix(Vector3 parentPosition, Vector3 parentRotation)
        {
            Matrix4 mat4 = Matrix4.CreateRotationX(MathHelper.DegreesToRadians(parentRotation.X)) * Matrix4.CreateRotationY(MathHelper.DegreesToRadians(parentRotation.Y)) *
                           Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(parentRotation.Z));

            return mat4 * Matrix4.CreateTranslation(parentPosition + Vector3.TransformPosition(Position, mat4));
        }
        public Voxel(Vector3 position, float[] color)
        {
            Position = position;
            Color = color;
            _vertexArrayObject = CreateVAO(GetArrayVertexColor(), Indexes);
        }

        public void Render(Vector3 parentPosition, Vector3 parentRotation)
        {
            Game.Instance.ShaderProgram.SetUniform(ShaderProgramUniforms.Model, GetTransformMatrix(parentPosition, parentRotation));

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
