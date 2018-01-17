//using NeighborDiscoveryLib.Environment;
//using NeighborDiscoveryLib.Nodes;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeighborDiscovery.Nodes
//{
//    public class HelloNode:Node
//    {
//        public int N { get; private set; }
//        public int C { get; private set; }
//        private int listening;
//        private Transmission<Node> nextT;
        
//        private int lastNextTimeSlotGenerated;
//        private bool symmetric;
//        private int[] helloValues;
//        private int cnt;
//        private int M1;

//        public HelloNode(int id, double dutyCyclePercentage, int communicationRange, bool symmetric = false) :base(id, dutyCyclePercentage, communicationRange)
//        {
//            this.symmetric = symmetric;
//            if (symmetric)
//            {
//                setDutyCycle(dutyCyclePercentage);
//            }
//            M1 = 0;
//            helloValues = new int[N + C / 2];
//            cnt = 0;
//            listening = -1;
//            lastNextTimeSlotGenerated = -1;
          
//            SetHelloValues();

//        }

//        private void SetHelloValues()
//        {
//            int v = 0;
//            int i = 0;
//            while (i < N)
//            {
//                helloValues[i] = v;
//                i++;
//                v += C;
//            }
//            v = 1;
//            while (v <= C / 2)
//            {
//                helloValues[i] = v;
//                i++;
//                v++;
//            }
//            Array.Sort(helloValues);
//        }

      

//        public override bool ListenTo(Transmission<Node> transmission, int slot)
//        {
//            bool neighborDiscovered = false;
//            HelloTransmission helloT = transmission as HelloTransmission;

//            if (helloT != null)
//            {
//                if (!neighbors.Contains(transmission.Sender))
//                {
//                    neighborDiscovered = true;
//                    neighbors.Add(transmission.Sender);
//                }
//            }
//            else throw new ArgumentException("Wrong transmission type. Expected: DiscoTransmission.");

//            return neighborDiscovered;
//        }

//        public override Transmission<Node> NextTransmission()
//        {
//            nextT = CalculateNextTransmission();
//            listening = nextT.Slot;
//            InternalTimeSlot = nextT.Slot;
//            return nextT;
//        }

//        private HelloTransmission CalculateNextTransmission()
//        {
//            int nextTimeSlot = M1*C*N + helloValues[cnt];
//            cnt++;
//            if (cnt == helloValues.Length)
//            {
//                cnt = 0;
//                M1++;
//            }

//            if (nextTimeSlot <= lastNextTimeSlotGenerated)
//                throw new Exception("Error generating nextTimeSlot");
//            lastNextTimeSlotGenerated = nextTimeSlot;

//            return new HelloTransmission(nextTimeSlot, this, desiredDutyCycle, C, N);
//        }

//        public override int Listening()
//        {
//            return listening;
//        }

//        public override void setDutyCycle(double dutyCyclePercentage)
//        {
//            if (dutyCyclePercentage < 1)
//                throw new ArgumentException("Invalid Duty Cycle");

//            switch ((int)dutyCyclePercentage)
//            {
//                //Hello (11,50), (23,73) and (101,5000)
//                case 1:
//                    C = (symmetric)?200: 101;
//                    N = (symmetric)?100: 5000;
//                    break;
//                case 5:
//                    C = (symmetric)?40: 23;
//                    N = (symmetric)?20: 73;
//                    break;
//                case 10://only asymmetric
//                    C = (symmetric)?20: 11;
//                    N = (symmetric)?10: 50;
//                    break;

//                default://5%
//                    Console.WriteLine("NOTE: Working at 5% duty cycle");
//                    C = 40;
//                    N = 20;//symmetric case only
//                    break;
//            }

//            desiredDutyCycle = (int)((C / 2.0 + N) / (C * N) * 100d);
//        }

//        public override double getDutyCycle()
//        {
//            return (C/2.0+N)/C*N;
//        }
//    }
//}
