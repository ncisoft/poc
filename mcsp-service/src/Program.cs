/*
 * Created by SharpDevelop.
 * User: qk
 * Date: 2018/4/17
 * Time: 21:52
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Diagnostics;

namespace cs_test
{
	public class  UserInfo
	{
		public int id;
		public string name;
		public int age;
		public UserInfo() {}
	}

	public interface IUserContext
	{
		UserInfo getUserInfo(int userId);
	}
	
	public class UserContextImpl : IUserContext
	{
		public UserInfo getUserInfo(int userId)
		{
			Utils.log_debug("in method()");
			UserInfo userInfo = new UserInfo();
			userInfo.id = userId*100;
			userInfo.name = "leeyg";
			userInfo.age = 99;
			return userInfo;
		}
	}
	
	
	public interface IGM
	{
		int getBestPlayer();
	}
	
	public class GMImpl : IGM
	{
		public int getBestPlayer()
		{
			return 0;
		}


	}

	class Program
	{
		public static void benchmark(string msg, IGM test)
        {
            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < 1000*10; i++) {
                test.getBestPlayer();
            }
            Utils.log_debug("{0} elapsed: {1}ms", msg, sw.ElapsedMilliseconds);
        }
		public static void run()
		{
			// 多消费者、单生产者的服务模式
			IUserContext userContext = ProducerExecutor.newServiceInterface<IUserContext>(new UserContextImpl());
			UserInfo userInfo = userContext.getUserInfo(88);
			Utils.log_debug("stage-100 id={0} name={1}, age={2}", userInfo.id, userInfo.name, userInfo.age);
			
			IGM gm = ProducerExecutor.newServiceInterface<IGM>(new GMImpl());
			Utils.log_debug("best player id={0}", gm.getBestPlayer());
			benchmark("proxied benchmark:", gm);
			gm = new GMImpl();
			benchmark("raw     benchmark:", gm);
			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
			Environment.Exit(0);
		}
		
		public static void Main(string[] args)
		{
			Console.WriteLine("Hello World! pid={0}", Utils.getpid());
			// TODO: Implement Functionality Here
			run	();
			Foo foo = new Foo();
			foo.run();
			var o = GC.GetGeneration(foo);
			Console.WriteLine("mono GC current plan = {0}", GC.MaxGeneration == 0 ? "Boehm": "sgen");

			Console.Write("Press any key to continue . . . ");
			Console.ReadKey(true);
			
		}
	}
}