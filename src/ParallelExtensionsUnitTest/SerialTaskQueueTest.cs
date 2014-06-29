using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Diagnostics;

namespace ParallelExtensionsUnitTest
{
    
    
    /// <summary>
    ///这是 SerialTaskQueueTest 的测试类，旨在
    ///包含所有 SerialTaskQueueTest 单元测试
    ///</summary>
    [TestClass()]
    public class SerialTaskQueueTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///获取或设置测试上下文，上下文提供
        ///有关当前测试运行及其功能的信息。
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region 附加测试特性
        // 
        //编写测试时，还可使用以下特性:
        //
        //使用 ClassInitialize 在运行类中的第一个测试前先运行代码
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //使用 ClassCleanup 在运行完类中的所有测试后再运行代码
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //使用 TestInitialize 在运行每个测试前先运行代码
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //使用 TestCleanup 在运行完每个测试后运行代码
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///Enqueue 的测试
        ///</summary>
        [TestMethod()]
        public void EnqueueTest()
        {
            //SerialTaskQueue target = new SerialTaskQueue(); // TODO: 初始化为适当的值
            //Func<Task> taskGenerator = null; // TODO: 初始化为适当的值
            //target.Enqueue(taskGenerator);
            //Assert.Inconclusive("无法验证不返回值的方法。");

            LimitedConcurrencyLevelTaskScheduler lcts = new LimitedConcurrencyLevelTaskScheduler(2);
            List<Task> tasks = new List<Task>();

            TaskFactory factory = new TaskFactory(lcts);
            CancellationTokenSource cts = new CancellationTokenSource();

            Object lockObj = new Object();
            int outputItem = 0;
            for (int tCtr = 0; tCtr <= 4; tCtr++)
            {
                int iteration = tCtr;
                Task t = factory.StartNew(() =>
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        System.Diagnostics.Debug.Print("{0} in task t-{1} on thread {2} ", i, iteration, Thread.CurrentThread.ManagedThreadId);

                        outputItem++;
                        if (outputItem % 3 == 0)
                            Console.WriteLine();
                    }
                }, cts.Token);
                tasks.Add(t);
            }

            for (int tCtr = 0; tCtr < 4; tCtr++)
            {
                int iteration = tCtr;
                Task t1 = factory.StartNew(() =>
                {
                    for (int outer = 0; outer <= 10; outer++)
                    {
                        for (int i = 0x21; i <= 0x7E; i++)
                        {
                            lock (lockObj)
                            {
                                System.Diagnostics.Debug.Print("'{0}' in task t1-{1} on thread {2} ", Convert.ToChar(i), iteration, Thread.CurrentThread.ManagedThreadId);
                                outputItem++;
                                if (outputItem % 3 == 0)
                                    System.Diagnostics.Debug.Print("\n");
                            }
                        }
                    }
                }, cts.Token);
                tasks.Add(t1);
            }

            Task.WaitAll(tasks.ToArray());
            System.Diagnostics.Debug.Print("\n\nSuccessful completion.");
        }

        [TestMethod()]
        public void TaskCompleationSourceTest()
        {
            TaskCompletionSource<int> tsc = new TaskCompletionSource<int>();
            Task<int> t1 = tsc.Task;
            Task.Factory.StartNew(() =>
                {
                    Thread.Sleep(1000);
                    tsc.SetResult(1);
                });

            Stopwatch sw = new Stopwatch();
            sw.Start();

            int result = t1.Result;
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("use time: " + sw.ElapsedMilliseconds + " result is :" + result);
        }
    }
}
