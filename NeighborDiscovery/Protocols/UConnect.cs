using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Protocols
{
    public sealed class UConnect : BoundedProtocol
    {
        public int P { get; private set; }
        private double DesiredDutyCycle { get; set; }

        public UConnect(int id, double dutyCyclePercentage) :base(id)
        {
            SetDutyCycle(dutyCyclePercentage);
        }

        public override double GetDutyCycle()
        {
            return DesiredDutyCycle;
        }

        public override void SetDutyCycle(double value)
        {
            if (value < 1)
            {
                double TOLERANCE = 1e-1;
                if (Math.Abs(value - 0.5) < TOLERANCE)
                {
                    P = 307;
                }
            }
            else
            {
                switch ((int)value)
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
            DesiredDutyCycle = value;
        }

        public override int Bound => P * P;
        
        public override int T => P * P;

        public override IDiscoveryProtocol Clone()
        {
            return new UConnect(Id, GetDutyCycle());
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " UConnect (" + P + ")";
        }

        public override bool IsListening()
        {
            return (InternalTimeSlot % P == 0 || InternalTimeSlot % (P * P) < (P+1)/2);
        }

        public override bool IsTransmitting()
        {
            return IsListening();
        }
    }
}
