using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Protocols
{
    public class UConnect : BoundedProtocol
    {
        public int P { get; private set; }

        public UConnect(int id, double dutyCyclePercentage) :base(id, dutyCyclePercentage)
        {
            DesiredDutyCycle = dutyCyclePercentage;
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

        //public override double GetDutyCycle()
        //{
        //    return (3.0 * P + 1) / (2 * P * P);
        //}

        public override IDiscoveryProtocol Clone()
        {
            return new UConnect(Id, DesiredDutyCycle);
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " UConnect (" + P + ")";
        }

        public override bool IsListening()
        {
            return (InternalTimeSlot % P == 0 || InternalTimeSlot % (P * P) < (P / 2 + 1));
        }

        public override bool IsTransmitting()
        {
            return IsListening();
        }
    }
}
