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
            var node = new AccGreedyBalancedNihao(0, 10);
            int its = node.InternalTimeSlot;
            int move = 5;
            node.MoveNext(move);
            return its == node.InternalTimeSlot - move;
        }

        public static bool TestIsListeningAccGreedy()
        {
            var node = new AccGreedyBalancedNihao(0, 10);
            int cnt = 0;
            for(int i = 0; i < node.T; i++, node.MoveNext())
                if(node.IsListening())
                {
                    //Console.Write(node.InternalTimeSlot + " ");
                    cnt++; 
                }
            //Console.WriteLine();
            double value = cnt*100/node.T;
            return value == 10;
        }

        public static bool IsPickingTheCorrectSlotAccGreedy()
        {
            var node0 = new AccGreedyBalancedNihao(0, 10);



            var node1 = new BalancedNihao(1, 10);
            node1.MoveNext(1);
            var node2 = new BalancedNihao(2, 10);
            node2.MoveNext(2);
            var node3 = new BalancedNihao(3, 10);
            node3.MoveNext(3);
            var node4 = new BalancedNihao(4, 10);
            node4.MoveNext(3);

            node1.NeighborsDiscovered.Add(node2, new ContactInfo(node2, node1.InternalTimeSlot));
            node1.NeighborsDiscovered.Add(node3, new ContactInfo(node3, node1.InternalTimeSlot));
            node1.NeighborsDiscovered.Add(node4, new ContactInfo(node4, node1.InternalTimeSlot));

            while(node0.NumberOfNeighbors == 0)
            {
                node0.MoveNext();
                node1.MoveNext();
                if(node1.IsTransmitting())
                    node0.ListenTo(node1.GetTransmission());

                if (node1.InternalTimeSlot > 1000)
                    return false;

            }
            
            while (node0.NumberOfNeighbors < 3)
            {
                node0.MoveNext();
                node2.MoveNext();
                node3.MoveNext();
                node4.MoveNext();
                
                if (node3.IsTransmitting() && node4.IsTransmitting())
                {
                    //if(node0.IsListening())
                    //    Console.WriteLine("debug");
                    //node0.ListenTo(node2.GetTransmission());
                    node0.ListenTo(node3.GetTransmission());
                    node0.ListenTo(node4.GetTransmission());
                }
                
                if(node0.InternalTimeSlot > 10000)
                    return false;

            }


            return true;
        }

        public static bool IsUnderlyingScheduleCorrectAccGreedy()
        {
            AccGreedyBalancedNihao node = new AccGreedyBalancedNihao(0,10);
            int cnt = 0;
            List<int> slots = new List<int>();
            bool[] mark = new bool[10];
            while(cnt < 20)
            { 
                if(node.IsListening())
                {
                    cnt++;
                    int slot = node.InternalTimeSlot % node.N;
                    if ((node.InternalTimeSlot / node.N) % 2 == 0)
                    {
                        slots.Add(slot);
                        mark[slot] = true;
                    }
                    else if(slot != 0)
                        return false;
                }
                node.MoveNext();
            }


            return !mark.Contains(false);
        }

    }
}
