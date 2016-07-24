using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Async;
using System.Threading.Tasks;

namespace ParallelExtensionExtrasTest.TestTools
{
    public class Car
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

    public class CarProducer
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
        public void Produce(IProducerConsumerCollection<Car> toStorage)
        {
            Task producer = new TaskFactory().StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    i++;
                    Car ci = new Car(factoryid + i);
                    toStorage.TryAdd(ci);
                    System.Diagnostics.Debug.WriteLine("Produced car: " + ci.ToString());
                    Thread.Sleep(restTime);
                }
            });
        }

        public void Produce(AsyncProducerConsumerCollection<Car> toStorage)
        {
            Task producer = new TaskFactory().StartNew(() =>
            {
                int i = 0;
                while (true)
                {
                    i++;
                    Car ci = new Car(factoryid + i);
                    toStorage.Add(ci);
                    System.Diagnostics.Debug.WriteLine("Produced car: " + ci.ToString());
                    Thread.Sleep(restTime);
                }
            });
        }
    }

    public class CarConsumer
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

        public void Consume(IProducerConsumerCollection<Car> fromStorage)
        {
            Task consumer = new TaskFactory().StartNew(() =>
            {
                while (true)
                {
                    Car ci;
                    fromStorage.TryTake(out ci);
                    System.Diagnostics.Debug.WriteLine("Consume car: " + ci.ToString());
                    Thread.Sleep(restTime);
                }
            });
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

    public enum Productivity
    {
        OnePerSecond = 1,
        TenPerSecond = 10,
        HundredPerSecond = 100,
        ThousandPerSecond = 1000,
        NonLimit = Int32.MaxValue
    }
}
