using NeighborDiscovery.Environment;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Utils;
using NeighborDiscovery.Statistics;

namespace NeighborDiscovery.Protocols
{
    /// <summary>
    /// This Acc Protocol is a greedy Acc that prioritize the listening in slots where the maximum number of 2-hop neighbors are transmitting
    /// </summary>
    public sealed class AccGreedyBalancedNihao : AccProtocol
    {
        public int N { get; set; }// M = N AND N => 2N
        public override int Bound => 2 * N * N;
        public override int T => 2 * N * N;
        private double DesiredDutyCycle { get; set; }
        private bool[,] _listeningSchedule;
        private int _nextAccSlot;
        private int _lastAccSlot;
        private readonly int[] _slotValue;
        private bool _slotsGainUpdatedNeeded;
        
        public AccGreedyBalancedNihao(int id, double dutyCyclePercentage) : base(id)
        {
            SetDutyCycle(dutyCyclePercentage);
            _listeningSchedule = new bool[2*N,N];
            GenerateListenningSchedule();
            _nextAccSlot = -1;
            _slotValue = new int[N];
            _lastAccSlot = -1;
            _slotsGainUpdatedNeeded = true;
            UpdateSlotsGain();
        }


        public override IDiscoveryProtocol Clone()
        {
            return new AccGreedyBalancedNihao(Id, GetDutyCycle());
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

            if (AccIsReadyToListenAgain())
                return col == _nextAccSlot;

            return _listeningSchedule[row, col];
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

        public override void MoveNext(int slot = 1)
        {
            if (slot < 0)
                throw new Exception("The Device can not move a negative number of slots");
            if (IsListening())
            {
                if (AccIsReadyToListenAgain())
                {
                    AccProtocolListenedSlots++;
                    _lastAccSlot = InternalTimeSlot;
                }
                else
                    NumberOfListenedSlots++;
            }

            if (IsAccSlot(InternalTimeSlot + 1) || _slotsGainUpdatedNeeded) //update slots gain, if needed
                UpdateSlotsGain();

            InternalTimeSlot += slot;
        }

        protected override double SlotGain(int slot)
        {
            int schedulePos = slot % T;
            int row = schedulePos / 2*N;
            int col = schedulePos % N;
            return _slotValue[col];
        }

        public override void ListenTo(ITransmission transmission)
        {
            if (!IsListening())
                return;
            var newDiscoveries = new NodeResult();
            _slotsGainUpdatedNeeded = false;
            if (!ContainsNeighbor(transmission.Sender))
            {
                AddNeighbor(transmission.Sender); //adding new neighbor as a direct one
                if (ContainsNeighbor2Hop(transmission.Sender))
                {
                    RemoveNeighbor2Hop(transmission.Sender); //removing the neighbor from 2-hop neighbors
                    _slotsGainUpdatedNeeded = true;
                }
                else //it's a completely new discovery
                {
                    newDiscoveries.AddDiscovery(GetContactInfo(transmission.Sender));
                }
            }
            else//update last contact => key updated to get more information
                NeighborsDiscovered[transmission.Sender].Update(InternalTimeSlot);

            foreach (var neighbor2Hop in Get2HopNeighborsFromDirectNeighbor(transmission.Sender)
            .Where(device => !ContainsNeighbor(device) && !ContainsNeighbor2Hop(device)))
            //adding new 2hop neighbors that may be discovered by the new information
            {
                    AddNeighbor2Hop(neighbor2Hop);
                    _slotsGainUpdatedNeeded = true;
                    newDiscoveries.AddDiscovery(GetContactInfoFor2Hop(neighbor2Hop));
            }


            if(newDiscoveries.Count > 0)
                RaiseOnDeviceDiscovered(newDiscoveries);
            
        }

        #region private methods
        private bool AccIsInCharge()
        {
            int slot = InternalTimeSlot % T;
            int row = slot / N;
            int col = slot % N;

            return row % 2 != 0;
        }

        //private void UpdateSlotsGain()
        //{
        //    if (IsAccSlot(InternalTimeSlot + 1))
        //    {
        //        ClearSlots();
        //        foreach (var neighbor2Hop in Neighbors2Hop())
        //        {
        //            int myT0 = InternalTimeSlot + 1;
        //            int myTn = LastAccSlotAfter(myT0);
        //            int t0 = neighbor2Hop.InternalTimeSlot + 1;
        //            int tn = t0 + (myTn - myT0);
        //            foreach (var neig2HopTran in GetDeviceNextTransmissionSlot(t0, tn, neighbor2Hop))
        //            {
        //                int transmitsIn = 1 + (neig2HopTran - t0);
        //                int slotToUpdate = InternalTimeSlot + transmitsIn;
        //                UpdateSlot(slotToUpdate);
        //            }
        //        }
        //        _nextAccSlot = GetBestSlot(InternalTimeSlot + 1);    
        //    }
        //}

        private void UpdateSlotsGain()
        {
            if (IsAccSlot(InternalTimeSlot + 1))
            {
                ClearSlots();

                foreach (var neighbor in Neighbors())
                {
                    UpdateViaDevice(neighbor);
                }

                //foreach (var neighbor2Hop in Neighbors2Hop())
                //{
                //    UpdateViaDevice(neighbor2Hop);
                //}

                _nextAccSlot = GetBestSlot(InternalTimeSlot + 1);
            }
        }

        private void UpdateViaDevice(IDiscoveryProtocol neighbor)
        {
            int myT0 = InternalTimeSlot + 1;
            int myTn = LastAccSlotAfter(myT0);
            int t0 = neighbor.InternalTimeSlot + 1;
            int tn = t0 + (myTn - myT0);
            foreach (var neig2HopTran in GetDeviceNextTransmissionSlot(t0, tn, neighbor))
            {
                int transmitsIn = 1 + (neig2HopTran - t0);
                int slotToUpdate = InternalTimeSlot + transmitsIn;
                UpdateSlot(slotToUpdate);
            }
        }

        private int LastAccSlotAfter(int t0)
        {
            while (IsAccSlot(t0))
                t0++;
            return t0-1;
        }

        private int GetBestSlot(int t0)
        {
            if(!IsAccSlot(t0))
                throw new Exception("The given slot is not within the correct range");
            int bestIndex = 0;
            while (IsAccSlot(t0))
            {
                int mod = t0 % T;
                int slotIndex = mod % N;
                if (_slotValue[slotIndex] > _slotValue[bestIndex])
                    bestIndex = slotIndex;
                t0++;
            }
            return Shuffle.random.Next(t0);
        }

        private void ClearSlots()
        {
            for (int i = 0; i < _slotValue.Length; i++)
            {
                _slotValue[i] = 0;
            }
        }

        private bool IsAccSlot(int t0)
        {
            int slot = t0 % T;
            int row = slot / N;
            int col = slot % N;

            if (row % 2 == 0 || col >= N)
                return false;

            return true;
        }

        private void UpdateSlot(int t0)
        {
            if(!IsAccSlot(t0))
                throw new Exception("The given slot is not within the correct range");
            int slot = t0 % T;
            int row = slot / N;
            int col = slot % N;
            _slotValue[col]++;
        }

        private bool AccIsReadyToListenAgain()
        {
            if(!AccIsInCharge())
                return false;

            if(_lastAccSlot < 0)
                return true;
            return  (InternalTimeSlot - _lastAccSlot) > N;
        }

        private void GenerateListenningSchedule()
        {
            var slots = new int[N];
            for (var i = 0; i < N; i++)
            {
                slots[i] = i;
            }
            var shuffle = new Shuffle(N);
            shuffle.KnuthShuffle(slots);

            _listeningSchedule = new bool[2 * N, N];
            for (int i = 0; i < 2 * N; i += 2)
                _listeningSchedule[i, slots[i / 2]] = true;
        }
        #endregion
    }
}
