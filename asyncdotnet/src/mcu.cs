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

  public class UniversalControllUnit
    {
#region fields
      private static CancellationTokenSource _cts = new CancellationTokenSource();
      private static CancellationToken _ct = _cts.Token;
#endregion
      public static CancellationToken _getCancellationToken()
        {
          return _ct;
        } 

      public static void _cancelUniversalControllUnit ()
        {
          _cts.Cancel();
        } 
    }
}
