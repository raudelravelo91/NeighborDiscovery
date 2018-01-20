using System;
using System.Collections.Generic;
using System.Linq;

namespace NeighborDiscovery.Protocols
{
    public sealed class Disco : BoundedProtocol
    {
        public int P1 { get; private set; }
        public int P2 { get; private set; }
        private double DesiredDutyCycle { get; set; }

        public Disco(int id, double dutyCyclePercentage) : base(id)
        {
            SetDutyCycle(dutyCyclePercentage);
        }

        public override double GetDutyCycle()
        {
            return DesiredDutyCycle;
        }

        public override void SetDutyCycle(double value)
        {
            double TOLERANCE = 1e-1;
            if (Math.Abs(value - 1) < TOLERANCE)
            {
                P1 = 17;
                P2 = 23;
            }
            else if (Math.Abs(value - 5) < TOLERANCE)
            {
                P1 = 37;
                P2 = 43;
            }
            else if (Math.Abs(value - 10) < TOLERANCE)
            {
                P1 = 17;
                P2 = 23;
            }
            else
                throw new Exception("Could not set the given duty cycle");

            DesiredDutyCycle = value;
            
        }

        public override int Bound => P1 * P2;

        public override int T => P1 * P2;

        public override IDiscoveryProtocol Clone()
        {
            return new Disco(Id, GetDutyCycle());
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " Disco (" + P1 + "," + P2 + ")";
        }

        public override bool IsListening()
        {
            return InternalTimeSlot % P1 == 0 || InternalTimeSlot % P2 == 0;
        }

        public override bool IsTransmitting()
        {
            return IsListening();
        }
    }
}
