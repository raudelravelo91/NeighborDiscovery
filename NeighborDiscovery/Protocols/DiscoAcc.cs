//using NeighborDiscovery.Environment;
//using NeighborDiscoveryLib.Environment;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeighborDiscoveryLib.Nodes
//{
//    public class DiscoAcc : Node,IAccDiscovery 
//    {
//        public int P1 { get; private set; }
//        public int P2 { get; private set; }
//        private int M1, M2, listening;
//        private Transmission<Node> nextT;
//        private Transmission<Node> currentTransmission;
//        private int lastNextTimeSlotGenerated;
//        //private List<Node> knownNeighbors;
//        private NeighborInfo[] knownNeighborsDic;
//        public int Bp
//        {
//            get; private set;
//        }
//        public int NeighborIdMaxSize;
//        NeighborInfo IAccDiscovery.this[int neighborId]
//        {
//            get
//            {
//                return knownNeighborsDic[neighborId];
//            }

//            set
//            {
//                knownNeighborsDic[neighborId] = value;
//            }
//        }

//        public DiscoAcc(int id, int dutyCyclePercentage, int communicationRange, int Bp, bool balancedPrimes = false) : base(id, dutyCyclePercentage, communicationRange)
//        {
//            //setDutyCycle(dutyCyclePercentage);
//            M1 = 1;
//            M2 = 1;
//            listening = -1;
//            NeighborIdMaxSize = 100;
//            knownNeighborsDic = new NeighborInfo[NeighborIdMaxSize+1];
//        }
//        public override void setDutyCycle(double dutyCyclePercentage)//2,4,5 and 10 so far
//        {
//            if (dutyCyclePercentage < 1)
//            {
//                if (dutyCyclePercentage == 0.05)
//                {

//                }
//            }
//            else
//            {
                
//                bool acc = true;
//                switch ((int)dutyCyclePercentage)
//                {
//                    case 1:
//                        P1 = (acc) ? 191 : 397;
//                        P2 = (acc) ? 211 : 409;
//                        break;
//                    case 5:
//                        P1 = (acc) ? 37 : 79;
//                        P2 = (acc) ? 43 : 83;
//                        break;
//                    case 10:
//                        P1 = (acc) ? 17 : 37;
//                        P2 = (acc) ? 23 : 43;
//                        break;

//                    default://5%
//                        Console.WriteLine("NOTE: Working at 2% duty cycle");
//                        P1 = 37;
//                        P2 = 43;
//                        break;
//                }
//            }
//            //reset the multipliers and the newTimeSlot
//            M1 = 1;
//            M2 = 1;

//            desiredDutyCycle = (int)((1.0 / P1 + 1.0 / P2) * 100d);
//        }

//        public override double getDutyCycle()
//        {
//            return (P1 + P2) * 1.0 / (P1 * P2);
//        }

//        public override Transmission<Node> NextTransmission()
//        {
//            //this is the final nextTranssmission, the chosen one!
//            return null;//TODO    
//        }

//        private void CalculateDiscoTransmission()
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

//        private Transmission<Node> discoTransmission()
//        {
//            if (nextT == null)
//                CalculateDiscoTransmission();
//            currentTransmission = nextT;
//            nextT = null;
//            listening = currentTransmission.Slot;
//            InternalTimeSlot = currentTransmission.Slot;
//            return currentTransmission;
//        }

//        public override bool ListenTo(Transmission<Node> transmission, int slot)
//        {
//            bool neighborDiscovered = false;
//            DiscoTransmission discoT = transmission as DiscoTransmission;

//            if (discoT != null)
//            {
//                int senderId = discoT.Sender.ID;
//                if (knownNeighborsDic[senderId] != null)
//                {
//                    neighborDiscovered = true;
//                    neighbors.Add(transmission.Sender);
                    
//                }
//                else
//                {
//                    knownNeighborsDic[senderId].LastSlotRendevouz = discoT.Slot;
//                    //TODO:update known neighbors and probable neighbors. 
//                }
//            }
//            else throw new ArgumentException("Wrong transmission type. Expected: DiscoTransmission.");

//            return neighborDiscovered;
//        }

//        public override int Listening()
//        {
//            return listening;
//        }

//        //ACC methods
//        private double TemporalDiversity(int neighbor, int slotT0, int slotT1)//of a known neighbor
//        {
//            double result = 0;
//            var myTransmissionSet = GetTransmissionSlotsBefore(slotT0, slotT1);
//           // var neighborTransmissionSet = 
//            double denominator = slotT1 - slotT0;

//            return result;
//        }

//        private int Union(List<int> orderList1, List<int> orderList2)
//        {
//            int total = 0;
//            int j = 0;
//            int i = 0;
//            while(i < orderList1.Count)
//            {
//                if (j == orderList2.Count)
//                    break;
//                if (orderList1[i] == orderList2[j])
//                {
//                    total++;
//                    i++;
//                    j++;
//                }
//                else if (orderList1[i] < orderList2[j])
//                {
//                    i++;
//                }
//                else j++;
//            }
//            return total;
//        }

//        public List<int> GetTransmissionSlotsBefore(int T0, int Tn)
//        {
//            var result = new List<int>();
//            for (; T0<Tn; T0++)
//            {
//                if (T0 % P1 == 0 || T0 % P2 == 0)
//                    result.Add(T0);
//            }
//            return result;
//        }

        
//    }
//}
