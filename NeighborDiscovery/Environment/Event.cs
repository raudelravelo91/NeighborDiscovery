using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Environment
{
    public enum EventType
    {
        IncomingDevice,
        DeviceGone
    }

    public class Event
    {
        public EventType EventType { get; }
        //public int TimeSlot { get; }
        public DiscoverableDevice Device { get; }

        public Event(DiscoverableDevice device, EventType eventType)
        {
            EventType = eventType;
            //TimeSlot = timeSlot;
            Device = device;
        }

        public override string ToString()
        {
            return EventType.ToString() + ": " + Device.ToString();
        }
    }
}
