using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hot_deployment
{
    class Const
    {
        public static string mPluginProxyPath = @"D:\msys64\home\qk\develop\poc\hot-deployment\PluginProxy\bin\Debug\PluginProxy.dll";
        public static string mPrimaryPluginPath = @"D:\msys64\home\qk\develop\poc\hot-deployment\PrimaryPlugin\bin\Debug\PrimaryPlugin.dll";
        public static string mFixedPluginPath = @"D:\msys64\home\qk\develop\poc\hot-deployment\NewPlugin\bin\Debug\NewPlugin.dll";

        public static void pause_msg(string msg)
        {
            Console.WriteLine("{0}, press any key to continue ...", msg);
            Console.ReadKey();
        }
    }
}
