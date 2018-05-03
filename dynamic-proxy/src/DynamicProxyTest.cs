using System;
using System.Diagnostics;

namespace DynamicProxy
{
    /// <summary>
    /// </summary>
    public class TestBed
    {
        /// <summary>
        /// </summary>
        [STAThread]
        public static void _Main(string[] args)
        {
            ITest test = (ITest)SecurityProxy.NewInstance(new TestImpl());
            test.TestFunctionOne();
            var xout = test.TestFunctionTwo(111, "haha");
            Utils.log_debug("out={0}", xout);
            benchmark("proxied interface method call, 1M iterations ", test);
            test = new TestImpl();
            benchmark("original interface method call, 1M iterations", test);
        }

        public static void benchmark(string msg, ITest test)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000*1000; i++)
                test.TestFunctionOne();
            Console.WriteLine("{0} elapsed: {1}", msg, sw.ElapsedMilliseconds);
        }

    }

    public interface ITest
    {
        void TestFunctionOne();
        Object TestFunctionTwo(int a, Object b);
    }

    public class TestImpl : ITest
    {
        public void TestFunctionOne()
        {
            //Console.WriteLine("In TestImpl.TestFunctionOne()");
            //Utils.log_debug("In TestImpl.TestFunctionOne()");
        }

        public Object TestFunctionTwo(int a, Object b)
        {
            Console.WriteLine("In TestImpl.TestFunctionTwo( Object a, Object b )");
            Utils.log_debug("In TestImpl.TestFunctionTwo( Object a={0}, Object b={1} )", a, b);
            return 2;
        }
    }
}
