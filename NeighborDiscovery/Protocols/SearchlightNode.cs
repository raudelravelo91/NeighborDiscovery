//using NeighborDiscoveryLib.Environment;
//using NeighborDiscoveryLib.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeighborDiscoveryLib.Nodes
//{
//    public class SearchlightNode : Node
//    {
//        //public int T { get; private set; }
        
//        //private Shuffle random;
//        private Transmission<Node> nextT;
//        private int lastNextTimeSlotGenerated;
//        private int listening;
//        private int M1;
//        private int rindex;
//        private int[] probingValues;
//        private bool isRandom;
//        private double dc;

//        public SearchlightNode(int id, double dutyCyclePercentage, int communicationRange, bool searchlightR = true) :base(id, dutyCyclePercentage, communicationRange)
//        {
            
//            lastNextTimeSlotGenerated = -1;
//            listening = -1;
//            rindex = 0;
//            M1 = 0;
//            probingValues = new int[T / 2];
//            isRandom = searchlightR;
            
//            for (int i = 0; i < probingValues.Length; i++)
//                probingValues[i] = i + 1;
//            if (isRandom)
//            {
//                //setting up SearchlightRandom by shuffeling the probing values with Knuth's algorithm
//                //random = new Shuffle(T / 2);
//                //random.KnuthShuffle(probingValues); 
//            }
//        }

//        public override bool ListenTo(Transmission<Node> transmission, int slot)
//        {
//            if (Listening() != slot)
//                return false;
//            bool neighborDiscovered = false;
//            SearchlightTransmission searchRT = transmission as SearchlightTransmission;

//            if (searchRT != null)
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
//        private SearchlightTransmission CalculateNextTransmission()
//        {
//            int nextTimeSlot = ((lastNextTimeSlotGenerated % T == 0)?M1++*T+probingValues[rindex++] :M1*T);
//            if (rindex == probingValues.Length)
//            {
//                rindex = 0;
//            }

//            if (nextTimeSlot <= lastNextTimeSlotGenerated)
//                throw new Exception("Error generating nextTimeSlot");
//            lastNextTimeSlotGenerated = nextTimeSlot;
            
//            return new SearchlightTransmission(nextTimeSlot, this, desiredDutyCycle, T);
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
//                    T = 400;
//                    dc = 0.005;
//                }
//                else throw new Exception("Duty cycle not find in node.");
//            }
//            else
//            {
//                switch ((int)dutyCyclePercentage)
//                {
//                    case 1:
//                        T = 190;
//                        dc = 0.01;
//                        break;
//                    case 2:
//                        T = 100;
//                        dc = 0.02;
//                        break;
//                    case 5:
//                        T = 42;
//                        dc = 0.05;
//                        break;
//                    case 10:
//                        T = 19;
//                        dc = 0.1;
//                        break;

//                    default://5%
//                        Console.WriteLine("NOTE: Working at 5% duty cycle");
//                        T = 40;
//                        dc = 0.05;
//                        break;
//                }
//            }
//            //reset the multipliers and the newTimeSlot
//            desiredDutyCycle = (int)((2.0/ T) * 100d);
//        }
//        public override double getDutyCycle()
//        {
//            return dc;
//        }
//    }
//}
