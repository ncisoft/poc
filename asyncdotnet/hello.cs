using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;


public class HelloWorld
{
  async Task<string> getBackground2()
    {
      return await Task.Run(() =>
                            {
                            Console.WriteLine("3: task will sleep 1 ms");
                            Thread.Sleep(1);
                            Console.WriteLine("4: task will sleep 1 ms done");
                            return "complete";
                            });
    }

  async Task<string> getBackground()
    {
      return await Task.Run(() =>
                            {
                            Console.WriteLine("1: invoked in task , tid={0}", Thread.CurrentThread.ManagedThreadId);
                            Console.WriteLine("1: task will sleep 1 ms, tid={0}", Thread.CurrentThread.ManagedThreadId);
                            Thread.Sleep(5);
                            Console.WriteLine("3: task sleep 1 ms done");
                            return "--";//await getBackground2();
                            });
    }

  async void run()
    {
      Console.WriteLine("0: init ..., tid={0}", Thread.CurrentThread.ManagedThreadId);
      var _out = await getBackground();
      Console.WriteLine("4:_out = {0}", _out);

    }
  public static void Main(string[] args)
    {
      HelloWorld o = new HelloWorld();
      o.run();
      Console.WriteLine("2: jump out of async call, will sleep 10 ms, tid={0}", Thread.CurrentThread.ManagedThreadId);
      Thread.Sleep(10);
      Console.WriteLine("10: exit Main()");
    }
}
