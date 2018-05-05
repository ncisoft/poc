using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
namespace DynamicProxy
{
    public class Utils
    {
        static Stopwatch sw = Stopwatch.StartNew();

        public static void log_debug(string fmt, params Object[] args)
        {
            if (true)
            {
        				StackFrame callStack = new StackFrame(1, true);
				string msg = String.Format(fmt, args);
				string methodName = callStack.GetMethod().Name;
				string filename = callStack.GetFileName();
				int pos = filename.LastIndexOf(Path.DirectorySeparatorChar);
				filename = filename.Substring(pos+1);

				Console.WriteLine("{0,4} [{1}] - {2} - {3}():{4} {5}",
				                  sw.ElapsedMilliseconds,
				                  System.Threading.Thread.CurrentThread.ManagedThreadId,
				                  filename, methodName,
				                  callStack.GetFileLineNumber(),
				                  msg);
    }
        }

    }
}
