using System;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;


namespace csetcd
{
  public class Utils
    { 
      private static Stopwatch sw = Stopwatch.StartNew();

      public static string getGC_Plan()
	{
	  return GC.MaxGeneration == 0 ? "Boehm": "sgen";
	}

      public static void println()
      {
          log_debug("");
      }
      public static void log_debug(string fmt, params Object[] args )
      {
	      log_xxx(0, fmt, args);
      }
      public static void log_info(string fmt, params Object[] args )
      {
	      log_xxx(2, fmt, args);
      }
      private static void log_xxx(int level, string fmt, params Object[] args )
	{
          if (level >= 10)
            {
              StackFrame callStack = new StackFrame(1, true);
              string msg = String.Format(fmt, args);

              Console.WriteLine("{0,4} [{1}] - {2}:{3} {4}", 
                                sw.ElapsedMilliseconds,
                                System.Threading.Thread.CurrentThread.ManagedThreadId, 
                                callStack.GetMethod(), 
                                callStack.GetFileLineNumber(), 
                                msg);
            }
        }

      public static bool isSocketConnected(Socket s)
      {
	         bool part1 = s.Poll(1000, SelectMode.SelectRead);
		    bool part2 = (s.Available == 0);
		       if (part1 & part2)
			          {//connection is closed
				        return false;
				     }
		          return true;

      } 
    }
}
