using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LightingDemo
{
    class Program
    {
        static void Main()
        {
            using (var game = new LighingDemoGame())
            {
                game.Run();
            }
        }
    }
}
