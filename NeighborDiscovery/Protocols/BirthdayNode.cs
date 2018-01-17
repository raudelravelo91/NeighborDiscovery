//using NeighborDiscoveryLib.Environment;
//using NeighborDiscoveryLib.Utils;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeighborDiscoveryLib.Nodes
//{
//    public class BirthdayNode : Node
//    {
//        private Queue<Transmission<Node>> nextTransmissions;
//        private int periodSize, TimeSlotRound, listening;
//        private Random random;

//        public BirthdayNode(int id, double dutyCyclePercentage, int communicationRange) :base(id, dutyCyclePercentage, communicationRange)
//        {
//            nextTransmissions = new Queue<Transmission<Node>>();
//            periodSize = 100;//default value
//            TimeSlotRound = 0;
//            random = new Random();
//        }

//        public override bool ListenTo(Transmission<DiscoverableDevice> transmission, int slot)
//        {
//            if (Listening() != slot)
//                return false;
//            bool neighborDiscovered = false;
//            BirthdayTransmission birthdayT = transmission as BirthdayTransmission;
//            if (birthdayT != null)
//            {
//                if (!neighbors.Contains(transmission.Sender))
//                {
//                    neighborDiscovered = true;
//                    neighbors.Add(transmission.Sender);
//                }
//            }
//            else throw new ArgumentException("Wrong transmission type. Expected: BirthdayTransmission.");
//            return neighborDiscovered;
//        }

//        public override Transmission<Node> NextTransmission()
//        {
//            double d;
//            while((d = random.NextDouble()) > getDutyCycle())
//            {
//                TimeSlotRound++;
//            }
//            listening = TimeSlotRound;
//            InternalTimeSlot = TimeSlotRound;
//            return new BirthdayTransmission(TimeSlotRound, this, desiredDutyCycle);

//        }

//        public override int Listening()
//        {
//            return listening;
//        }

//        public void SetPeriodSize(int periodSize)
//        {
//            this.periodSize = periodSize;
//        }
//    }
//}
