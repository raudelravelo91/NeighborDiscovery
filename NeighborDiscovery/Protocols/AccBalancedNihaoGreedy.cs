using NeighborDiscovery.Environment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Protocols
{
    public class AccBalancedNihaoGreedy : AccProtocol
    {
        public int N { get; set; }// M = N AND N => 2N
        public override int Bound => 2 * N * N;
        public override int T => 2 * N * N;
        private double DesiredDutyCycle { get; set; }
        private bool[,] _listeningSchedule;
        private int _nextAccSlot;
        
        public AccBalancedNihaoGreedy(int id, double dutyCyclePercentage) : base(id)
        {
            _listeningSchedule = new bool[2*N,N];
            GenerateListenningSchedule();
            _nextAccSlot = -1;
        }

        public void  GenerateListenningSchedule()
        {
            var slots = new int[N];
            for (var i = 0; i < N; i++)
            {
                slots[i] = i;
            }
            var shuffle = new Shuffle(N);
            shuffle.KnuthShuffle(slots);

            _listeningSchedule = new bool[2*N,N];
            for (int i = 0; i < 2 * N; i += 2)
                _listeningSchedule[i, slots[i / 2]] = true;
        }

        public override IDiscoveryProtocol Clone()
        {
            return new AccBalancedNihaoGreedy(Id, GetDutyCycle());
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " AccBalancedNihaoGreedy (" + N + ")";
        }

        public override bool IsListening()
        {
            int slot = InternalTimeSlot % T;
            int row = slot / N;
            int col = slot % N;

            if (row % 2 == 0)
                return _listeningSchedule[row, col];
            
            //ACC in action here
            if (InternalTimeSlot == _nextAccSlot)
                return true;

            return false;//todo
        }

        public override bool IsTransmitting()
        {
            return InternalTimeSlot % N == 0;
        }

        public override double GetDutyCycle()
        {
            return DesiredDutyCycle;
        }

        public override void SetDutyCycle(double value)
        {
            switch ((int)value)
            {
                case 1:
                    N = 100;
                    break;
                case 2:
                    N = 50;
                    break;
                case 5:
                    N = 20;
                    break;
                case 10:
                    N = 10;
                    break;
                default:
                    throw new Exception("duty cycle not possible to set.");
            }
            DesiredDutyCycle = value;
        }

        protected override double SlotGain(int slot)
        {
            double value = 0;
            foreach (var neihbor in Neighbors2Hop())
            {
                //todo => if neighbor is transmitting at slot
                value += 1;
            }

            return value;
        }

        public override void ListenTo(ITransmission transmission)
        {
            if (!IsListening())
                return;

            if (!ContainsNeighbor(transmission.Sender))
            {
                AddNeighbor(transmission.Sender);
                if (ContainsNeighbor2Hop(transmission.Sender))
                {
                    RemoveNeighbor2Hop(transmission.Sender);
                }
            }
            //else todo => Update ContactInfo
        }

    }
}
