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
            Console.WriteLine(" --------------------------------------------------------------");
            PrintResult("TestMoveNextAccGreedy", TestingAManoLOL.TestMoveNextAccGreedy());
            PrintResult("TestIsListeningAccGreedy", TestingAManoLOL.TestIsListeningAccGreedy());
            PrintResult("IsUnderlyingScheduleCorrectAccGreedy", TestingAManoLOL.IsUnderlyingScheduleCorrectAccGreedy());
            PrintResult("IsPickingTheCorrectSlotAccGreedy", TestingAManoLOL.IsPickingTheCorrectSlotAccGreedy());
            
            Console.WriteLine(" --------------------------------------------------------------");
        }
        
        static void PrintResult(string testName, bool result)
        {
            if(result)
            {
                Console.Write("|");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("Passed: ");
            }
            else
            {
                Console.Write("|");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Failed: ");
            }
            Console.ResetColor();
            Console.WriteLine(testName);
        }
    }
}