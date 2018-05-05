/*
 * Created by SharpDevelop.
 * User: qk
 * Date: 2018/4/19
 * Time: 13:16
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
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

namespace cs_test
{
	/// <summary>
	/// Description of Foo.
	/// </summary>
	public class Foo
	{
		private const int port=80;
		private const string _host="192.168.11.1";

		public Foo()
		{
		}
		void println(string msg )
		{
			StackFrame callStack = new StackFrame(1, true);
			string _msg = "" + callStack.GetMethod() + ":" + callStack.GetFileLineNumber() + " " + msg;
			Console.WriteLine(_msg);
			
		}
		Task Test1()
		{
			Thread.Sleep(1000);
			println("create task in test1");

			return Task.Run(() =>
			                {
			                	Thread.Sleep(3000);
			                	Console.WriteLine("Test1");
			                });
		}
		async void loadAsync()
		{
			await Test1();
			println("loadAsync");
		}
		public void run()
		{
			Console.WriteLine("totalMemory={0:#,##0}\n    ", GC.GetTotalMemory(false));
			Console.WriteLine("totalMemory={0}\n    ", GC.GetTotalMemory(false).ToString("#,0") );
			loadAsync();
			println("run");
			Thread.Sleep(5000);
		}
		
	}
}
