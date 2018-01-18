//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NeighborDiscovery.Utils;
//using NeighborDiscovery.Environment;
//using NeighborDiscovery.Protocols;


//namespace NeighborDiscovery.Protocols
//{
//    public class BNihaoR: BoundedProtocol
//    {
//        protected int M;
//        protected int N;
//        protected int NumberOfTransmisions;
//        protected HashSet<int> ListeningSlots;

//        public BNihaoR(int id, int duty, int communicationRange,  int channelOccupancyRate, int startUpTime, bool randomInitialState = false): base(id, (double)duty, communicationRange, startUpTime)
//        {
//            M = channelOccupancyRate;
//            SetDutyCycle(duty, M);
//            InternalTimeSlot = 0;
//            ListeningSlots = new HashSet<int>();
//            MyListeningAt5();
//        }
//        /// <summary>
//        /// calling this method modifies the internal state of the node
//        /// </summary>
//        /// <returns></returns>

//        public override Transmission NextTransmission()
//        {
//            var realSlot = ToRealTimeSlot(InternalTimeSlot);
//            var trans = new Transmission(realSlot, this);
//            InternalTimeSlot += M;
//            return trans;
//        }

//        /// <summary>
//        /// calculates the first transmission that is going to be trasmited at a slot greater than or equal than the given slot
//        /// </summary>
//        /// <param name="realTimeSlot"></param>
//        /// <returns>the transmission</returns>
//        public override Transmission GetFirstTransmissionAfter(int realTimeSlot)
//        {
//            var slot = FromRealTimeSlot(realTimeSlot);
//            if (slot < InternalTimeSlot)
//                throw new Exception("Transmission already given");
//            if (IsTransmitting(slot))
//            {
//                InternalTimeSlot = slot;
//            }
//            else//not transmitting => (slot % m) != 0
//            {
//                InternalTimeSlot = slot + (M - (slot % M));
//            }
//            return NextTransmission();
//        }

//        /// <summary>
//        /// parameter pos is cero based and represents the slot where you want to know if the node was/is/will be listening
//        /// </summary>
//        /// <param name="realTimeSlot"></param>
//        /// <returns></returns>
//        public override bool IsListening(int realTimeSlot)
//        {
//            var slot = FromRealTimeSlot(realTimeSlot);
//            return slot >= 0 && ListeningSlots.Contains(slot % T);
//            //return slot % T < m;//original G-Nihao
//        }

//        public void MyListeningAt5()
//        {
//            var slots = new int[M];
//            for (var i = 0; i < M; i++)
//            {
//                slots[i] = i;
//            }
//            var shuffle = new Shuffle(M);
//            shuffle.KnuthShuffle(slots);

//            for (var i = 0; i < M; i++)
//            {
//                ListeningSlots.Add(i * M + slots[i]);
//                //ListeningSlots.Add(slots[i]);
//            }

//        }


//        /// <summary>
//        /// parameter pos is cero based and represents the slot where you want to know if the node was/is/will be transmitting
//        /// </summary>
//        /// <param name="realTimeSlot"></param>
//        /// <returns></returns>
//        public override bool IsTransmitting(int realTimeSlot)
//        {
//            var slot = FromRealTimeSlot(realTimeSlot);
//            if (slot < 0)
//                return false;
//            return slot % M == 0;
//        }

//        public double ChannelUsage()
//        {
//            return 1.0 * NumberOfTransmisions/T;
//        }

//        public override double GetDutyCycle()
//        {
//            return M * 1.0 / (N * M);
//        }

//        public void SetDutyCycle(int duty)
//        {
//            switch (duty)
//            {
//                case 1:
//                    M = 100;
//                    N = 100;
//                    break;
//                case 5:
//                    M = 20;
//                    N = 20;
//                    break;
//                case 10:
//                    M = 10;
//                    N = 10;
//                    break;

//                default:
//                    break;
//            }
//            DesiredDutyCycle = duty;
//            NumberOfTransmisions = N;
//            T = N * M;
//            BuildSchedule();
//        }

//        public void SetDutyCycle(int duty, int m)
//        {
//            DesiredDutyCycle = duty;
//            N = GetNbyM(duty, m);
//            NumberOfTransmisions = N;
//            T = N * m;
//            BuildSchedule();
//        }

//        private int GetNbyM(int duty, int m)
//        {
//            var n = 1;
//            while ((m * 1.0) / (n * m) * 100 > duty)
//            {
//                n++;
//            }
//            return n;
//        }

//        private void BuildSchedule()
//        {
//            //listen = new bool[T];
//            //transmit = new bool[T];
//            //for (int i = 0; i < m; i++)
//            //{
//            //    listen[i] = true;
//            //}
//            //for (int i = 0; i < numberOfTransmisions; i++)
//            //{
//            //    transmit[i*m] = true;
//            //}
//        }

//        public override DiscoverableDevice Clone()
//        {
//            return new BNihaoR(Id, (int)DesiredDutyCycle, CommunicationRange, M, StartUpTime, false);
//        }
//    }
//}
