using NeighborDiscovery.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public sealed class BalancedNihao: BoundedProtocol
    {
        public int N { get; set; }
        public override int Bound => N * N;
        public override int T => N * N;
        private double DesiredDutyCycle { get; set; }
        private bool[,] _listeningSchedule;

        public BalancedNihao(int id, double dutyCyclePercentage) : base(id)
        {
            SetDutyCycle(dutyCyclePercentage);
            _listeningSchedule = new bool[N, N];
            GenerateListenningSchedule();
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

        

        public override IDiscoveryProtocol Clone()
        {
            return new BalancedNihao(Id, GetDutyCycle());
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " BalancedNihao (" + N + ")";
        }

        public override bool IsListening()
        {
            int slot = InternalTimeSlot % T;
            int row = slot / N;
            int col = slot % N;

            return _listeningSchedule[row, col];
        }

        public override bool IsTransmitting()
        {
            return InternalTimeSlot % N == 0;
        }

        private void GenerateListenningSchedule()
        {
            var slots = new int[N];
            for (var i = 0; i < N; i++)
            {
                slots[i] = i;
            }
            //var shuffle = new Shuffle(N);
            //shuffle.KnuthShuffle(slots);

            _listeningSchedule = new bool[N, N];
            for (int i = 0; i < N; i ++)
                _listeningSchedule[i, slots[i]] = true;
        }
    }
}
