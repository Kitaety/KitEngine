using OpenTK.Graphics.OpenGL4;
using System.Runtime.InteropServices;

namespace KitEngine.Render
{
    public sealed class BufferObject : IDisposable
    {
        private const int ErrorCode = -1;
        public int Id { private set; get; }
        public bool IsActive { private set; get; }

        private readonly BufferTarget _type;
        public BufferObject(BufferTarget type)
        {
            _type = type;
            Id = GL.GenBuffer();
        }

        public void Activate()
        {
            IsActive = true;
            GL.BindBuffer(_type, Id);
        }

        public void Deactivate()
        {
            IsActive = false;
            GL.BindBuffer(_type, 0);
        }

        public void SetData<T>(BufferUsageHint bufferUsageHint, T[] data) where T : struct
        {
            if (data.Length == 0)
            {
                throw new ArgumentException("Array should be not empty");
            }

            Activate();
            GL.BufferData(_type, data.Length * Marshal.SizeOf<T>(), data, bufferUsageHint);
        }

        public void Delete()
        {
            if (Id == ErrorCode)
            {
                return;
            }

            Deactivate();
            GL.DeleteBuffer(Id);

            Id = ErrorCode;
        }

        public void Dispose()
        {
            Delete();
            GC.SuppressFinalize(this);
        }
    }
}
