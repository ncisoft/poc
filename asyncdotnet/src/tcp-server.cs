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
  using static MicroControllUnit ;

  public class TcpServer
    {
#region fields
      TcpListener _listener=null;   
      IWorker _worker = null; 
      private CancellationToken _ct = _getCancellationToken();
      private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
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
              _ct = _getCancellationToken();
              IPAddress serverAddr = IPAddress.Parse(host);
              _listener = new TcpListener(serverAddr, port);
              _listener.Start();
              _logger.Info("BeginAcceptTcpClient {0}:{1}", host, port);
              _listener.BeginAcceptTcpClient(
                                            new AsyncCallback(doAcceptListner), 
                                            _listener);
              _logger.Info("BeginAcceptTcpClient...");
              Console.WriteLine("To quit, press eny key.");
              myReadKey();
	      stop();
            }
          catch (Exception ex)
            {
              //if (!ShouldThrow(ex))
                {
                  _logger.Info(ex.Message);
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
	      if (read) _logger.Info("recv key");
              if (read) return task.Result;
            }
          return null;
        }
      public void stop()
        {
          if(_ct.IsCancellationRequested)
            return;
          _cancelMicroControllUnit();
          //Waits a little, to guarantee that all operation receive information about cancellation.
          Thread.Sleep(100);
          _listener.Stop();
	  _logger.Info("stop()..");

        }

      public void doAcceptListner( IAsyncResult ar )
        {
          if(_ct.IsCancellationRequested)
            return;
          TcpListener listener = (TcpListener) ar.AsyncState;
          TcpClient client = listener.EndAcceptTcpClient(ar);
          _logger.Info("incoming client");
          //Starts waiting for the next request.
          listener.BeginAcceptTcpClient(doAcceptListner, listener);
          IWorker worker = _worker.newInstance();
          worker.process(this, client, _ct);
        }
    }

}
