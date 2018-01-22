using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public sealed class NeighborDiscoveryEnvironment
    {
        private int EndsAt(int startUpSlot, BoundedProtocol device)
        {
            return startUpSlot + device.Bound;
        }

        public IDiscoveryProtocol FromDeviceDataToDiscoverableDevice(DeviceData data)
        {
            //todo
            return null;
        }

        public StatisticTestResult  RunSingleSimulation(IEnumerable<DeviceData> data, NodeType deviceProtocol)
        {
            List<DeviceData> _data = data.ToList();
            _data.Sort();

            int slot = 0;
            //todo

            FullDiscoveryEnvironmentTmll fullEnv = new FullDiscoveryEnvironmentTmll(RunningMode.StaticDevices);

            

            return fullEnv.GetCurrentResult();
        }


    }
}
