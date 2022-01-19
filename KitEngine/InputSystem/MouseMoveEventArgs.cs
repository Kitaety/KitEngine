using System.Runtime.InteropServices;

namespace KitEngine.InputSystem
{
    [StructLayout(LayoutKind.Sequential)]
    public class MouseMoveEventArgs
    {
        public readonly float Vertical;
        public readonly float Horizontal;

        public MouseMoveEventArgs(float vertical, float horizontal)
        {
            Vertical = vertical;
            Horizontal = horizontal;
        }
    }
}
