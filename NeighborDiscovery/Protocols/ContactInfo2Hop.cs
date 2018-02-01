using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public class ContactInfo2Hop
    {
        public BoundedProtocol Device { get; set; }
        public int ExpectedInternalSlotWhenDiscovered { get; private set; }

        public ContactInfo2Hop(BoundedProtocol device)
        {
            Device = device;
            CalculateInfo();
        }

        //public int CumulativeTransmissions(int t0, int tn)
        //{
        //    if(tn > ExpectedInternalSlotWhenDiscovered)
        //        throw new ArgumentException($"Parameter {tn} can not be beyond the expected internal slot when discovered (Property {ExpectedInternalSlotWhenDiscovered})");

        //}

        private void CalculateInfo()
        {
            var clone = Device.Clone();
            clone.MoveNext();
        }
    }
}
