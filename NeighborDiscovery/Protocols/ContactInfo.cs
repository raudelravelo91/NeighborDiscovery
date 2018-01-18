using System;

namespace NeighborDiscovery.Protocols
{
    public class ContactInfo:IContact, ICloneable
    {
        public IDiscoveryProtocol Device { get; private set; }

        public int FirstContact { get; }

        public int LastContact { get; private set; }

        public ContactInfo(IDiscoveryProtocol neighbor, int discovered)
        {
            Device = neighbor;
            FirstContact = discovered;
            LastContact = discovered;
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
            var clone = new ContactInfo(Device, FirstContact);
            clone.Update(LastContact);
            return clone;
        }
    }
}
