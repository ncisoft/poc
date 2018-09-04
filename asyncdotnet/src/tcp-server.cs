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
using System.IO;
using System.Runtime.InteropServices;

namespace csetcd
{
  using static Utils;
  using static UniversalControllUnit ;

  public class TcpServer
    {
#region fields
      TcpListener _listener=null;   
      IWorker _worker = null; 
      private CancellationToken _ct = _getCancellationToken();
      private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
      private BlockingCollection<TcpClient>[] _mClientQueueList = new BlockingCollection<TcpClient>[ calculateWorkerThreadNum() ];
      private BlockingCollection<TcpClient> _mSpareClientQueue = new BlockingCollection<TcpClient>();
      private AsyncCallback mAcceptCallback;
#endregion

      static int calculateWorkerThreadNum()
        {
          int num = Math.Max(2, Environment.ProcessorCount) - 2; // leave 1 io thread, 1 accept io thread, 1 benchmark client process
          num = Math.Max(2, num);
          num = 2;
          Console.WriteLine("worker thread count={0}", num);

          return num;
        }
      void reusePort()
        {
          _listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        }

      public void initThreadWorker()
        {
          for (int i = 0; i < _mClientQueueList.Length; i++)
            {
               var  _mQueue = new BlockingCollection<TcpClient>();
              _mClientQueueList[i] = _mQueue;
              var _mThread = new Thread( () => { ThreadWorkerMain( i, this, _mQueue ); });
              _mThread.IsBackground = true;

              _mThread.Start();  
              Thread.Sleep(1);
            }

          {
            var _thread = new Thread(monitorGCMain);
            _thread.IsBackground = true;
            _thread.Start(); 
            Thread.Sleep(1);
          }

              {
                // fork spare thread
                var _mQueue = _mSpareClientQueue;
                var _thread = new Thread( () => { ThreadWorkerMain( 99, this, _mQueue ); });
                _thread.IsBackground = true;
                _thread.Start();
                Thread.Sleep(1);
              }
        }

      public void monitorGCMain()
      {
        Stopwatch sw = Stopwatch.StartNew();
        int count = 0;
        Console.WriteLine("starting GC monitor ...");
        while (true)
        {
          int gc_timeout = 10;
          int sleep = 4; 
          long start = sw.ElapsedMilliseconds;
          Thread.Sleep(sleep);
          long now = sw.ElapsedMilliseconds;
          if ((now - start) >= (gc_timeout + 0))
          {
            Console.WriteLine( "  GC timeout[{0}]: {1} ms", count++, now-start-sleep );
          }
        }
    }

      [ThreadStatic]
      private int mMaxQueueCount = 0;
      public int getMaxQueueCount()
      {
        return mMaxQueueCount;
      }
      
      public void ThreadWorkerMain(int _mpId, TcpServer _mpTcpServer, BlockingCollection<TcpClient> _mpQueue)
        {
          Console.WriteLine("fork [{0}] thread, Id={1}", _mpId, Thread.CurrentThread.ManagedThreadId);
          int _count = 0;
          int iConnection = 0;
          Stopwatch sw = new Stopwatch();
          long _ElapsedMilliseconds = 0;
          while (true)
            {
              IWorker worker = _worker.newInstance();
              TcpClient client = _mpQueue.Take();
              if (client == null)
                throw new InvalidDataException("got null data from _mpQueue");
              long now = sw.ElapsedMilliseconds;
              if (now >= (_ElapsedMilliseconds + 10))
                Console.WriteLine("lag: {0} {1}", iConnection, now - _ElapsedMilliseconds);
              _ElapsedMilliseconds = now;
              if (_mpQueue.Count >= _count + 5)
              {
                _count = _mpQueue.Count;
                Console.WriteLine("tid=[{0}] max_queue={1}, iConnection={2}", Thread.CurrentThread.ManagedThreadId, _count, iConnection);
              }
             //   throw new InvalidDataException("_mpQueue.Count >= 10");
          //Console.WriteLine("  threadId={0}", Thread.CurrentThread.ManagedThreadId);
              worker.process(_mpTcpServer, client, _mpTcpServer._ct);
              iConnection++;
            }

        }
      public void start(IWorker worker, string host, int port)
        {
          try
            {
              _worker = worker;
              _ct = _getCancellationToken();
              initThreadWorker();
              IPAddress serverAddr = IPAddress.Parse(host);
              _listener = new TcpListener(serverAddr, port);
              _listener.Start(128);
              _logger.Info("BeginAcceptTcpClient {0}:{1}", host, port);
              mAcceptCallback = new AsyncCallback(doAcceptListner);
              _listener.BeginAcceptTcpClient( mAcceptCallback, _listener );
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
          _cancelUniversalControllUnit();
          //Waits a little, to guarantee that all operation receive information about cancellation.
          Thread.Sleep(100);
          _listener.Stop();
          _logger.Info("stop()..");
        }

       static int _mSpareConnectionCount = 0;
        Stopwatch swAccept = Stopwatch.StartNew();

      public void processAcceptedClient(TcpClient client)
      {
          int id = client.Client.RemoteEndPoint.GetHashCode() % _mClientQueueList.Length;
          if (id < 0) id = -id;
          //Console.WriteLine("  id={0}", id);
          //Starts waiting for the next request.
          {
            var _queue = _mClientQueueList[id];
            if (_queue.Count >= 5)
            {
              _queue = _mSpareClientQueue;
               Console.WriteLine(" move connection to spare worker thread: c-{0} -> {1} from [{2}] worker", ++_mSpareConnectionCount, _queue.Count, id);
            }

            long _start = swAccept.ElapsedMilliseconds;
            _queue.Add(client);
            long _now = swAccept.ElapsedMilliseconds;
            if (_now > (_start + 10))
              Console.WriteLine("blocking queue {0}", _now - _start);
          }
      }
      
      public void doAcceptListner( IAsyncResult ar )
        {
          if(_ct.IsCancellationRequested)
            return;
          TcpListener listener = (TcpListener) ar.AsyncState;
          listener.BeginAcceptTcpClient(mAcceptCallback, listener);
          TcpClient client = listener.EndAcceptTcpClient(ar);
          processAcceptedClient(client);
          //_logger.Info("incoming client");
        }
    }

}
