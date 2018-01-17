using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Environment
{
    public class ContactInfo
    {
        public int FirstContact { get; private set; }

        public int LastContact { get; private set; }

        public ContactInfo(int discovered)
        {
            FirstContact = discovered;
            LastContact = discovered;
        }

        public void Update(int lastMet)
        {
            LastContact = lastMet;
        }

        public override string ToString()
        {
            return "[" + FirstContact + ":" + LastContact + "]" + "[Fst.C/Lst.C]";
        }
    }
}
