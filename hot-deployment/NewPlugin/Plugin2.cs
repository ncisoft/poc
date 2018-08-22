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
        int id = 100;
        public void print(string msg)
        {
            Console.WriteLine("new plugin: {0} {1}", id, msg);
        }

    }
}
