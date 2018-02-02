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
            var p = ComfortableNumbers(10, 12);
            Console.WriteLine(p);
        }

        static int ComfortableNumbers(int L, int R) {
            //precomputar la suma de los digitos hasta el maximo N posible
            int maxN = 1000;
            _digitSum = new int[maxN + 1];
            for(int i = 1; i <= maxN; i++)//este loop es nlog(n) pq GetDigitSum(int n) es log(n)
            {
                _digitSum[i] = GetDigitSum(i);
            }
    
            //aqui viene la otra parte que tambien es nlog(n)
            int pairs = 0;
            for (int i = L; i <= R; i++)//este loop es O(n), pero miremos lo de adentro a ver si es log(n)
            {
                int currentDigitSum = _digitSum[i];
                for(int j = i - 1; j >= L && j >= i - currentDigitSum; j--)//este loop va por los numeros entre i y (i - digitSum[i])
                    if (FeelsComfortable(j, i))//y digitSum[i] es O(log(n)) pq digitSum[i] < 10*log(i)
                        pairs++;
            }

            return pairs;
        }
        
        static int[] _digitSum;

        static bool FeelsComfortable(int a, int b)//si a is comfortable with b
        {
            return a - _digitSum[a] <= b && b <= a + _digitSum[a];
        }

        static int GetDigitSum(int n)//devuelve la suma de los digitos
        {
            int sum = 0;
            while(n > 0)
            {
                sum += n%10;
                n/=10;
            }
            return sum;
        }
    }
}