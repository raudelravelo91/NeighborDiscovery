using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;

namespace UIConsole
{
    public class TestingAManoLOL
    {
        public static bool TestMoveNextAccGreedy()
        {
            var node = new AccBalancedNihaoGreedy(0, 10);
            int its = node.InternalTimeSlot;
            int move = 5;
            node.MoveNext(move);
            return its == node.InternalTimeSlot - move;
        }

        public static bool TestIsListeningAccGreedy()
        {
            var node = new AccBalancedNihaoGreedy(0, 10);
            int cnt = 0;
            for(int i = 0; i < node.T; i++, node.MoveNext())
                if(node.IsListening())
                {
                    //Console.Write(node.InternalTimeSlot + " ");
                    cnt++; 
                }
            Console.WriteLine();
            double value = cnt*100/node.T;
            return value == 10;
        }


    }
}
