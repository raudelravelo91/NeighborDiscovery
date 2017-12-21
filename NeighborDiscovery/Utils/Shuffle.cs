using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Utils
{
    public class Shuffle
    {
        public static Random random= new Random();
        private bool[] dic;
        public int MaxSize { get; private set; }
        

        public Shuffle(int maxSize)
        {
            MaxSize = maxSize;
            //random = new Random();
            dic = new bool[MaxSize];
        }

        public int[] RandomSubSet(int subsetSize)
        {
            var permutation = new int[MaxSize];//contains numbers from 1 to MaxSize
            for (var i = 0; i < MaxSize; i++)
                permutation[i] = i + 1;
            KnuthShuffle(permutation);
            var answer = new int[subsetSize];
            for (var i = 0; i < subsetSize; i++)
            {
                answer[i] = permutation[i];
            }
            return answer;
        }

        public void KnuthShuffle(int[] permutation)
        {
            var n = permutation.Length;
            
            for (var i = 0; i <= n - 2; i++)
            {
                var j = random.Next(n - i); /* A random integer such that 0 ≤ j < n-i*/
                Swap(ref permutation[i], ref permutation[i + j]);   /* Swap an existing element [i+j] to position [i] */
            }
        }

        public int[] RandomShuffle(int subsetSize)
        {
            var answer = new int[subsetSize];
            var cnt = 0;
            var next = 0;
            while (cnt < answer.Length)
            {
                next = random.Next(subsetSize);
                if (!dic[next])
                {
                    dic[next] = true;
                    answer[cnt] = next;
                    cnt++;
                }
            }
            ClearDic(answer);
            return answer;
        }

        private void ClearDic(int[] numbers)
        {
            for (var i = 0; i < numbers.Length; i++)
            {
                dic[numbers[i]] = false;
            }
        }

        private void Swap(ref int v1, ref int v2)
        {
            var temp = v1;
            v1 = v2;
            v2 = temp;
        }
    }
}
