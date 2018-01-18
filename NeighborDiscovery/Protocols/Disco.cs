using System;
using System.Collections.Generic;
using System.Linq;

namespace NeighborDiscovery.Protocols
{
    public sealed class Disco : BoundedProtocol
    {
        public int P1 { get; private set; }
        public int P2 { get; private set; }

        public Disco(int id, double dutyCyclePercentage) : base(id, dutyCyclePercentage)
        {
            SetDutyCycle(dutyCyclePercentage);
            T = P1 * P2;
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
        }

        public override IDiscoveryProtocol Clone()
        {
            //todo, this is just a new node
            return new Disco(Id, DesiredDutyCycle); //todo this is not clonning the exact internal state of the node
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
