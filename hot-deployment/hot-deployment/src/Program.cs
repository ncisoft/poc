using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QKNode;

namespace hot_deployment
{
    class Program
    {
        static IPlugin loadPlugin(string strDLLPath)
        {
            var DLL = Assembly.LoadFile(strDLLPath);
            IPlugin plugin = null;
            foreach (Type type in DLL.GetExportedTypes())
            {
                Console.WriteLine(type.Name);
                if (type.GetInterfaces().Contains( typeof(IPlugin) ))
                {
                    Console.WriteLine(type.Name + "---");
                    plugin = (IPlugin)Activator.CreateInstance(type);
                    break;
                }
            }
            return plugin;
        }
        static void Main(string[] args)
        {
            IPlugin plugin, plugin2;
            //plugin = loadPlugin(Const.mFixedPluginPath);
            plugin = loadPlugin(Const.mPluginProxyPath);
            plugin.print("-- " + Const.mPrimaryPluginPath);
            Const.pause_msg("load new version Plugin");
            plugin2 = plugin;
            plugin.print("balala");
            plugin.print("-- " + Const.mFixedPluginPath);
            plugin.print("__balala, after that will switch to new plugin");
            plugin.print("__balala 222");
            
            Const.pause_msg("will exit");
        }
    }
}
