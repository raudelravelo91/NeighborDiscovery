using System;
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
        public MyPair Direction { get; }
        public int Speed { get; }
        public int CommunicationRange { get; }

        public DeviceData(int id, int startUpSlot, int dutyCycle, int communicationRange, double posX, double posY, double dirX, double dirY, int speed)
        {
            Id = id;
            StartUpSlot = startUpSlot;
            DutyCycle = dutyCycle;
            Position = new MyPair(posX, posY);
            CommunicationRange = communicationRange;
            Direction = new MyPair(dirX, dirY);
            Speed = speed;
        }

        public int CompareTo(DeviceData other)
        {
            return StartUpSlot.CompareTo(other.StartUpSlot);
        }
    }
}
