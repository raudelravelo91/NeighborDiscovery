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
            var row = 2;//0 based
            var column = 0;//0 based
            var m = 4;

            var v = PeriodValues(row, column, m);
            foreach (var item in v)
            {
                Console.Write(item + " ");
            }

        }

        public static int[] PeriodValues(int row, int column, int m)
        {
            var period = new List<int>();
            var i = row*m;
            var end = (row + 1) * m;
            while (i < end)
            {
                period.Add(i);//adding the row values
                i++;
            }

            i = column;
            var k = 0;
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
