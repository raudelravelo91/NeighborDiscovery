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
        private List<TestCase> _tests;
        public IEnumerable<TestCase> Tests => _tests;
        public int NumberOfTests => _tests.Count;

        public TestSuite(IEnumerable<TestCase> data)
        {
            _tests = data.ToList();
        }
    }
}
