using System;
using System.Collections.Generic;
using System.Windows.Forms;
using SharpDX.Multimedia;
using SharpDX.RawInput;
using Device = SharpDX.RawInput.Device;
using DeviceFlags = SharpDX.RawInput.DeviceFlags;
namespace KitEngine.InputSystem
{
    public class Input: IDisposable
    {
        private static Input _instance;
        public static Input Instance => _instance ?? new Input();
        private readonly List<Keys> _keysPressed = new List<Keys>();

        public Input()
        {
            // setup the device
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None);
            Device.MouseInput += (sender, args) => OnMouseInput(args);

            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None);
            Device.KeyboardInput += (sender, args) => OnKeyboardInput(args);

            _instance = this;
        }

        public bool IsPressed(Keys key)
        {
            return _keysPressed.Contains(key);
        }

        private void OnMouseInput(MouseInputEventArgs args)
        {
            Keys key = MouseButtonToKeys(args.ButtonFlags);

            switch (args.ButtonFlags)
            {
                case MouseButtonFlags.LeftButtonDown:
                case MouseButtonFlags.RightButtonDown:
                case MouseButtonFlags.MiddleButtonDown:
                case MouseButtonFlags.Button4Down:
                case MouseButtonFlags.Button5Down:
                {
                    _keysPressed.Add(key);
                    MouseKeyDown?.Invoke(key);
                    break;
                }
                case MouseButtonFlags.LeftButtonUp:
                case MouseButtonFlags.RightButtonUp:
                case MouseButtonFlags.MiddleButtonUp:
                case MouseButtonFlags.Button4Up:
                case MouseButtonFlags.Button5Up:
                {
                    _keysPressed.Remove(key);
                    MouseKeyUp?.Invoke(key);
                    break;
                }

                case MouseButtonFlags.None:
                {
                    MouseMove?.Invoke(new MouseMoveEventArgs(args.Y, args.X));
                    break;
                }
            }
        }
        
        private void OnKeyboardInput(KeyboardInputEventArgs args)
        {
            switch (args.State)
            {
                case KeyState.KeyDown:
                {
                    if (!IsPressed(args.Key))
                    {
                        _keysPressed.Add(args.Key);
                        KeyDown?.Invoke(args.Key);
                    }
                    break;
                }

                case KeyState.KeyUp:
                {
                    _keysPressed.Remove(args.Key);
                    KeyUp?.Invoke(args.Key);
                    break;
                }
            }
        }

        private Keys MouseButtonToKeys(MouseButtonFlags mouseButtonFlags)
        {
            Keys key = Keys.None;
            switch (mouseButtonFlags)
            {
                case MouseButtonFlags.LeftButtonDown:
                case MouseButtonFlags.LeftButtonUp:
                {
                    key = Keys.LButton;
                    break;
                }

                case MouseButtonFlags.RightButtonDown:
                case MouseButtonFlags.RightButtonUp:
                {
                    key = Keys.RButton;
                    break;
                }

                case MouseButtonFlags.MiddleButtonDown:
                case MouseButtonFlags.MiddleButtonUp:
                {
                    key = Keys.MButton;
                    break;
                }

                case MouseButtonFlags.Button4Down:
                case MouseButtonFlags.Button4Up:
                {
                    key = Keys.XButton1;
                    break;
                }

                case MouseButtonFlags.Button5Down:
                case MouseButtonFlags.Button5Up:
                {
                    {
                        key = Keys.XButton2;
                        break;
                    }
                }
            }

            return key;
        }

        public delegate void KeyInputHandler(Keys key);
        public event KeyInputHandler KeyUp;
        public event KeyInputHandler KeyDown;

        public delegate void MouseMoveInputHandler(MouseMoveEventArgs args);
        public delegate void MouseKeyInputHandler(Keys key);

        public event MouseKeyInputHandler MouseKeyUp;
        public event MouseKeyInputHandler MouseKeyDown;
        public event MouseMoveInputHandler MouseMove;

        public void Dispose()
        {
        }
    }
}
