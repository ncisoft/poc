using System;
using System.Diagnostics;
using System.Threading;
using System.Net.Sockets;
using System.Text;


namespace csetcd
{
  public class Utils
    { 
#region Const
    public const int BUF_SIZE=8192;
#endregion

      private static Stopwatch sw = Stopwatch.StartNew();

      public static string getGC_Plan()
	{
	  return GC.MaxGeneration == 0 ? "Boehm": "sgen";
	}

      public static string string_format(string fmt, params Object[] args )
      {
        if (args == null || args.Length%2 != 0)
          {
            throw new System.ArgumentException("args were incorrect");
          }
        StringBuilder sbuf = new StringBuilder(fmt);
        Object[] _args = new Object[args.Length/2];
        for (int i=0; i < args.Length/2; i++)
          {
            sbuf.Append(args[2*i]);
            _args[i] = args[2*i + 1];
          }
        return String.Format(sbuf.ToString(), _args);
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
