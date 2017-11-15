using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using NeighborDiscovery.Nodes;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Networks
{
    public class NetworkGenerator
    {
        public static void Generate2DNetworkToFile(StreamWriter fileWriter, int size, int startUpTimeSlotLimit, int maxX, int maxY, int globalRange, double[] possibleDutyCycles, int gotInRangeLimit)
        {
            Random random = new Random();

            fileWriter.WriteLine("Network Size");
            fileWriter.WriteLine(size);
            
            fileWriter.WriteLine("StartUp TimeSlots");
            int[] startUpTimes = new int[size];
            for (int i = 0; i < size-1; i++)
            {
                startUpTimes[i] = random.Next(startUpTimeSlotLimit);
                fileWriter.Write(startUpTimes[i] + " ");
            }
            startUpTimes[size - 1] = random.Next(startUpTimeSlotLimit);
            fileWriter.WriteLine(startUpTimes[size - 1]);

            fileWriter.WriteLine("Duty Cycles");
            for (int i = 0; i < size - 1; i++)
            {
                fileWriter.Write(possibleDutyCycles[random.Next(possibleDutyCycles.Length)] + " ");
            }
            fileWriter.WriteLine(possibleDutyCycles[random.Next(possibleDutyCycles.Length)]);

            fileWriter.WriteLine("Nodes Ids");
            for (int i = 0; i < size; i++)
            {
                double x = random.NextDouble() * maxX;
                double y = random.NextDouble() * maxY;
                fileWriter.WriteLine(i + " " + x + " " + y + " " + globalRange);// id, x, y, range
            }

            fileWriter.WriteLine("Neighbors Got In Range Values");
            int[,] values = new int[size, size];
            for (int i = 0; i < size; i++)
                for (int j = i+1; j < size; j++)
                    values[i, j] = values[j, i] = Math.Max(startUpTimes[i], startUpTimes[j]) + random.Next(gotInRangeLimit);

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size - 1; j++)
                    fileWriter.Write(values[i, j] + " ");
                fileWriter.WriteLine(values[i,size-1]);
            }

        }

        public static bool GenerateTestCasesToFile(string fileName, int numberOfTests, int startUpLimit, int posRange, int networkSize, int minCRange, int maxCRange, double[] duties, int gotInRangeLimit)
        {
            var sw = new StreamWriter(fileName);
            Random random = new Random();
            for (int i = 0; i < numberOfTests; i++)
            {
                int gCommRange = minCRange + random.Next(maxCRange - minCRange);//communication range
                Generate2DNetworkToFile(sw, networkSize, startUpLimit, posRange, posRange, gCommRange, duties, gotInRangeLimit);
            }
            sw.Close();
            return true;
        }

        public IEnumerable<Network2D> CreateFromFile(string fileName, Func<BasicParameters, Node> nodeFactory)
        {
            //Node Id and Node index (inside the Network) use the same value, 
            //warning: the code below is based on that constrain
            using (StreamReader sr = new StreamReader(fileName))
            {
                while (sr.ReadLine() != null)
                {
                    Network2D network = null;
                    //sr.ReadLine();//node size
                    int size = int.Parse(sr.ReadLine());
                    sr.ReadLine();//startup TimeSlot
                    string[] startUpTimeSlotsString = sr.ReadLine().Split();
                    sr.ReadLine();//duty cycles
                    string[] dutyCycles = sr.ReadLine().Split();
                    int cnt = dutyCycles.Distinct().Count();
                    BasicParameters.numberOfDutyCyclesToHandle = cnt;
                    int[] startUpTimeSlots = startUpTimeSlotsString.Select(x => int.Parse(x)).ToArray();
                    int[,] gotInRange = new int[size, size];
                    network = new Network2D(size, gotInRange);
                    sr.ReadLine();//nodes ids
                    for (int i = 0; i < size; i++)
                    {
                        string[] line = sr.ReadLine().Split();
                        double x = double.Parse(line[1]);
                        double y = double.Parse(line[2]);
                        int range = int.Parse(line[3]);
                        int dutyCycle = int.Parse(dutyCycles[i]);
                        network.AddNode(nodeFactory(new BasicParameters(i, dutyCycle, range, startUpTimeSlots[i])), x, y);
                    }
                    sr.ReadLine();//got in range header
                    for (int i = 0; i < size; i++)
                    {
                        int[] line = sr.ReadLine().Split().Select(x => int.Parse(x)).ToArray();
                        for (int j = 0; j < line.Length; j++)
                            gotInRange[i, j] = line[j];

                    }

                    network.BuildNetwork(PercentageToFix);
                    yield return network;
                }
            }
        }

        private static double fixAsymmetricCase = 0.1;

        public static double PercentageToFix { get { return fixAsymmetricCase; } set { fixAsymmetricCase = value; } }
    }
}
