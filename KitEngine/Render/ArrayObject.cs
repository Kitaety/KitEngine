using OpenTK.Graphics.OpenGL4;

namespace KitEngine.Render
{
    public class ArrayObject : IDisposable
    {
        private const int ErrorCode = -1;
        public int Id { private set; get; }
        public bool IsActive { private set; get; }
        private List<BufferObject> _bufferObjects = new List<BufferObject>();
        private List<int> _attribs = new List<int>();

        public ArrayObject()
        {
            Id = GL.GenVertexArray();

        }

        public void Activate()
        {
            IsActive = true;
            GL.BindVertexArray(Id);
        }

        public void Deactivate()
        {
            IsActive = false;
            GL.BindVertexArray(0);
        }

        public void AttachBuffer(BufferObject buffer)
        {
            if (!IsActive)
            {
                Activate();
            }

            buffer.Activate();
            _bufferObjects.Add(buffer);
        }

        public void AttribPointer(int index, int elementsPerVertex, VertexAttribPointerType type, int stride, int offset)
        {
            _attribs.Add(index);
            GL.EnableVertexAttribArray(index);
            GL.VertexAttribPointer(index, elementsPerVertex, type, false, stride, offset);
        }

        public void DisableAttribAll()
        {
            foreach (int attrib in _attribs)
            {
                GL.DisableVertexAttribArray(attrib);
            }
        }

        public void DrawElements(int start, int count, DrawElementsType elementType)
        {
            Activate();
            GL.DrawElements(PrimitiveType.Triangles, count, elementType, start);
        }

        public void Delete()
        {
            if (Id == ErrorCode)
            {
                return;
            }

            Deactivate();
            GL.DeleteVertexArray(Id);
            DeleteBufferAll();

            Id = ErrorCode;
        }

        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }

        private void DeleteBufferAll()
        {
            foreach (BufferObject buffer in _bufferObjects)
            {
                buffer.Dispose();
            }
        }
    }
}
