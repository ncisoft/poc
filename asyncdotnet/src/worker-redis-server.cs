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

namespace csetcd
{
  using static Utils;

  public class RedisWorker : IWorker
  {
    private static NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();
#region Fields.
    TcpServer _tcpServer = null;
    TcpClient _client;
    NetworkStream _stream = null;
    byte[] _bs = null;
    CancellationToken _ct;
    static readonly byte[] _bsOut = System.Text.Encoding.Default.GetBytes("+PONG\r\n");
    static readonly byte[] _bsExit = System.Text.Encoding.Default.GetBytes("+OK exit\r\n");
#endregion
    ConcurrentDictionary<int, bool> _mThreadSet = new ConcurrentDictionary<int, bool>();

    public RedisWorker()
      {
      }

#region IWorker implemetation
    IWorker IWorker.newInstance()
      {
        return  new RedisWorker();
      }
    void IWorker.process(TcpServer tcpServer, TcpClient client, CancellationToken ct)
      {
        try {
            _tcpServer = tcpServer;
            _client = client;
            _stream = _client.GetStream();
            _ct = ct;
            _stream.ReadTimeout = 1000;
            _bs = new byte[BUF_SIZE];

            _process();
        }
        catch (Exception ex)
          {
            _logger.Debug(ex.Message);
          }
      }
    string trim(byte[] bs, int length)
      {
        int len = length;
        while (len > 0 && bs[len-1] == '\n')
          {
            len--;
            if (len > 0 && bs[len-1] == '\r')
              len--;
          }
        _logger.Debug("before={0}, after={1}", length, len);
        return System.Text.Encoding.Default.GetString(bs, 0, len);
      }

    string parse_redis_protocol(byte[] bs, int length)
      {
        int offset = 0;
        if (bs[0] == '*' && bs[1] == '1')
          {
            offset += 4;
            if (bs[offset] == '$')
              {
                int  nbytes = bs[offset+1] - '0';
                offset += 4;
                return System.Text.Encoding.Default.GetString(bs, offset, nbytes);
              }
          }
        else
          {
            return trim(bs, length);
          }
        return "unknown";
      }
    void dump_bs(string msg, byte[] ba, int length)
      {
        byte[] bs = new byte[length];
        Array.Copy(ba, 0, bs, 0, length);
        _logger.Debug("{0}, length={1}\n", msg, length);
        //Console.WriteLine( Hex.Dump(bs) );
      }

    async Task meth_ping()
      {
        _logger.Debug("received redis ping command");
        await _stream.WriteAsync(_bsOut, 0, _bsOut.Length);
      }

    void close()
      {
        _stream.Close();
        _client.Close();
        _bs = null;
        _stream = null;
        _client = null;
      }
    async Task meth_exit()
      {
        await _stream.WriteAsync(_bsExit, 0, _bsOut.Length);
        _tcpServer.stop();
        _logger.Fatal("will shutdown");
        close();
      }

    async Task meth_gcstatus()
      {
        string msg = string_format("+OK gc.status \n    ",
                                   "maxGeneration={0}, ", GC.MaxGeneration,
                                   "totalMemory={1:N0}\n    ", GC.GetTotalMemory(false),
                                   "collectionCount[0]={2}\n    ", GC.CollectionCount(0),
                                   "collectionCount[1]={3}\n    ", GC.CollectionCount(1),
                                   "collectionCount[2]={4}\n    ", GC.CollectionCount(2),
                                   "max_thread_count={5}\n\n",   _mThreadSet.Count
        );
        // reset _mThreadSet
        _mThreadSet = new ConcurrentDictionary<int, bool>();
        byte[] bs = System.Text.Encoding.Default.GetBytes(msg);
        await _stream.WriteAsync(bs, 0, bs.Length);
        close();
      }
    async Task meth_gccollect()
      {
        string msg = string_format("+OK gc.status before\n    ",
                                   "maxGeneration={0}, ", GC.MaxGeneration,
                                   "totalMemory={1:N0}\n    ", GC.GetTotalMemory(false),
                                   "collectionCount[0]={2} \n    ", GC.CollectionCount(0),
                                   "collectionCount[1]={3} \n   ", GC.CollectionCount(1),
                                   "collectionCount[2]={4}\n\n ", GC.CollectionCount(2)
        );

        byte[] bs = System.Text.Encoding.Default.GetBytes(msg);
        await _stream.WriteAsync(bs, 0, bs.Length);
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();
        await meth_gcstatus();
      }

    async void _process()
      {
        while (!_ct.IsCancellationRequested && _stream != null)
          {
            try
              {
                int nRead = await _stream.ReadAsync(_bs, 0, _bs.Length);
                if (nRead > 0)
                  {
                    dump_bs("dump incoming msg", _bs, nRead);
                    string str = parse_redis_protocol(_bs, nRead);
                    byte[] bs2 = System.Text.Encoding.Default.GetBytes(str);
                    dump_bs("dump incoming msg", bs2, bs2.Length);
                    _logger.Warn("incoming cmd={0} , Length={1},{2}",  str, nRead, str.Length);
                    _mThreadSet.TryAdd(Thread.GetCurrentProcessorId(), true);
                    switch (str.ToLower())
                      {
                      case "ping":
                        await meth_ping();
                        break;
                      case "gcstatus":
                        await meth_gcstatus();
                        break;
                      case "gccollect":
                        await meth_gccollect();
                        break;
                      case "exit":
                        await meth_exit();
                        break;
                      default:
                        Console.WriteLine("illegal cmd {0}, Length={1}", str, nRead);
                        break;
                      }
                  }
                else
                  {
                    if (!isSocketConnected(_client.Client))
                      _client.Client.Disconnect(true);
                    break;
                  }
              }
            catch (Exception ex)
              {
                _logger.Info("exception is {0}:{1}", ex.GetType(), ex.Message);
                _client.Close();
                break;
              }
          }
      }
#endregion
  }
}
