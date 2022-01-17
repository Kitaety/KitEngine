using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.Windows;

namespace KitEngine
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Core core = new Core())
            {
                core.Run();
            }
        }
    }
}
