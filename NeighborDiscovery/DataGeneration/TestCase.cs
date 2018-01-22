using NeighborDiscovery.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.DataGeneration
{
    [Serializable]
    public class TestCase
    {
        public IEnumerable<DeviceData> Data { get; }

        public int NetworkSize { get; }

        public TestCase(IEnumerable<DeviceData> data)
        {
            Data = data;
            NetworkSize = data.Count();
        }
    }
}
