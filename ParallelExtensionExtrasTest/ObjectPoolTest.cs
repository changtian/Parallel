using System;
using System.Collections.Concurrent;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading.Async;
using System.Threading;

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
            public void Produce(ObjectPool<Car> toStorage)
            {
                Task producer = new TaskFactory().StartNew(() =>
                {
                    int i = 0;
                    while (true)
                    {
                        i++;
                        Car ci = new Car(factoryid + i);
                        toStorage.PutObject(ci);
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

            public void Consume(ObjectPool<Car> fromStorage)
            {
                Task consumer = new TaskFactory().StartNew(() =>
                {
                    while (true)
                    {
                        Car ci = fromStorage.GetObject();
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
