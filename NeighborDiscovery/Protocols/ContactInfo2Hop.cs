using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public class ContactInfo2Hop: IContact, ICloneable
    {
        public IDiscoveryProtocol Device { get; set; }
        public int FirstContact { get; }
        public int LastContact { get; private set; }
        public int ExpectedDiscoverySlot { get; }
        public int MissedToListen { get; private set; }

        public ContactInfo2Hop(IDiscoveryProtocol device, int discovered, int expectedDiscoverySlot)
        {
            Device = device;
            FirstContact = LastContact = discovered;
            ExpectedDiscoverySlot = expectedDiscoverySlot;
        }

        public void UpdateMissed(int number)
        {
            MissedToListen = number;
        }

        public void Update(int lastContact)
        {
            LastContact = lastContact;
        }

        public override string ToString()
        {
            return "[" + FirstContact + ":" + LastContact + "]" + "[Fst.C/Lst.C]";
        }

        public object Clone()
        {
            var clone = new ContactInfo2Hop(Device, FirstContact, ExpectedDiscoverySlot);
            clone.Update(LastContact);
            clone.UpdateMissed(MissedToListen);
            return clone;
        }

    }
}
