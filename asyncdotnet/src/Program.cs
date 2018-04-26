using System;
using System.Reflection;
using static csetcd.Utils;
using System.Globalization;


namespace csetcd
{
  class MainClass
    {
      private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
      public static void check()
        {
        }
      public static void Main(string[] args)
        {

          try{
	Console.WriteLine("totalMemory={0:#,##0}\n    ", GC.GetTotalMemory(false));
	Console.WriteLine("totalMemory={0}\n    ", GC.GetTotalMemory(false).ToString("#,##0", new CultureInfo("en-US")) );
              Foo foo = new Foo();
              IWorker worker = new RedisWorker(); 
              TcpServer server = new TcpServer();
              //GC.TryStartNoGCRegion(1024*1024*1024);
              server.start(worker, "127.0.0.1", 3333);
              _logger.Info("runtime Version: {0}", Environment.Version.ToString());
#if (DEBUG && MYTEST)
              foo.run();
              _logger.Debug("Hello World!");
              //          Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Version);
              //          Console.WriteLine(args[1]);
#endif  
          }
          catch (Exception ex)
            {
              throw ex;
            }
        }
    }
}
