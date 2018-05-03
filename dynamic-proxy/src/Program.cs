using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DynamicProxy;

namespace reflection_service
{
    class Program
    {
        static void Main(string[] args)
        {
            TestBed._Main(args);
            Console.Write("Press any key to continue . . . ");
            Console.ReadKey(true);
        }
    }
}
