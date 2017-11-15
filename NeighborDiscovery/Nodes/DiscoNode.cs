//using NeighborDiscoveryLib.Environment;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeighborDiscoveryLib.Nodes
//{
//    public class DiscoNode:Node
//    {
//        public int P1 { get; private set; }
//        public int P2 { get; private set; }
//        private int M1, M2, listening;
//        private Transmission<Node> nextT;
//        private Transmission<Node> currentTransmission;
//        private int lastNextTimeSlotGenerated;
//        private bool balancedPrimes;
//        private double dc;

//        public DiscoNode(int id, double dutyCyclePercentage, int communicationRange, bool balancedPrimes = false) :base(id, dutyCyclePercentage, communicationRange)
//        {
//            this.balancedPrimes = true;
//            if (this.balancedPrimes)
//            {
//                setDutyCycle(dutyCyclePercentage);
//            }
//            M1 = 1;
//            M2 = 1;
//            listening = -1;
//            //dc = 0;
//        }
//        public override void setDutyCycle(double dutyCyclePercentage)//2,4,5 and 10 so far
//        {
//            if (dutyCyclePercentage < 1)
//            {
//                if (dutyCyclePercentage == 0.5)
//                {
//                    P1 = 397;
//                    P2 = 401;
//                    dc = 0.005;
//                }
//            }
//            else
//            {
//                switch ((int)dutyCyclePercentage)
//                {
//                    case 1:
//                        P1 = (balancedPrimes) ? 197 : 101;
//                        P2 = (balancedPrimes) ? 199 : 9973;//191 y 211 from paper searchlight. Mine is 197 199
//                        dc = 0.01;
//                        break;
//                    case 2:
//                        P1 = (balancedPrimes) ? 97 : 53;
//                        P2 = (balancedPrimes) ? 103 : 4951;//191 y 211 from paper searchlight. Mine is 197 199
//                        dc = 0.02;
//                        break;
//                    case 5:
//                        P1 = (balancedPrimes) ? 41 : 23;//paper searchlight
//                        P2 = (balancedPrimes) ? 43 : 157;
//                        dc = 0.05;
//                        break;
//                    case 10:
//                        P1 = (balancedPrimes) ? 17 : 11;
//                        P2 = (balancedPrimes) ? 19 : 101;
//                        dc = 0.1;
//                        break;

//                    default://5%
//                        Console.WriteLine("NOTE: Working at 2% duty cycle");
//                        P1 = 37;
//                        P2 = 43;
//                        dc = 0.05;
//                        break;
//                }
//            }
//            //reset the multipliers and the newTimeSlot
//            M1 = 1;
//            M2 = 1;
            
//            //dutyCycle = (int)((1.0 / P1 + 1.0 / P2) * 100d);
//        }

//        public override double getDutyCycle()
//        {
//            return dc;
//        }
//        public override Transmission<Node> NextTransmission()
//        {
//            if(nextT == null)
//                CalculateNextTransmission();
//            currentTransmission = nextT;
//            nextT = null;
//            listening = currentTransmission.Slot;
//            InternalTimeSlot = currentTransmission.Slot;
//            return currentTransmission;
//        }

//        private void CalculateNextTransmission()
//        {
//            if (nextT == null)
//            {
//                int nextTimeSlot = Math.Min(M1 * P1, M2 * P2);
//                if (nextTimeSlot <= lastNextTimeSlotGenerated)
//                    throw new Exception("Error generating nextTimeSlot");
//                lastNextTimeSlotGenerated = nextTimeSlot;
//                if (M1 * P1 < M2 * P2) M1++;
//                else if (M1 * P1 > M2 * P2) M2++;
//                else { M1++; M2++; }
//                nextT = new DiscoTransmission(nextTimeSlot, this, desiredDutyCycle, P1, P2);
//            }
//            else throw new Exception("Malfunction: The previous transmission was never called.");
//        }

//        public override bool ListenTo(Transmission<Node> transmission, int slot)
//        {
//            if (Listening() != slot)
//                return false;
//            bool neighborDiscovered = false;
//            DiscoTransmission discoT = transmission as DiscoTransmission;
            
//            if (discoT != null)
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

//        public override int Listening()
//        {
//            return listening;
//        }
//    }
//}
