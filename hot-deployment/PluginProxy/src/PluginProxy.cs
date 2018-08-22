using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QKNode;

namespace QKGame
{
    public class PluginProxy : IPlugin
    {
        IPlugin mPluginInstance = null;
        IPlugin mNewPluginInstance = null;
        IPlugin loadPlugin(string strDLLPath)
        {
            var DLL = Assembly.LoadFile(strDLLPath);
            IPlugin plugin = null;
            foreach (Type type in DLL.GetExportedTypes())
            {
                Console.WriteLine(type.Name);
                if (type.GetInterfaces().Contains( typeof(IPlugin) ))
                {
                    Console.WriteLine(type.Name + "--- matched");
                    plugin = (IPlugin)Activator.CreateInstance(type);
                    break;
                }
            }
            return plugin;
        }

        void loadPluginInstance(string strPluginPath)
        {
            IPlugin _NewPlugin = loadPlugin(strPluginPath);
            if (_NewPlugin == null)
                throw new System.InvalidOperationException("can not load dll as " + strPluginPath);
            if (mNewPluginInstance != null)
                throw new System.InvalidOperationException("can not load DLL multiple times as: " + strPluginPath);
            Console.WriteLine("{0} was loaded", strPluginPath);
            if (mPluginInstance == null)
                mPluginInstance = _NewPlugin;
            else
                mNewPluginInstance = _NewPlugin;

        }
        public void print(string msg)
        {
            if (msg.StartsWith("-- "))
            {
                var strPluginPath = msg.Substring(3);
                Console.WriteLine(strPluginPath);
                loadPluginInstance(strPluginPath);
                return;
            }
            if (mPluginInstance == null)
                throw new System.InvalidOperationException("can not run plugin without initial DLL");

            mPluginInstance.print(msg);
            if (mNewPluginInstance != null)
            {
                // clean mPluginInstance
                mPluginInstance = mNewPluginInstance;
                mNewPluginInstance = null;
            }
        }
    }
}
