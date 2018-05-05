/*
 * Created by SharpDevelop.
 * User: qk
 * Date: 2018/4/27
 * Time: 16:33
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Threading;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;

namespace cs_test
{
	public class Utils
	{
		static Stopwatch sw = Stopwatch.StartNew();

		public static void log_debug(string fmt, params Object[] args )
		{
			if (true)
			{
				StackFrame callStack = new StackFrame(1, true);
				string msg = String.Format(fmt, args);
				string methodName = callStack.GetMethod().Name;
				string filename = callStack.GetFileName();
				int pos = filename.LastIndexOf(Path.DirectorySeparatorChar);
				filename = filename.Substring(pos+1);

				Console.WriteLine("{0,4} [{1}] - {2} - {3}():{4} {5}",
				                  sw.ElapsedMilliseconds,
				                  System.Threading.Thread.CurrentThread.ManagedThreadId,
				                  filename, methodName,
				                  callStack.GetFileLineNumber(),
				                  msg);
			}
		}
		
		public static int getpid()
		{
			return Process.GetCurrentProcess().Id;
		}

	}
	class MethodContext
	{
		public Object obj;
		public System.Reflection.MethodInfo method;
		public Object[] parameters;
		public Object result;
		public ManualResetEvent _mConsumerResetEvent;
		
		public MethodContext(Object _obj, System.Reflection.MethodInfo _method, Object[] _parameters)
		{
			obj = _obj;
			method = _method;
			parameters = _parameters;
			_mConsumerResetEvent = new ManualResetEvent(false);
		}
		
		public void setResult(Object o)
		{
			result = o;
		}
		
		public Object getResult()
		{
			return result;
		}
	}
	
	public class ProducerExecutor : DynamicProxy.IProxyInvocationHandler
	{
		Object _objServiceImpl = null;
		Object _mInterface = null;
		
		static int sessionId = 0;
		static Thread _mainThread;
		static ConcurrentDictionary<string, ProducerExecutor> _mProducerExecutorSet = new ConcurrentDictionary<string, ProducerExecutor>();

		BlockingCollection<int> _mSessionIdQueue = new BlockingCollection<int>();
		ConcurrentDictionary<int, MethodContext> _mSessionId2MethodContext = new ConcurrentDictionary<int, MethodContext>();
		
		ProducerExecutor(Object serviceImpl)
		{
			this._objServiceImpl = serviceImpl;
		}
		ProducerExecutor()
		{
			this._objServiceImpl = null;
		}
		
		///<summary>
		/// IProxyInvocationHandler method that gets called from within the proxy
		/// instance.
		///</summary>
		///<param name="proxy">Instance of proxy</param>
		///<param name="method">Method instance
		public Object Invoke(Object proxy, System.Reflection.MethodInfo method, Object[] parameters)
		{

			Object retVal = null;
			string userRole = "role";
			// if the user has permission to invoke the method, the method
			// is invoked, otherwise an exception is thrown indicating they
			// do not have permission
			if (DynamicProxy.SecurityManager.IsMethodInRole(userRole, method.Name))
			{
				// The actual method is invoked
				//retVal = method.Invoke(_mServiceImpl, parameters);
				
				var producerExecutor = this;
				int sessionId = _fetchSessionId();
				// enqueue TRequest to queue
				MethodContext mContext = new MethodContext(_objServiceImpl, method, parameters);
				producerExecutor.enqueueRequest(sessionId, mContext);
				mContext._mConsumerResetEvent.WaitOne();
				// waiting for release by producer
				// get result from MethodContext
				retVal = mContext.getResult();
				var map = (IDictionary<int, MethodContext>)_mSessionId2MethodContext;
				map.Remove(sessionId);
			}
			else
			{
				throw new Exception("Invalid permission to invoke " + method.Name);
			}

			return retVal;
		}

		public static T newServiceInterface<T>(Object serviceImpl)
		{
			string key = typeof(T).ToString();
			ProducerExecutor producer;
			if (!_mProducerExecutorSet.TryGetValue(key, out producer))
			{
				producer = new ProducerExecutor(serviceImpl);
				var xoo = DynamicProxy.ProxyFactory.GetInstance().Create(
					producer, serviceImpl.GetType());
				producer._mInterface = xoo;
				
				_mProducerExecutorSet.TryAdd(key, producer);
				producer.startSingletonThread();
			}
			return (T)producer._mInterface;
		}
		
		
		int _fetchSessionId()
		{
			int id = Interlocked.Increment(ref sessionId);
			return id;
		}
		
		void startSingletonThread()
		{
			_mainThread = Thread.CurrentThread;
			var _mThread = new Thread(thread_main);
			_mThread.IsBackground = true;
			
			_mThread.Start();
		}
		
		void enqueueRequest(int sessionId, MethodContext mContext)
		{
			_mSessionId2MethodContext[sessionId] = mContext;
			_mSessionIdQueue.Add(sessionId);
		}
		
		Object getResponse(int sessionId)
		{
			MethodContext methodContext = _mSessionId2MethodContext[sessionId];
			return methodContext.getResult();
		}
		
		void thread_main()
		{
			int sessionId;
			while (true)
			{
				sessionId = _mSessionIdQueue.Take();
				
				MethodContext mContext = _mSessionId2MethodContext[sessionId];
				Object result;
				lock (this._objServiceImpl)
				{
					result = mContext.method.Invoke(mContext.obj, mContext.parameters);
				}
				mContext.setResult(result);
				mContext._mConsumerResetEvent.Set();
				// cleanup, _mConsumerResetEvent was reset already
				var map = (IDictionary<int, MethodContext>)_mSessionId2MethodContext;
				map.Remove(sessionId);
			}
		}
		
	}
}
