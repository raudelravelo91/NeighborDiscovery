using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Nodes;


namespace NeighborDiscovery.Protocols
{
    public class BNihao : Node
    {
        protected int M;
        protected int N;
        protected int NumberOfTransmisions;

        public BNihao(int id, int duty, int communicationRange, int channelOccupancyRate, int startUpTime) : base(id, (double)duty, communicationRange, startUpTime)
        {
            M = channelOccupancyRate;
            SetDutyCycle(duty);
            InternalTimeSlot = 0;
        }
        /// <summary>
        /// calling this method modifies the internal state of the node
        /// </summary>
        /// <returns></returns>

        public override Transmission NextTransmission()
        {
            var realSlot = ToRealTimeSlot(InternalTimeSlot);
            var trans = new Transmission(realSlot, this);
            InternalTimeSlot += M;
            return trans;
        }

        /// <summary>
        /// calculates the first transmission that is going to be trasmited at a slot greater than or equal than the given slot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>the transmission</returns>
        public override Transmission GetFirstTransmissionAfter(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
            if (slot < InternalTimeSlot)
                throw new Exception("Transmission already given, restart the node if needed.");
            if (IsTransmitting(slot))
            {
                InternalTimeSlot = slot;
            }
            else//not transmitting => (slot % m) != 0
            {
                InternalTimeSlot = slot + (M - (slot % M));
            }
            return NextTransmission();
        }

        /// <summary>
        /// parameter realTimeSlot is cero based and represents the slot where you want to know if the node was/is/will be listening
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public override bool IsListening(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
            if (slot < 0)
                return false;
            return (slot % T) < M;
        }

        /// <summary>
        /// parameter realTimeSlot is cero based and represents the slot where you want to know if the node was/is/will be transmitting
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public override bool IsTransmitting(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
            if (slot < 0)
                return false;
            return slot % M == 0;
        }

        public double channelUsage()
        {
            return 1.0 * NumberOfTransmisions / T;
        }

        public override double GetDutyCycle()
        {
            return M * 1.0 / (N * M);
        }

        public void setDutyCycle(int duty)
        {
            switch (duty)
            {
                case 1:
                    M = 100;
                    N = 100;
                    break;
                case 5:
                    M = 20;
                    N = 20;
                    break;
                case 10:
                    M = 10;
                    N = 10;
                    break;

                default:
                    break;
            }
            DesiredDutyCycle = duty;
            NumberOfTransmisions = N;
            T = N * M;
        }

        public void SetDutyCycle(int duty, int newM = -1)
        {
            if(newM > 0)
                M = newM;
            DesiredDutyCycle = duty;
            N = getNByM(duty, M);
            NumberOfTransmisions = N;
            T = N * M;
        }

        private int getNByM(int duty, int m)
        {
            var n = 1;
            while ((m * 1.0) / (n * m) * 100 > duty)
            {
                n++;
            }
            return n;
        }

        public override string ToString()
        {
            return "PNihao NodeId: " + Id + " Duty: " + Math.Round(GetDutyCycle(), 2);
        }

        public override DiscoverableDevice Clone()
        {
            return new BNihao(Id, (int)DesiredDutyCycle, CommunicationRange, M, StartUpTime);
        }
    }
}
