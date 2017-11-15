using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace UIConsole
{
    public class SortedListTests
    {
        /// <summary>
        /// TimeSlot difference between inserting a list of sorted elemnts and then the same list unsorted.
        /// </summary>
        public static void Test1(int numberOfElements)
        {
            Stopwatch clock = new Stopwatch();

            SortedList<int, int> sortedListO = new SortedList<int, int>();
            SortedList<int, int> sortedListR = new SortedList<int, int>();
            Shuffle r = new Shuffle(numberOfElements);
            int[] array = new int[numberOfElements];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = i;
            }

            clock.Start();
            for (int i = 0; i < array.Length; i++)
            {
                sortedListO.Add(array[i], array[i]);
            }
            clock.Stop();
            Console.WriteLine("Sorted: " + clock.ElapsedMilliseconds);

            array = r.RandomSubSet(numberOfElements);

            clock.Restart();
            for (int i = 0; i < array.Length; i++)
            {
                sortedListR.Add(array[i], array[i]);
            }
            clock.Stop();
            Console.WriteLine("Unsorted: " + clock.ElapsedMilliseconds);
        }
            

    }
}
