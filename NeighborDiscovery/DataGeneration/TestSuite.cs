using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.DataGeneration
{
    [Serializable]
    public class TestSuite
    {
        public IEnumerable<TestCase> Tests { get; }
        public int NumberOfTests { get; }

        public TestSuite(IEnumerable<TestCase> data)
        {
            Tests = data;
            NumberOfTests = data.Count();
        }
    }
}
