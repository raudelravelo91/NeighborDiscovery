using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Nodes;


namespace NeighborDiscovery.Nodes
{
    public class PNihao : Node
    {
        protected int m;
        protected int n;
        protected int numberOfTransmisions;

        public PNihao(int id, int duty, int communicationRange, int channelOccupancyRate, int startUpTime ,bool randomInitialState = false) : base(id, (double)duty, communicationRange, startUpTime)
        {
            m = channelOccupancyRate;
            setDutyCycle(duty, m);
            internalTimeSlot = 0;
        }
        /// <summary>
        /// calling this method modifies the internal state of the node
        /// </summary>
        /// <returns></returns>

        public override Transmission NextTransmission()
        {
            int realSlot = ToRealTimeSlot(internalTimeSlot);
            var trans = new Transmission(realSlot, this);
            internalTimeSlot += m;
            return trans;
        }

        /// <summary>
        /// calculates the first transmission that is going to be trasmited at a slot greater than or equal than the given slot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>the transmission</returns>
        public override Transmission FirstTransmissionAfter(int realTimeSlot)
        {
            int slot = FromRealTimeSlot(realTimeSlot);
            if (slot < internalTimeSlot)
                throw new Exception("Transmission already given");
            if (IsTransmitting(slot))
            {
                internalTimeSlot = slot;
            }
            else//not transmitting => (slot % m) != 0
            {
                internalTimeSlot = slot + (m - (slot % m));
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
            int slot = FromRealTimeSlot(realTimeSlot);
            if (slot < 0)
                return false;
            return (slot % T) < m/2 || ((slot % T) >= (T/2 + m/2) && (slot % T) < (T/2 + m));
        }

        /// <summary>
        /// parameter realTimeSlot is cero based and represents the slot where you want to know if the node was/is/will be transmitting
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public override bool IsTransmitting(int realTimeSlot)
        {
            int slot = FromRealTimeSlot(realTimeSlot);
            if (slot < 0)
                return false;
            return slot % m == 0;
        }

        public double channelUsage()
        {
            return 1.0 * numberOfTransmisions / T;
        }

        public override double GetDutyCycle()
        {
            return m * 1.0 / (n * m);
        }

        public void setDutyCycle(int duty)
        {
            switch (duty)
            {
                case 1:
                    m = 100;
                    n = 100;
                    break;
                case 5:
                    m = 20;
                    n = 20;
                    break;
                case 10:
                    m = 10;
                    n = 10;
                    break;

                default:
                    break;
            }
            desiredDutyCycle = duty;
            numberOfTransmisions = n;
            T = n * m;
        }

        public void setDutyCycle(int duty, int m)
        {
            desiredDutyCycle = duty;
            n = getNByM(duty, m);
            numberOfTransmisions = n;
            T = n * m;
        }

        private int getNByM(int duty, int m)
        {
            int n = 1;
            while ((m * 1.0) / (n * m) * 100 > duty)
            {
                n++;
            }
            return n;
        }

        public override string ToString()
        {
            return "PNihao NodeId: " + ID + " Duty: " + Math.Round(GetDutyCycle(), 2);
        }

        public override IDiscovery Clone()
        {
            return new PNihao(ID, (int)desiredDutyCycle, CommunicationRange, m, StartUpTime, false);
        }
    }
}
