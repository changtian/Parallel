using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading.Async;
using System.Threading;

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

        private class Car
        {
            public Car(string license)
            {
                this.license = license;
                this.frame = "Frame of car: " + license;
            }

            public Car(string license, string frame)
            {
                this.license = license;
                this.frame = frame;
            }

            private string license;

            public string License
            {
                get { return license; }
                set { license = value; }
            }

            private string frame;

            public string Frame
            {
                get { return frame; }
                set { frame = value; }
            }

            public override string ToString()
            {
                return "[ license is: " + license + ", frame is: " + frame + ".]"; 
            }
        }

        private class CarProducer
        {
            public CarProducer(string factoryid)
            {
                this.factoryid = factoryid;
                this.productivity = Productivity.OnePerSecond;
            }

            public CarProducer(string factoryid, Productivity productivity)
            {
                this.factoryid = factoryid;
                this.productivity = productivity;
            }

            private string factoryid;

            public string FactoryID
            {
                get { return factoryid; }
                set { factoryid = value; }
            }


            private Productivity productivity;

            public Productivity Productivity
            {
                get { return productivity; }
                set { productivity = value; }
            }

            private int restTime
            {
                get { return 1000 / (int)productivity; }
            }
            public void Produce(AsyncProducerConsumerCollection<Car> toStorage)
            {
                Task producer = new TaskFactory().StartNew(() =>
                {
                    int i = 0;
                    while (true)
                    {
                        i++;
                        Car ci = new Car(factoryid+i);
                        toStorage.Add(ci);
                        System.Diagnostics.Debug.WriteLine("Produced car: " + ci.ToString());
                        Thread.Sleep(restTime);
                    }
                });
            }
        }

        private class CarConsumer
        {
            public CarConsumer(Productivity productivity)
            {
                this.productivity = productivity;
            }

            private Productivity productivity;

            public Productivity Productivity
            {
                get { return productivity; }
                set { productivity = value; }
            }

            private int restTime
            {
                get { return 1000 / (int)productivity; }
            }

            public void Consume(AsyncProducerConsumerCollection<Car> fromStorage)
            {
                Task consumer = new TaskFactory().StartNew(() =>
                {
                    while (true)
                    {
                        Car ci = fromStorage.Take().Result;
                        System.Diagnostics.Debug.WriteLine("Consume car: " + ci.ToString());
                        Thread.Sleep(restTime);
                    }
                });
            }

        }

        private enum Productivity
        {
            OnePerSecond = 1,
            TenPerSecond = 10,
            HundredPerSecond = 100,
            ThousandPerSecond = 1000,
            NonLimit = Int32.MaxValue
        }
    }
}
