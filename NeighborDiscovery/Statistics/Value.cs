using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Statistics
{
    public class Value
    {
        public int TimeSlot { get; set; }
        public int TotalLatency { get; private set; }
        public int NeighborsDiscovered { get; private set; }
        public int ContactMade { get; private set; }
        public int WakesUpMade { get; private set; }
        //public double CommunicationEfficiency { get { return ContactMade*1.0 / WakesUpMade*100; } }

        public Value(int TimeSlot, int totalLatency, int neighborsDiscovered, int contactMade, int wakesUpMade)
        {
            TotalLatency = totalLatency;
            NeighborsDiscovered = neighborsDiscovered;
            ContactMade = contactMade;
            WakesUpMade = wakesUpMade;
        }

    }
}
