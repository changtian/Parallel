using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;

namespace ParallelExtensionExtrasTest.CoordinationDataStructures.AsyncCoordination
{
    [TestClass]
    public class AsyncCallTest
    {
        [TestMethod]
        public void ConstructorTest()
        {

        }

        [TestMethod]
        public void PostTest()
        {
            AsyncCall<string> stringAsync = AsyncCall.Create<string>(GetAction("A"), 5, 2, TaskScheduler.Default);

            stringAsync.Post("Hello");
            stringAsync.Post("World");
            stringAsync.Post("This is AI speaking.");
            stringAsync.Post("A");
            stringAsync.Post("B");
            stringAsync.Post("C");
            stringAsync.Post("D");
            stringAsync.Post("E");
            stringAsync.Post("F");
            stringAsync.Post("G");
        }

        [TestMethod]
        public void PostAsyncTest()
        {
            AsyncCall<string> sAsync = AsyncCall.Create<string>(new Func<string, Task>((s) =>
            {
                return new TaskFactory().StartNew(() => 
                {
                    System.Diagnostics.Debug.WriteLine("Task is Processing string: " + s);
                });
            }), 5, TaskScheduler.Default);


            sAsync.Post("Hello");
            sAsync.Post("World");
            sAsync.Post("This is AI speaking.");
            sAsync.Post("A");
            sAsync.Post("B");
            sAsync.Post("C");
            sAsync.Post("D");
            sAsync.Post("E");
            sAsync.Post("F");
            sAsync.Post("G");
        }

        public Action<string> GetAction(string actionName)
        {
            Action<string> task = new Action<string>((s) =>
            {
                if (s == "B")
                    Thread.Sleep(1000);
                System.Diagnostics.Debug.WriteLine("Task"+ actionName+" is Processing string: " + s);
            });

            return task;
        }

        private void HandleString(string s)
        { }
    }
}
