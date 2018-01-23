using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public class DiscoverableDevice
    {
        public IDiscoveryProtocol DeviceLogic { get; }
        public MyPair Position { get; set; }
        public MyPair Direction { get; set; }
        public double Speed { get; set; }
        public int CommunicationRange { get; }

        public DiscoverableDevice(IDiscoveryProtocol deviceLogic, MyPair position, MyPair direction, double speed,
            int communicationRange)
        {
            DeviceLogic = deviceLogic;
            Position = position;
            Direction = direction;
            Speed = speed;
            CommunicationRange = communicationRange;
        }

        public override string ToString()
        {
            return DeviceLogic.ToString() + " Pos:" + Position.ToString();
        }
    }
}
