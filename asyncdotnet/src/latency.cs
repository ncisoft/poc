using System.Diagnostics;

namespace csetcd
{
    public class latency
    {
        
    }
  public class LatencySlot
    {
      Stopwatch sw;

      private long msConnectedLatency;
      private long msRequestedLatency;
      private long msResponsedLatency;
      private long msCompletedLatency;

      public long ConntectedLatency
        {
          get { return msConnectedLatency; }
          set { this.msConnectedLatency = sw.ElapsedMilliseconds; }
        }

      public  long MsRequestedLatency
        {
          get { return msRequestedLatency; }
          set { this.msRequestedLatency = sw.ElapsedMilliseconds - msConnectedLatency; }
        }
      
      public  long MsResponsedLatency
        {
          get { return msResponsedLatency;}
          set { 
              msResponsedLatency = sw.ElapsedMilliseconds - msConnectedLatency - msRequestedLatency;
              msCompletedLatency = sw.ElapsedMilliseconds;
          }
        }
      
      public  long CompletedLatency
        {
          get { return msCompletedLatency; }
        }

      public LatencySlot()
        {
          sw = Stopwatch.StartNew();
          msConnectedLatency = 0;
          msRequestedLatency = 0;
          msResponsedLatency = 0;
          msCompletedLatency = 0;
        }
    }
}