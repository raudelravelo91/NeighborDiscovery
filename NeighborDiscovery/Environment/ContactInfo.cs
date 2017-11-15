using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Environment
{
    public class ContactInfo
    {
        public int FirstDiscovered { get; private set; }

        public int LastMet { get; private set; }

        public ContactInfo(int discovered)
        {
            FirstDiscovered = discovered;
            LastMet = discovered;
        }

        public void Update(int lastMet)
        {
            LastMet = lastMet;
        }

        public override string ToString()
        {
            return "[" + FirstDiscovered + ":" + LastMet + "]";
        }
    }
}
