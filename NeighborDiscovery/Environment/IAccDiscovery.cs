using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Nodes;

namespace NeighborDiscovery.Environment
{
    public interface IAccDiscovery
    {
        NeighborInfo this[int neighborId] { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        List<int> GetTransmissionSlotsBefore(int T0, int Tn);

        /// <summary>
        /// The period size of the new budget, ex...if the new budget is 5% Tn = 20 and the alg has 1 more transmission to send every 20 slots
        /// </summary>
        int Bp { get; }
    }
}
