using System;
using System.Collections.Generic;
using System.Windows.Forms;
using KitEngine.Common;
using SharpDX.DirectInput;
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

            //Log.Info($"(x,y):({args.X},{args.Y}) Buttons: {args.ButtonFlags} State: {args.Mode} Wheel: {args.WheelDelta}");
        }

        private void UpdateKeyboardText(KeyboardInputEventArgs rawArgs)
        { 
            if (rawArgs.Key == Keys.W)
            {
                Log.Error("!!!Exit!!!");
            }

            Log.Info(
                $"Key: {rawArgs.Key} State: {rawArgs.State} ScanCodeFlags: {rawArgs.ScanCodeFlags} MakeCode: {rawArgs.MakeCode}");
        }

        public void Dispose()
        {
        }
    }
}
