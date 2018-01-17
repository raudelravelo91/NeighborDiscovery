using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Protocols
{
    public class UConnectNode : Node
    {
        public int P { get; private set; }

        private int listening;
        

        public UConnectNode(int id, double dutyCyclePercentage, int communicationRange, int startUpTime) :base(id, dutyCyclePercentage, communicationRange, startUpTime)
        {
            listening = -1;
            InternalTimeSlot = 0;

        }

        public override Transmission NextTransmission()
        {
            var trans = new Transmission(ToRealTimeSlot(InternalTimeSlot), this);
            //calculate next
            if (InternalTimeSlot % T < P / 2)
            {
                InternalTimeSlot++;
            }
            else InternalTimeSlot += (P - InternalTimeSlot % P);

            return trans;
        }

        public override Transmission GetFirstTransmissionAfter(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
            if (IsTransmitting(slot))
            {
                InternalTimeSlot = slot;
            }
            else
            {
                InternalTimeSlot = slot + (P - slot%P);
            }
            return NextTransmission();
        }

        public override bool IsListening(int realTimeSlot)
        {
            var slot = FromRealTimeSlot(realTimeSlot);
            return slot % P == 0 || slot % T <= P / 2;
        }

        public override bool IsTransmitting(int realTimeSlot)
        {
            return IsListening(realTimeSlot);//the node transmits and listens at the same time
        }

        public override void SetDutyCycle(double duty)
        {
            if (duty < 1)
            {
                if (duty == 0.5)
                {
                    P = 307;
                }
            }
            else
            {
                switch ((int)duty)
                {
                    case 1:
                        P = 151;
                        break;
                    case 2:
                        P = 79;
                        break;
                    case 5:
                        P = 31;
                        break;
                    case 10:
                        P = 15;
                        break;

                    default://5%
                        Console.WriteLine("WARNING!...node working at the default duty cycle");
                        P = 31;
                        //throw new Exception("duty cycle not possible to set.");
                        break;
                }
            }
            T = P * P;
            DesiredDutyCycle = duty;
        }

        public override double GetDutyCycle()
        {
            return (3.0 * P + 1) / (2 * P * P);
        }

        public override DiscoverableDevice Clone()
        {
            return new UConnectNode(Id, DesiredDutyCycle,CommunicationRange, StartUpTime);
        }
    }
}
