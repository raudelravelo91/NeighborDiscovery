using System;
using System.Collections.Generic;

namespace SuperPrimeRib
{
    class Program
    {
        static void Main(string[] args)
        {
            Randy();
            Console.WriteLine("----------------------");
            Raude();
        }

        public static string resultado = "0";

        public static void PrimeRib(int n, int pos)
        {
            if (pos == n)
            {
                if (EsPrimo(int.Parse(resultado)))
                    Console.WriteLine(resultado.Substring(1, resultado.Length - 1));
            }
            else
            {
                for (int i = 1; i <= 9; i++)
                    if (EsPrimo(int.Parse(resultado + i)))
                    {
                        resultado += i;
                        PrimeRib(n, pos + 1);
                        resultado = resultado.Substring(0, resultado.Length - 1);
                    }
            }
        }

        public static void Raude()
        {
            string s;
            while ((s = Console.ReadLine()) != null)
            { resultado = "0"; PrimeRib(int.Parse(s), 0); }
        }

        public static void Randy()
        {
            int cifras =int.Parse(Console.ReadLine());

            List<int> r =new List<int>{3,5,7};

            for (int i = 1; i < cifras; i++)
            {
                r = CreaSuperPrimos(r);
            }                    

            for (int i = 0; i < r.Count; i++)
            {
                Console.WriteLine(r[i].ToString());
            }
        }

        public  static List<int> CreaSuperPrimos(List<int> SuperPrimosMenores)
        {
            List<int> r = new List<int>();

            int t;
            for (int i = 0; i < SuperPrimosMenores.Count; i++)
            {
                for (int j = 1; j <= 9; j++)
                {
                    t = SuperPrimosMenores[i] * 10 + j;
                    if (EsPrimo(t))
                    {
                        r.Add(t);
                    }
                }
            }

            return r;
        }

        public static bool EsPrimo(int n)
        {
            if (n < 2 || n % 2 == 0) 
                return false;
            for (int i = 3; i * i <= n; i += 2)
                if (n % i == 0)
                    return false;
            return true;
        }
    }
}