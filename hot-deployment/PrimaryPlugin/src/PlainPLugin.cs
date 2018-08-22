using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QKNode;

namespace QKGame
{
    public class PlainPlugin : IPlugin
    {
        public void print(string msg)
        {
            Console.WriteLine("primary plugin: {0}", msg);
        }

    }
}
