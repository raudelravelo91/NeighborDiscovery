using NeighborDiscovery.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;

namespace NeighborDiscovery.Protocols
{
    public class AccBNihaoGreedy : AccProtocol
    {

        public AccBNihaoGreedy(int id) : base(id)
        {

        }

        public override int Bound { get; }
        public override int T { get; }
        public override IDiscoveryProtocol Clone()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override bool IsListening()
        {
            throw new NotImplementedException();
        }

        public override bool IsTransmitting()
        {
            throw new NotImplementedException();
        }

        public override double GetDutyCycle()
        {
            throw new NotImplementedException();
        }

        public override void SetDutyCycle(double value)
        {
            throw new NotImplementedException();
        }

        public override double SlotGain(int slot)
        {
            throw new NotImplementedException();
        }

        

    }
}
