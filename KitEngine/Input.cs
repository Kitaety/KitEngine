using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KitEngine.Common;
using SharpDX.Multimedia;
using SharpDX.RawInput;
using Device = SharpDX.RawInput.Device;
using DeviceFlags = SharpDX.RawInput.DeviceFlags;

namespace KitEngine
{
    class Input: IDisposable
    {
        private List<Keys> _keysPressed = new List<Keys>();

        public Input()
        {
            // setup the device
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None);
            Device.MouseInput += (sender, args) => OnMouseInput(args);

            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None);
            Device.KeyboardInput += (sender, args) => OnKeyboardInput(args);
        }

        public bool IsPressed(Keys key)
        {
            return _keysPressed.Contains(key);
        }

        private void OnMouseInput(RawInputEventArgs rawArgs)
        {
            var args = (MouseInputEventArgs)rawArgs;
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
                        OnKeyDown?.Invoke(args.Key);
                       Log.Info($"Key: {args.Key} State: {args.State}");
                    }
                    break;
                }

                case KeyState.KeyUp:
                {
                    _keysPressed.Remove(args.Key);
                    OnKeyUp?.Invoke(args.Key);
                    Log.Info($"Key: {args.Key} State: {args.State}");
                    break;
                }
            }
        }

        public delegate void KeyboardInputHandler(Keys key);

        public event KeyboardInputHandler OnKeyUp;
        public event KeyboardInputHandler OnKeyDown;

        public void Dispose()
        {
        }
    }
}
