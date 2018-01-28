using System;
using System.Collections.Generic;

namespace SuperPrimeRib
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(subsetGcd(37,6));
        }

        static int subsetGcd(int n, int g) {
            int mod = 1000000007;
            long[] pow = new long[100001];
            pow[0] = 1;
            for (int i = 1; i < pow.Length; i++)
                pow[i] = (pow[i - 1] * 2) % mod;
    
            int mults = n/g;
            long total = getPowMinusOne(pow, mod, mults);
            mults--;
            for(int i = 2*g; i <= n; i += g)//for every mult
            {
                long withi = getPowMinusOne(pow, mod, n / i);
                long minus = (withi - (n/i - 1) + mod) % mod;
                total = ((total - minus + mod) % mod);
                mults--;
            }
            
            return (int)total;
        }

        static long getPowMinusOne(long[] pow, int mod, int pos)
        {
            return (pow[pos] - 1 + mod) % mod;
        }


    }
}