//using NeighborDiscoveryLib.Environment;
//using NeighborDiscoveryLib.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeighborDiscoveryLib.Nodes
//{
//    public class TestAlgorithm : Node
//    {
        
//        private Transmission<Node> nextT;
//        private int lastNextTimeSlotGenerated;
//        private int afterValue = -1;
//        private int listening;
//        private int period;
//        private int cnt;
//        //public int T{ get; private set; }
//        private int m1;

//        public TestAlgorithm(int id, double dutyCyclePercentage, int communicationRange) : base(id, dutyCyclePercentage, communicationRange)
//        {
//            Random random = new Random();
            
//            lastNextTimeSlotGenerated = -1;
//            listening = -1;
//            cnt = 0;
//            period = T/2;
//            m1 = 0;
//        }

//        public override bool ListenTo(Transmission<Node> transmission, int slot)
//        {
//            if (Listening() != slot)
//                return false;
//            bool neighborDiscovered = false;
//            TestAlgorithmTransmission testT = transmission as TestAlgorithmTransmission;

//            if (testT != null)
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
//        private TestAlgorithmTransmission CalculateNextTransmission()
//        {
//            int nextTimeSlot = 0;
//            if (afterValue == -1)
//            {
//                int v1 = m1*T + cnt;
//                int v2 = m1*T + (3*T/4 - cnt - 1);
//                cnt++;
//                if (cnt == T / 2)
//                    cnt = 0;
//                afterValue = Math.Max(v1, v2);
//                nextTimeSlot = Math.Min(v1,v2);
//                if (afterValue == nextTimeSlot)
//                {
//                    afterValue = -1;
//                    m1++;
//                }
//            }
//            else
//            {
//                nextTimeSlot = afterValue;
//                afterValue = -1;
//                m1++;
//            }

//            if (nextTimeSlot <= lastNextTimeSlotGenerated)
//                throw new Exception("Error generating nextTimeSlot");
//            lastNextTimeSlotGenerated = nextTimeSlot;
            
//            return new TestAlgorithmTransmission(nextTimeSlot, this, desiredDutyCycle, T);
//        }

//        public override int Listening()
//        {
//            return listening;
//        }
//        public override void setDutyCycle(double dutyCyclePercentage)
//        {
//            if (dutyCyclePercentage < 1)
//            {
//                if (dutyCyclePercentage == 0.5)
//                {
//                    T = 420;
//                }
//                else throw new Exception("Duty cycle not find in node.");
//            }
//            else
//            {
//                switch ((int)dutyCyclePercentage)
//                {
//                    case 1:
//                        //m = 199;
//                        T = 198;
//                        break;
//                    case 2:
//                        T = 102;
//                        break;
//                    case 5:
//                        T = 42;
//                        break;
//                    case 10:
//                        //m = 23;
//                        T = 18;
//                        break;

//                    default://5%
//                        Console.WriteLine("NOTE: Working at 5% duty cycle");
//                        T = 40;
//                        break;
//                }
//            }
//        }
//        public override double getDutyCycle()
//        {
//            return 2.0 / T;
//        }
//    }
//}
