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
        private List<int> _keysPressed = new List<int>();

        public Input()
        {
            // setup the device
            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericMouse, DeviceFlags.None);
            Device.MouseInput += (sender, args) => UpdateMouseText(args);

            Device.RegisterDevice(UsagePage.Generic, UsageId.GenericKeyboard, DeviceFlags.None);
            Device.KeyboardInput += (sender, args) => UpdateKeyboardText(args);
        }

        private void UpdateMouseText(RawInputEventArgs rawArgs)
        {
            var args = (MouseInputEventArgs)rawArgs;
        }

        private void UpdateKeyboardText(KeyboardInputEventArgs rawArgs)
        {
            OnKeyPress?.Invoke(rawArgs.Key);
            Log.Info($"Key: {rawArgs.Key} State: {rawArgs.State} ScanCodeFlags: {rawArgs.ScanCodeFlags} MakeCode: {rawArgs.MakeCode}");
        }

        public delegate void KeyboardInputHandler(Keys key);

        public event KeyboardInputHandler OnKeyPress;

        public void Dispose()
        {
        }
    }
}
