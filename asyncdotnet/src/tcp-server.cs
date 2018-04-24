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

  public class TcpServer
    {
#region fields
      TcpListener _listener=null;   
      IWorker _worker = null; 
      private CancellationToken _ct;
      private CancellationTokenSource _cts = new CancellationTokenSource();
#endregion

      void reusePort()
      {
	      _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

      }
      public void start(IWorker worker, string host, int port)
        {
          try
            {
              _worker = worker;
              _ct = _cts.Token;
              IPAddress serverAddr = IPAddress.Parse(host);
              _listener = new TcpListener(serverAddr, port);
              _listener.Start();
              log_info("BeginAcceptTcpClient {0}:{1}", host, port);
              _listener.BeginAcceptTcpClient(
                                            new AsyncCallback(doAcceptListner), 
                                            _listener);
              log_info("BeginAcceptTcpClient...");
              Console.WriteLine("To quit, press eny key.");
              myReadKey();
	      stop();
            }
          catch (Exception ex)
            {
              //if (!ShouldThrow(ex))
                {
                  log_info(ex.Message);
                }
              // else throw;
              throw;
            }
        }

      ConsoleKeyInfo?  myReadKey()
        {
          var task = Task.Run(() => Console.ReadKey(true));

          while (!_ct.IsCancellationRequested)
            {
              bool read = task.Wait(1000);
	      if (read) log_info("recv key");
              if (read) return task.Result;
            }
          return null;
        }
      public void stop()
        {
          if(_ct.IsCancellationRequested)
            return;
          _cts.Cancel();
          //Waits a little, to guarantee that all operation receive information about cancellation.
          Thread.Sleep(100);
          _listener.Stop();
	  log_info("stop()..");

        }

      public void doAcceptListner( IAsyncResult ar )
        {
          if(_ct.IsCancellationRequested)
            return;
          TcpListener listener = (TcpListener) ar.AsyncState;
          TcpClient client = listener.EndAcceptTcpClient(ar);
          log_info("incoming client");
          //Starts waiting for the next request.
          listener.BeginAcceptTcpClient(doAcceptListner, listener);
          IWorker worker = _worker.newInstance();
          worker.process(this, client);
        }
    }

}
