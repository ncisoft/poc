using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using  System.Diagnostics;
 
namespace csetcd
{
  using static Utils;

  public class Foo
    {
      private const int port=80;
      private const string _host="192.168.11.1";

      public Foo()
        {
        }
      public void println(string msg )
        {
          StackFrame callStack = new StackFrame(1, true);
          string _msg = "" + callStack.GetMethod() + ":" + callStack.GetFileLineNumber() + " " + msg;
          Console.WriteLine("{0}:{1} {2}", callStack.GetMethod(), callStack.GetFileLineNumber(), msg);
        }
      Task Test1()
        {
          Thread.Sleep(1000);
          log_debug("create task in test1, will run Task as below");

          return Task.Run(() =>
                          {
                          Thread.Sleep(1000);
                          log_debug("Task.Run() Test1");
                          });
        }
      async void loadAsync()
        {
          await Test1();
          log_debug("run loadAsync");
        }
      public void run()
        {
          log_debug("mono current GC Plan = {0}\n", Utils.getGC_Plan());

          log_debug("start run()...");
          loadAsync();
          log_debug("out of async func, will sleep 3s");
          Thread.Sleep(3000);
          log_debug("run here, done sleep 3s");
        }
      public void check()
      {

      }
    }
}
