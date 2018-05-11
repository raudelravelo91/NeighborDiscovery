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
    public sealed class AccBalancedNihaoExtended : AccProtocol
    {
        public int N { get; set; }// M = N AND N => 2N
        public override int Bound => N * N;
        public override int T => N * N;
        private double DesiredDutyCycle { get; set; }
        private bool[,] _listeningSchedule;
        private bool[,] _transmitSchedule;
        private readonly int[,] _awakeDevices;
        private readonly HashSet<IDiscoveryProtocol> _neighborsAwaiting;

        public AccBalancedNihaoExtended(int id, double dutyCyclePercentage) : base(id)
        {
            SetDutyCycle(dutyCyclePercentage);
            _listeningSchedule = new bool[N, N];
            _transmitSchedule = new bool[N, N];
            _awakeDevices = new int[N, N];
            _neighborsAwaiting = new HashSet<IDiscoveryProtocol>();
            GenerateListenningSchedule();
            GenerateLTransmissionsSchedule();
        }

        public override IDiscoveryProtocol Clone()
        {
            return new AccBalancedNihaoExtended(Id, GetDutyCycle());
        }

        public override string ToString()
        {
            return "Id: " + Id + " DC: " + DesiredDutyCycle + " AccBalancedNihaoExtended (" + N + ")";
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
            return IsRegularTransmission() || IsExtraTransmission();
        }

        private bool IsRegularTransmission()
        {
            int slot = InternalTimeSlot % T;
            int row = slot / N;
            int col = slot % N;

            return _transmitSchedule[row, col];
        }

        private bool IsExtraTransmission()
        {
            return _neighborsAwaiting.Any(node => node.IsListening());
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

        public override void MoveNext(int slot = 1)
        {
            //if (slot < 0)
            //    throw new Exception("The Device can not move a negative number of slots");
            //if (slot == 0)
            //    return;
            
            if (IsListening())
            {
                NumberOfListenedSlots++;
            }
            //if (IsTransmitting())
            //{
            //    NumberOfTransmissions++;
            //}
            var cnt = _neighborsAwaiting.RemoveWhere(node => node.ContainsNeighbor(this));//remove awaiting neighbors that already listened to you

            InternalTimeSlot++;
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
            _listeningSchedule = new bool[N, N];
            int col = 0;
            for (int row = 0; row < N; row++, col++)
            {
                if (col == (N / 2 - 1))
                {
                    _listeningSchedule[row, col+1] = true;
                }

                col %= (N / 2);

                _listeningSchedule[row, col] = true;
            }
        }
        
        private void GenerateLTransmissionsSchedule()
        {
            _transmitSchedule = new bool[N, N];
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
