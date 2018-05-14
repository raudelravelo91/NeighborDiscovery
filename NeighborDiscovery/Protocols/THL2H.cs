using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Protocols
{
    public sealed class THL2H : AccProtocol
    {
        public int N { get; private set; }
        public int M { get; private set; }
        public override int Bound => N * M;
        public override int T => N * M;
        private double DesiredDutyCycle { get; set; }
        private bool[,] _listeningSchedule;
        private bool[,] _transmitSchedule;
        private readonly int[,] _awakeDevices;
        private readonly HashSet<IDiscoveryProtocol> _neighborsAwaiting;
        public int NumberOfListeningSlots { get; set; }

        public THL2H(int id, double dutyCyclePercentage, int m) : base(id)
        {
            M = m;
            SetDutyCycle(dutyCyclePercentage);
            _listeningSchedule = new bool[N, M];
            _transmitSchedule = new bool[N, M];
            _awakeDevices = new int[N, M];
            _neighborsAwaiting = new HashSet<IDiscoveryProtocol>();
            GenerateListenningSchedule();
            GenerateLTransmissionsSchedule();
        }

        public override IDiscoveryProtocol Clone()
        {
            return new THL2H(Id, GetDutyCycle(), M);
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " THL2H (" + N +  "," + M + ")";
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
            if (IsManualTransmission() && _neighborsAwaiting.Count > 0)
            {
                //check if there are neighbors awaiting listening
                return _neighborsAwaiting.Any(node => node.IsListening());
            }

            int slot = InternalTimeSlot % T;
            int row = slot / M;
            int col = slot % M;

            return _transmitSchedule[row, col];
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

        public override void MoveNext()
        {
            if (IsListening())
            {
               NumberOfListenedSlots++;
            }
            var cnt = _neighborsAwaiting.RemoveWhere(node => node.ContainsNeighbor(this));//remove awaiting neighbors that already listened to you
            InternalTimeSlot ++;//if slot > 1 then the property ProtocolListenedSlots may not give a correct value.
        }

        protected override double SlotGain(int slot)
        {
            int schedulePos = slot % T;
            int row = schedulePos / N;
            int col = schedulePos % N;
            return _awakeDevices[row, col];
        }

        public override void ListenTo(ITransmission transmission)
        {
            if (!IsListening())
                return;
            var newDiscoveries = new NodeResult();
            if (!ContainsNeighbor(transmission.Sender))
            {
                AddNeighbor(transmission.Sender);
                _neighborsAwaiting.Add(transmission.Sender); //adding new neighbor as a direct one
                newDiscoveries.AddDiscovery(GetContactInfo(transmission.Sender));

                //this part is when he already listened to you before
                //ACTIVATE ONLY WHEN TRANSMISSIONS COMES WITH EXTRA INFO
                //CheckIfNeedsAnswer(transmission.Sender);
            }
            else//update last contact => key updated to get more information
                NeighborsDiscovered[transmission.Sender].Update(InternalTimeSlot);

            if (newDiscoveries.Count > 0)
                RaiseOnDeviceDiscovered(newDiscoveries);
        }

        #region private methods
        private bool IsManualTransmission()
        {
            int schedulePos = InternalTimeSlot % T;
            //  int row = schedulePos / N;
            int col = schedulePos % M;
            return col == M/2;
        }

        private void CheckIfNeedsAnswer(IDiscoveryProtocol neighbor)
        {
            if (neighbor.ContainsNeighbor(this) && _neighborsAwaiting.Contains(neighbor))
            {
                _neighborsAwaiting.Remove(neighbor);
            }
        }

        private void RemoveFromAwaiting(IDiscoveryProtocol neighbor)
        {
            if (_neighborsAwaiting.Contains(neighbor))
            {
                _neighborsAwaiting.Remove(neighbor);
            }
        }

        private bool IsAwaiting(IDiscoveryProtocol neighbor)
        {
            return _neighborsAwaiting.Contains(neighbor);
        }

        private void UpdateSlot(int slot, int value)
        {
            int schedulePos = slot % T;
            int row = schedulePos / N;
            int col = schedulePos % N;
            _awakeDevices[row, col] += value;
        }

        private void GenerateListenningSchedule()
        {
            _listeningSchedule = new bool[N, M];
            int row = 0;
            for (int col = 0; col <= M / 2; col++)
            {
                _listeningSchedule[row, col] = true;
                NumberOfListeningSlots++;
            }
            

            row = N / 2;//the middle row
            for (int col = 0; col <= M / 2; col++)
            {
                _listeningSchedule[row, col] = true;
                NumberOfListeningSlots++;
            }
        }

        private void GenerateLTransmissionsSchedule()
        {
            _transmitSchedule = new bool[N, M];
            for (int i = 0; i < N; i++)
            {
                int row = i;
                int col = 0;
                _transmitSchedule[row, col] = true;
            }
        }
        #endregion
    }
}
