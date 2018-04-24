using System;
using System.Reflection;
using static csetcd.Utils;


namespace csetcd
{
  class MainClass
    {
      public static void Main(string[] args)
      {

        try{

            Foo foo = new Foo();
            IWorker worker = new Worker(); 
            TcpServer server = new TcpServer();
            //GC.TryStartNoGCRegion(1024*1024*1024);
            server.start(worker, "127.0.0.1", 3333);
            log_info("runtime Version: {0}", Environment.Version.ToString());
#if (DEBUG && MYTEST)
            foo.run();
            log_debug("Hello World!");
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
