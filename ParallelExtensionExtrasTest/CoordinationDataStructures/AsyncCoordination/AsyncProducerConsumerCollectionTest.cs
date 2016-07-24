using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading.Async;
using System.Threading;
using ParallelExtensionExtrasTest.TestTools;

namespace ParallelExtensionExtrasTest.CoordinationDataStructures.AsyncCoordination
{
    [TestClass]
    public class AsyncProducerConsumerCollectionTest
    {
        private AsyncProducerConsumerCollection<Car> carStorage = new AsyncProducerConsumerCollection<Car>();

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

        private void ProducerConsumerTest(string factoryID, Productivity productivity, Productivity consumeSpeed= Productivity.OnePerSecond)
        {
            CarProducer producerA = new CarProducer(factoryID, Productivity.HundredPerSecond);
            CarConsumer consumer = new CarConsumer(consumeSpeed);
            
            producerA.Produce(carStorage);
            consumer.Consume(carStorage);

            Thread.CurrentThread.Join(60000);
        }

       
    }
}
