using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIConsole
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //code here
            int row = 2;//0 based
            int column = 0;//0 based
            int m = 4;

            var v = PeriodValues(row, column, m);
            foreach (var item in v)
            {
                Console.Write(item + " ");
            }

        }

        public static int[] PeriodValues(int row, int column, int m)
        {
            List<int> period = new List<int>();
            int i = row*m;
            int end = (row + 1) * m;
            while (i < end)
            {
                period.Add(i);//adding the row values
                i++;
            }

            i = column;
            int k = 0;
            while (k < m)
            {
                period.Add(i);//adding the column values
                i += m;
                if (k == row)
                {
                    period.RemoveAt(period.Count - 1);
                }
                k++;
            }

            period.Sort();

            return period.ToArray();

        }
    }
}
