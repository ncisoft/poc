using System;
using System.Reflection;
using static csetcd.Utils;
using Microsoft.Extensions.Logging;


namespace csetcd
{
  class MainClass
    {
      private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
      public static void check()
        {
              ILogger log = getLogger(typeof(MainClass));
              log.LogTrace(20, "Doing hard work! {Action}", "xxx");
              log.LogTrace(20, "Doing hard work! ", "xxx");
              log.LogTrace("Doing hard work! ", "xxx");
        }
      public static void Main(string[] args)
        {

          try{
              Foo foo = new Foo();
              IWorker worker = new Worker(); 
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
