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
        private readonly List<DeviceData> _data;

        public IEnumerable<DeviceData> Data => _data;

        public int NetworkSize => _data.Count;

        public TestCase(IEnumerable<DeviceData> data)
        {
            _data = data.ToList();
        }
    }
}
