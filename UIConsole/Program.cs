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
            var sequence = new[] {1, 2, 3, 4, 5};
            var startElement = 2;
            var endElement = 4;
            
            
            int startIndex = Array.IndexOf(sequence, startElement);
            int endIndex = Array.LastIndexOf(sequence, endElement);




            //var range = sequence.GetRange(startIndex, 1 + endIndex - startIndex);

        }
    }
}