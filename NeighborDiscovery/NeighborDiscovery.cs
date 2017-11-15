using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Nodes;
using NeighborDiscovery.Utils;
using NeighborDiscovery.Networks;

namespace NeighborDiscovery
{
    public class NeighborDiscovery
    {
        //public static Task<StatisticsResult> RunSimulationInTask(string fileName, NodeType type)
        //{
        //    var reader = new NetworkGenerator();
        //    IEnumerable<Network2D> networks;
        //    Task<StatisticsResult> t1 = Task.Run(() => {
        //        networks = reader.CreateFromFile(fileName, type);
        //        var environment = new NeighborDiscoveryEnvironment();
        //        return environment.RunMultipleSimulations(networks, type);
        //    });
            
        //    return t1;
        //}

        //public static StatisticsResult RunSimulation(string fileName, NodeType type)
        //{
        //    var reader = new NetworkGenerator();
        //    var networks = reader.CreateFromFile(fileName, type);
        //    var environment = new NeighborDiscoveryEnvironment();
        //    return environment.RunMultipleSimulations(networks, type);
        //}

    }
}
