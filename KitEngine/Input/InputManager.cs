using OpenTK.Mathematics;
using OpenTK.Windowing.GraphicsLibraryFramework;
namespace KitEngine.Input
{
    public static class InputManager
    {
        public static KeyboardState KeyboardState => Game.Instance.KeyboardState;
        public static MouseState MouseState => Game.Instance.MouseState;
        public static Vector2 MousePosition => new(MouseState.X, MouseState.Y);
        public static Vector2 DeltaMousePosition = Vector2.Zero;
        private static Vector2 _lastMousePosition = MousePosition;

        public static Keys? GetKeyDown()
        {
            foreach (Keys key in Enum.GetValues(typeof(Keys)))
            {
                if (key != Keys.Unknown && KeyboardState.IsKeyDown(key))
                {
                    return key;
                }
            }

            return Keys.Unknown;
        }

        public static void Update()
        {
            DeltaMousePosition = MousePosition - _lastMousePosition;
            _lastMousePosition = MousePosition;
        }
    }
}
