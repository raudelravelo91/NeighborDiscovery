//using NeighborDiscovery.Environment;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace NeighborDiscovery.Nodes
//{
//    public class AccGreedy : BNihao
//    {
//        public AccGreedy(int id, int duty, int communicationRange, int channelOccupancyRate, int startUpTime, bool randomInitialState = false) : base(id, duty, communicationRange, channelOccupancyRate, startUpTime, randomInitialState)
//        {

//        }

//        public override bool IsListening(int realTimeSlot)
//        {
//            return base.IsListening(realTimeSlot);
//        }

//        public override bool ListenTo(Transmission transmission, out List<DiscoverableDevice> discoveredNodes)
//        {
//            return base.ListenTo(transmission, out discoveredNodes);
//            //update the slots
//        }
//    }
//}
