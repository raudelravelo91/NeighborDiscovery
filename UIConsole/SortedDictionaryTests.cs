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
            var clock = new Stopwatch();
            var dic = new SortedDictionary<int, int>();

            for (var i = 0; i < numberOfElements; i++)
            {
                dic.Add(i, i);
            }

            clock.Start();
            for (var i = 0; i < numberOfElements; i++)
            {
                var key = i;
                var element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

            for (var i = 0; i < numberOfElements; i++)
            {
                dic.Add(i, i);
            }

            clock.Restart();
            for (var i = 0; i < numberOfElements; i++)
            {
                var key = dic.Keys.First();
                var element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

        }

        public static void Test2(int numberOfElements)
        {
            var clock = new Stopwatch();
            var dic = new SortedDictionary<int, int>();
            var r = new Shuffle(numberOfElements);
            var array = new int[numberOfElements];
            array = r.RandomSubSet(numberOfElements);

            for (var i = 0; i < numberOfElements; i++)
            {
                dic.Add(array[i], array[i]);
            }

            clock.Start();
            for (var i = 0; i < numberOfElements; i++)
            {
                var key = i;
                var element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

            for (var i = 0; i < numberOfElements; i++)
            {
                dic.Add(array[i], array[i]);
            }

            clock.Restart();
            for (var i = 0; i < numberOfElements; i++)
            {
                var key = dic.Keys.First();
                var element = dic[key];
                dic.Remove(key);
            }
            Console.WriteLine(clock.ElapsedMilliseconds);

        }

    }
}
