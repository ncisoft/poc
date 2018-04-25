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
  public interface IWorker
    {
      void process(TcpServer tcpServer, TcpClient client, CancellationToken _ct);
      IWorker newInstance();
    }
}

