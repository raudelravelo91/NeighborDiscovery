using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace UIConsole
{
    public class SortedDictionaryTests
    {
        public static void Test1(int numberOfElements)
        {
            Stopwatch clock = new Stopwatch();
            SortedDictionary<int, int> dic = new SortedDictionary<int, int>();

            for (int i = 0; i < numberOfElements; i++)
            {
                dic.Add(i, i);
            }

            clock.Start();
            for (int i = 0; i < numberOfElements; i++)
            {
                int key = i;
                int element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

            for (int i = 0; i < numberOfElements; i++)
            {
                dic.Add(i, i);
            }

            clock.Restart();
            for (int i = 0; i < numberOfElements; i++)
            {
                int key = dic.Keys.First();
                int element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

        }

        public static void Test2(int numberOfElements)
        {
            Stopwatch clock = new Stopwatch();
            SortedDictionary<int, int> dic = new SortedDictionary<int, int>();
            Shuffle r = new Shuffle(numberOfElements);
            int[] array = new int[numberOfElements];
            array = r.RandomSubSet(numberOfElements);

            for (int i = 0; i < numberOfElements; i++)
            {
                dic.Add(array[i], array[i]);
            }

            clock.Start();
            for (int i = 0; i < numberOfElements; i++)
            {
                int key = i;
                int element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

            for (int i = 0; i < numberOfElements; i++)
            {
                dic.Add(array[i], array[i]);
            }

            clock.Restart();
            for (int i = 0; i < numberOfElements; i++)
            {
                int key = dic.Keys.First();
                int element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

        }

    }
}
