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

namespace csetcd
{
  using static Utils;

 public class Worker : IWorker
  {
#region Fields.
    TcpServer _tcpServer = null;
    TcpClient _client;
    NetworkStream _stream = null;
    byte[] _bs = null;
    static readonly byte[] _bsOut = System.Text.Encoding.Default.GetBytes("+PONG\r\n");
#endregion

    public Worker()
      {
      }

#region IWorker implemetation
    IWorker IWorker.newInstance()
      {
        return  new Worker();
      }
    void IWorker.process(TcpServer tcpServer, TcpClient client)
      {
        try {
            _tcpServer = tcpServer;
            _client = client;
            _stream = _client.GetStream();
            _stream.ReadTimeout = 1000;
            _bs = new byte[8192];

            _process();
        }
        catch (Exception ex)
          {
            log_debug(ex.Message);
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
        log_debug("before={0}, after={1}", length, len);
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
        log_debug("{0}, length={1}\n", msg, length);
        //Console.WriteLine( Hex.Dump(bs) );
      }

    async void _process()
      {
        while (true)
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
                    log_debug("incoming cmd , Length={0},{1}",  nRead, str.Length);
                    if (str.ToLower() == "ping")
                      {
                        log_debug("received redis ping command");
                        await _stream.WriteAsync(_bsOut, 0, _bsOut.Length);

                      }
                    else if (str.ToLower() == "exit")
                      {
                        _tcpServer.stop();
                        log_info("will shutdown");
                        _client.Close();
                        break;
                      }
                    else
                      log_debug("illegal cmd {0}, Length={1}", str, nRead);
                    //    _client.Close();
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
                log_info("exception is {0}:{1}", ex.GetType(), ex.Message);
                _client.Close();
                break;
              }
          }
      }
#endregion
  }
}
