using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading.Async;
using System.Threading;
using ParallelExtensionExtrasTest.TestTools;

namespace ParallelExtensionExtrasTest
{
    [TestClass]
    public class ObjectPoolTest
    {
        
        ObjectPool<string> GetToTestTest()
        {
            ObjectPool<string> toTestPool;
            Func<string> stringFactory = GetTestString;
            toTestPool = new ObjectPool<string>(stringFactory);
            return toTestPool;
        }

        [TestMethod]
        public void PutObjectTest()
        {
            //GetToTestTest.PutObject("TestStart");
        }



        [TestMethod]
        public void GetObjectTest()
        {
            var toTest =  new ObjectPool<string>(GetTestString);
            toTest.PutObject("TestStart");
            Assert.IsTrue(toTest.Count == 1);
            var result = toTest.GetObject();
            Assert.AreEqual(result, "TestStart");
            Assert.IsTrue(toTest.Count == 0);
        }

        [TestMethod]
        public void OnePerSencondTest()
        {
            ProducerConsumerTest("A", Productivity.OnePerSecond, Productivity.ThousandPerSecond);
        }
        [TestMethod]
        public void HundredPerSecondTest()
        {
            ProducerConsumerTest("A", Productivity.HundredPerSecond);
        }

        [TestMethod]
        public void ThousandPerSecondTest()
        {
            ProducerConsumerTest("A", Productivity.ThousandPerSecond);
        }

        private void ProducerConsumerTest(string factoryID, Productivity productivity, Productivity consumeSpeed = Productivity.OnePerSecond)
        {
            ObjectPool<Car> carStorage = new ObjectPool<Car>(GetTestCar);

            CarProducer producerA = new CarProducer(factoryID, Productivity.HundredPerSecond);
            CarConsumer consumer = new CarConsumer(consumeSpeed);

            producerA.Produce(carStorage);
            consumer.Consume(carStorage);

            Thread.CurrentThread.Join(60000);
        }
        private static string GetTestString()
        {
            return "";
        }

        private Car GetTestCar()
        {
            return new Car("default");
        }

    }
}
