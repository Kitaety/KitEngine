using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KitEngine
{
    class Input
    {
        private List<int> _keysPressed = new List<int>();

        protected virtual void OnMouseDown(MouseButtons button, Point location)
        {
        }

        protected virtual void OnMouseUp(MouseButtons button, Point location)
        {
        }

        protected virtual void OnMouseMove(MouseButtons button, Point location)
        {
        }

        protected virtual void OnKeyDown(Keys keyCode)
        {
            _keysPressed.Add((int)keyCode);
        }

        protected virtual void OnKeyUp(Keys keyCode)
        {
            _keysPressed.Remove((int)keyCode);
        }

        protected bool IsKeyDown(Keys keyCode) => _keysPressed.Contains((int)keyCode);
        //protected bool IsKeyDown(Keys keyCode) => Keyboard.IsKeyDown(KeyInterop.KeyFromVirtualKey((int)keyCode));
    }
}
