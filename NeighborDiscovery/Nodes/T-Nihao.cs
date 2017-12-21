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
    public class TNihao : Node
    {
        protected int m;
        protected int n;
        protected int numberOfTransmisions;

        public TNihao(int id, int duty, int communicationRange, int channelOccupancyRate, int startUpTime, bool randomInitialState = false) : base(id, (double)duty, communicationRange, startUpTime)
        {
            m = channelOccupancyRate;
            setDutyCycle(duty, m);
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
            InternalTimeSlot += m;
            return trans;
        }

        /// <summary>
        /// calculates the first transmission that is going to be trasmited at a slot greater than or equal than the given slot
        /// </summary>
        /// <param name="slot"></param>
        /// <returns>the transmission</returns>
        public override Transmission FirstTransmissionAfter(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
            if (slot < InternalTimeSlot)
                throw new Exception("Transmission already given");
            if (IsTransmitting(slot))
            {
                InternalTimeSlot = slot;
            }
            else//not transmitting => (slot % m) != 0
            {
                InternalTimeSlot = slot + (m - (slot % m));
            }
            return NextTransmission();
        }

        /// <summary>
        /// parameter pos is cero based and represents the slot where you want to know if the node was/is/will be listening
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public override bool IsListening(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
            if (slot < 0)
                return false;

            return isInRange(slot, 0, 9) || isInRange(slot, 110, 119) || isInRange(slot, 220, 229) || isInRange(slot, 330, 339);
        }

        private bool isInRange(int slot, int l, int r)
        {
            return slot >= l && slot <= r;
        }

        /// <summary>
        /// parameter pos is cero based and represents the slot where you want to know if the node was/is/will be transmitting
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public override bool IsTransmitting(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
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
            DesiredDutyCycle = duty;
            numberOfTransmisions = n;
            T = n * m;
            buildSchedule();
        }

        public void setDutyCycle(int duty, int m)
        {
            DesiredDutyCycle = duty;
            n = getNByM(duty, m);
            numberOfTransmisions = n;
            T = n * m;
            buildSchedule();
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

        private void buildSchedule()
        {
            //listen = new bool[T];
            //transmit = new bool[T];
            //for (int i = 0; i < m; i++)
            //{
            //    listen[i] = true;
            //}
            //for (int i = 0; i < numberOfTransmisions; i++)
            //{
            //    transmit[i*m] = true;
            //}
        }

        public override IDiscovery Clone()
        {
            return new GNihao(Id, (int)DesiredDutyCycle, CommunicationRange, m, StartUpTime, false);
        }
    }
}
