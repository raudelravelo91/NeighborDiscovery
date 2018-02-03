using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

namespace UIConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            for (int i = 1; i <= 180; i++)
            {
                if(Test(i))
                    Console.WriteLine("X = " + i + " Y = " + (i+30));
            }
        }
        
        static bool Test(int x)
        {
            int y = x + 30;
            return (1800 / x) == (1800 / y) + 10 && 1800 % x == 0 && 1800 % y == 0;
        }
    }
}