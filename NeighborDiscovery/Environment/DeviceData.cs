using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    [Serializable]
    public class DeviceData:IComparable<DeviceData>
    {
        public int Id { get; }
        public int StartUpSlot { get; }
        public int DutyCycle { get; }
        public MyPair Position { get; }
        public int CommunicationRange { get; }

        public DeviceData(int id, int startUpSlot, int dutyCycle, MyPair position, int communicationRange)
        {
            Id = id;
            StartUpSlot = startUpSlot;
            DutyCycle = dutyCycle;
            Position = position;
            CommunicationRange = communicationRange;
        }

        public int CompareTo(DeviceData other)
        {
            return StartUpSlot.CompareTo(other.StartUpSlot);
        }
    }
}
