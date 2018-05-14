using NeighborDiscovery.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public sealed class GNihao: BoundedProtocol
    {
        public int N { get; private set; }
        public int M { get; private set; }
        public override int Bound => N * M;
        public override int T => N * M;
        private double DesiredDutyCycle { get; set; }
        private bool[,] _listeningSchedule;

        public GNihao(int id, double dutyCyclePercentage, int m) : base(id)
        {
            M = m;
            SetDutyCycle(dutyCyclePercentage);
            _listeningSchedule = new bool[N, M];
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
            return new GNihao(Id, GetDutyCycle(), M);
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " GNihao (" + N + "," + M +")";
        }

        public override bool IsListening()
        {
            int slot = InternalTimeSlot % T;
            int row = slot / M;
            int col = slot % M;

            return _listeningSchedule[row, col];
        }

        public override bool IsTransmitting()
        {
            return InternalTimeSlot % M == 0;
        }

        private void GenerateListenningSchedule()
        {
            _listeningSchedule = new bool[N, M];
            for (int i = 0; i < M; i++)
            {
                _listeningSchedule[0, i] = true;
                //todo number of listeningSlots++   
            }
        }
    }
}
