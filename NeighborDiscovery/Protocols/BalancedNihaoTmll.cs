using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public sealed class BalancedNihaoTmll: BoundedProtocol
    {
        public int N { get; set; }
        private double DesiredDutyCycle { get; set; }

        public BalancedNihaoTmll(int id, double dutyCyclePercentage) : base(id)
        {
            SetDutyCycle(dutyCyclePercentage);
        }

        public override double GetDutyCycle()
        {
            return DesiredDutyCycle;
        }

        public override void SetDutyCycle(double value)
        {
            switch ((int)value)
            {
                case 1:
                    N = 100;
                    break;
                case 2:
                    N = 50;
                    break;
                case 5:
                    N = 20;
                    break;
                case 10:
                    N = 10;
                    break;
                default:
                    throw new Exception("duty cycle not possible to set.");
            }
            DesiredDutyCycle = value;
        }

        public override int Bound => N * N;
        
        public override int T => N * N;

        public override IDiscoveryProtocol Clone()
        {
            return new BalancedNihaoTmll(Id, GetDutyCycle());
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " BalancedNihao (" + N + ")";
        }

        public override bool IsListening()
        {
            return InternalTimeSlot % T < N;
        }

        public override bool IsTransmitting()
        {
            return InternalTimeSlot % N == 0;
        }
    }
}
