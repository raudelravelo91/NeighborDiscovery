//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using NeighborDiscoveryLib.Nodes;
//using NeighborDiscoveryLib.Utils;
//using NeighborDiscoveryLib;
//using System.Diagnostics;
//using NeighborDiscoveryLib.Networks;
//using NeighborDiscoveryLib.Environment;
//using System.IO;
//using System.Threading;

//namespace UIConsole
//{
//    class Program
//    {
//        static void Main(string[] args)
//        {
//            string fileName = "testCases.txt";
//            //Task t = new Task(() => GenerateTestCases("testCases.txt", 1000, 20, 20, 100, 100, 200, new int[] { 5 }));
//            GenerateTestCases(fileName, 10, 20, 20, 100, 100, 20, new int[] { 5 });

//            NodeType type = NodeType.Disco;
//            var reader = new NetworkGenerator();
//            IEnumerable<Network2D> networks;
//            Task t1 = Task.Run(() => { 
//                networks = reader.CreateFromFile(fileName, type);
//                RunMultipleExperiment(networks.ToList(), type);
//            });
            
//            NodeType type2 = NodeType.UConnect;
//            var reader2 = new NetworkGenerator();
//            IEnumerable<Network2D> networks2;
//            Task t2 = Task.Run(() => {
//                networks2 = reader2.CreateFromFile(fileName, type2);
//                RunMultipleExperiment(networks2.ToList(), type2);
//            });

//            NodeType type3 = NodeType.SearchlightR;
//            var reader3 = new NetworkGenerator();
//            IEnumerable<Network2D> networks3;
//            Task t3 = Task.Run(() => {
//                networks3 = reader3.CreateFromFile(fileName, type3);
//                RunMultipleExperiment(networks3.ToList(), type3);
//            });
//            Console.WriteLine("Console running on thread: " + Thread.CurrentThread.ManagedThreadId);


//            Console.WriteLine("-----------------------------------------------------");
//            Console.WriteLine("Running Simulation...");

//            Task all = Task.Run(() => {
//                Task.WaitAll(new Task[] { t1, t2, t3 });
//                Console.WriteLine("All done");
//            });

//            Console.WriteLine("Al task running...");
//            Console.ReadLine();
//            //type = NodeType.SearchlightR;
//            //networks = reader.CreateFromFile(fileName, type);
//            //RunSingleExperiment(networks.FirstOrDefault(), type);
//            //RunMultipleExperiment(networks, type);

//            //type = NodeType.Birthday;
//            //networks = reader.CreateFromFile(fileName, type);
//            //RunSingleExperiment(networks.FirstOrDefault(), type);
//            //RunMultipleExperiment(networks, type);


//        }

//        static void RunSingleExperiment(Network2D network, NodeType type)
//        {
//            Console.WriteLine("-----------------------------------------------------");
//            Console.WriteLine(type.ToString() + " network.");
//            var environment = new NeighborDiscoveryEnvironment();
//            Console.WriteLine("Running Simulation...");
//            string cycles = "Running at";
//            foreach (var c in network.GetDutyCycles())
//            {
//                cycles += ", " + Math.Round(c * 100, 2) + "%";
//            }
//            Console.WriteLine(cycles);
//            var data = environment.RunSingleSimulation(network, type);

//            //foreach (var value in data.GetAllValues())
//            //{
//            //    Console.WriteLine("TimeSlot: " + value.TimeSlot + " " + value.NeighborsDiscovered);
//            //}
//            Console.WriteLine("Results:");
            
//            Console.WriteLine("Fraction Of Discovery over 50%: " + data.GetAllStatisticInfo().Where(v => v.FractionOfDiscovery > 0.5).First().CumulativeDiscoveryLatency);
//            Console.WriteLine("Fraction Of Discovery over 90%: " + data.GetAllStatisticInfo().Where(v => v.FractionOfDiscovery > 0.9).First().CumulativeDiscoveryLatency);
//            Console.WriteLine("Latency: " + data.MaxLatency);
//            //Console.WriteLine("Neighbors Discovered: " + value.NeighborsDiscovered);
//            //Console.WriteLine("Total Number of Wake Ups: " + value.WakesUpMade);
            

//            //Console.WriteLine("Communication Efficiency: " + Math.Round(value.CommunicationEfficiency, 2)+"%");

//            Console.WriteLine("-----------------------------------------------------");
//        }

//        static void RunMultipleExperiment(IEnumerable<Network2D> networks, NodeType type)
//        {
//            var environment = new NeighborDiscoveryEnvironment();
            
//            var result = environment.RunMultipleSimulations(networks, type);
            
//            //var result = task.Result;
            
//            Console.WriteLine(type + " results:");
//            int cnt = 0;
//            foreach (var avgLatency in result.GetAverageFractionOfDiscoveryByTimeSlot())
//            {
//                if (cnt % 10 == 0)
//                    Console.Write(" " + cnt + "% : " + avgLatency);
//                cnt++;
//            }
//            Console.WriteLine();
//            //Console.WriteLine("Neighbors Discovered: " + value.NeighborsDiscovered);
//            //Console.WriteLine("Total Number of Wake Ups: " + value.WakesUpMade);


//            //Console.WriteLine("Communication Efficiency: " + Math.Round(value.CommunicationEfficiency, 2)+"%");

//            Console.WriteLine("-----------------------------------------------------");
//        }
//        static void GenerateTestCases(string fileName, int numberOfCases, int networkSize, int globalCommunicationRange, int maxX, int maxY, int startUpTimeSlotLimit, int[] dutyCycles)
//        {
//            Console.WriteLine("Generating Test Cases...");
//            var sw = new StreamWriter(fileName);
//            Random r = new Random();
//            for (int i = 0; i < numberOfCases; i++)
//            {
//                int nsize = r.Next(30);
//                int stlimit = r.Next(nsize * 100);
//                int gcrange = r.Next(10);
//                NetworkGenerator.Generate2DNetworkToFile(sw, networkSize + nsize, startUpTimeSlotLimit + stlimit, maxX, maxY, globalCommunicationRange+gcrange, dutyCycles);
//            }

//            sw.Close();
//            Console.WriteLine("Test Cases Done.");
//        }

        
//    }
//}
