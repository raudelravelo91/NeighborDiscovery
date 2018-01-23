using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.DataGeneration
{
    public class TestCasesGenerator
    {
        public TestCasesGenerator()
        {

        }

        private void GenerateTest(StreamWriter wr, int networkSize, int startUpLimit, int posRange, int comRange, int[] duties)
        {
            wr.WriteLine(networkSize);
            for (int i = 0; i < networkSize; i++)
            {
                //id, startUp, posX, posY, comRange, duty
                var startUp = RandomGenerator.GetRandomInteger(startUpLimit);
                var posX = RandomGenerator.GetRandomDouble() * posRange;
                var posY = RandomGenerator.GetRandomDouble() * posRange;
                var duty = duties[RandomGenerator.GetRandomInteger(duties.Length)];
                wr.WriteLine($"{i},{startUp},{posX},{posY},{comRange},{duty}");
            }
        }

        private void GenerateTestCases(string fileName, int numberOfTests, int networkSize, int startUpLimit, int posRange,
            int comRange, int[] duties)
        {
            using (StreamWriter sw = new StreamWriter(fileName))
            {
                for (int i = 0; i < numberOfTests; i++)
                {
                    GenerateTest(sw, networkSize, startUpLimit, posRange, comRange, duties);
                }
            }
        }

        private IEnumerable<DeviceData> GenerateDeviceData(int networkSize, int startUpLimit, int posRange, int comRange, double[] duties)
        {
            for (int i = 0; i < networkSize; i++)
            {
                //id, startUp, posX, posY, comRange, duty
                var startUp = RandomGenerator.GetRandomInteger(startUpLimit);
                var posX = RandomGenerator.GetRandomDouble() * posRange;
                var posY = RandomGenerator.GetRandomDouble() * posRange;
                var duty = duties[RandomGenerator.GetRandomInteger(duties.Length)];
                yield return new DeviceData(i,startUp, (int)duty, new MyPair(posX, posY), comRange);
            }
        }

        public TestCase GenerateTestCase(int networkSize, int startUpLimit, int posRange, int comRange, double[] duties)
        {
            var data = GenerateDeviceData(networkSize, startUpLimit, posRange, comRange, duties);
            var testCase = new TestCase(data);
            return testCase;
        }

        public TestSuite GenerateTestSuite(int numberOfTests, int networkSize, int startUpLimit, int posRange, int minComRange,
            int maxComRange, double[] duties)
        {
            List<TestCase> tests = new List<TestCase>();
            for (int i = 0; i < numberOfTests; i++)
            {
                int testStartUpLimit = RandomGenerator.GetRandomInteger(startUpLimit);
                int testCommRange = minComRange + RandomGenerator.GetRandomInteger(maxComRange - minComRange + 1);
                var test = GenerateTestCase(networkSize, testStartUpLimit, posRange, testCommRange, duties);
                tests.Add(test);
            }
            return new TestSuite(tests);
        }

        public bool SaveTestSuite(string fileName, TestSuite suite)
        {
            BinaryFormatter bf = new BinaryFormatter();  
  
            FileStream fsout = new FileStream(fileName, FileMode.Create, FileAccess.Write, FileShare.None);  
            try  
            {  
                using (fsout)  
                {  
                    bf.Serialize(fsout, suite);  
                }  
            }  
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }
            fsout.Close();
            return true;
        }

        public TestSuite LoadTestSuite(string fileName)
        {
            BinaryFormatter bf = new BinaryFormatter();  
  
            FileStream fsin = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.None);  
            try
            {
                TestSuite suite;
                using (fsin)  
                {  
                    suite = (TestSuite) bf.Deserialize(fsin);
                } 
                return suite;
            }  
            catch(Exception e)
            {  
                Console.WriteLine(e.ToString());
                throw new Exception("An error ocurred when trying to load the data.");
            }
            
        }

    }
}
