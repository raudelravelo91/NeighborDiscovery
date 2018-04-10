using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Protocols
{
    public abstract class AccProtocol:BoundedProtocol
    {
        protected Dictionary<IDiscoveryProtocol, ContactInfo2Hop> Neighbors2HopDiscovered;
        protected int NumberOfListenedSlots;
        protected int NumberOfTransmissions;
        protected int AccProtocolListenedSlots;
        public virtual int NumberOfNeighbors2Hop => Neighbors2HopDiscovered.Count;
        

        protected AccProtocol(int id) : base(id)
        {
            NumberOfListenedSlots = 0;
            AccProtocolListenedSlots = 0;
            Neighbors2HopDiscovered = new Dictionary<IDiscoveryProtocol, ContactInfo2Hop>();
        }

        protected abstract double SlotGain(int slot);

        protected virtual int ExpectedDiscovery(int t0, IDiscoveryProtocol device)
        {
            int limit = int.MaxValue;
            var transmissions = GetDeviceNextTransmissionSlot(t0, limit, device).GetEnumerator();
            var listen = GetDeviceNextListeningSlots(t0, limit, this).GetEnumerator();
            transmissions.MoveNext();
            listen.MoveNext();
            while (listen.Current != transmissions.Current)
            {
                if (listen.Current < transmissions.Current)
                    listen.MoveNext();
                else
                {
                    transmissions.MoveNext();
                }
            }
            transmissions.Dispose();
            listen.Dispose();
            return listen.Current - t0;
        }

        protected virtual IEnumerable<IDiscoveryProtocol> Get2HopNeighborsFromDirectNeighbor(IDiscoveryProtocol neighbor)
        {
            var lastContact = ((ContactInfo)GetContactInfo(neighbor)).LastContact;
            int slotsPassed = InternalTimeSlot - lastContact;//how many slots since I listened to my neighbor the last time
            int myKey = neighbor.InternalTimeSlot - slotsPassed;//this means I can see any discovered device by my neighbor before he contacted me the last time
            //this can access 2 hop neighbors via a neighbor when they were discovered before myKey
            foreach (var neighbor2Hop in neighbor.Neighbors())
            {
                if(neighbor2Hop.Equals(this))//skip yourself
                    continue;
                var info2Hop = neighbor.GetContactInfo(neighbor2Hop);
                if (info2Hop.FirstContact < myKey)
                    yield return neighbor2Hop;
            }
        }

        /// <summary>
        /// T0 and Tn are defined as the internal time slot of the given device
        /// </summary>
        /// <param name="t0">the starting internal time slot</param>
        /// <param name="tn">the ending internal time slot</param>
        /// <param name="device"></param>
        /// <returns></returns>
        protected virtual IEnumerable<int> GetDeviceNextTransmissionSlot(int t0, int tn, IDiscoveryProtocol device)
        {
            var clone = device.Clone();
            clone.MoveNext(t0);
            for (; t0 <= tn; t0++, clone.MoveNext())
            {
                if (clone.IsTransmitting())
                    yield return t0;
            }
        }

        /// <summary>
        /// T0 and Tn are defined as the internal time slot of the given device
        /// </summary>
        /// <param name="t0">the starting internal time slot</param>
        /// <param name="tn">the ending internal time slot</param>
        /// <param name="device"></param>
        /// <returns></returns>
        protected virtual IEnumerable<int> GetDeviceNextListeningSlots(int t0, int tn, IDiscoveryProtocol device)
        {
            var clone = device.Clone();
            clone.MoveNext(t0);
            for (; t0 <= tn; t0++, clone.MoveNext())
            {
                if (clone.IsListening())
                    yield return t0;
            }
        }

        protected virtual void AddNeighbor2Hop(IDiscoveryProtocol device)
        {
            if (ContainsNeighbor2Hop(device))
                return;
            int expectedDiscovery = ExpectedDiscovery(InternalTimeSlot, device);
            ContactInfo2Hop info = new ContactInfo2Hop(device, InternalTimeSlot, expectedDiscovery);
            Neighbors2HopDiscovered.Add(device, info);
        }

        protected virtual void RemoveNeighbor2Hop(IDiscoveryProtocol device)
        {
            if (ContainsNeighbor2Hop(device))
                Neighbors2HopDiscovered.Remove(device);
        }

        public virtual bool ContainsNeighbor2Hop(IDiscoveryProtocol device)
        {
            return Neighbors2HopDiscovered.ContainsKey(device);
        }

        public virtual IEnumerable<IDiscoveryProtocol> Neighbors2Hop()
        {
            return Neighbors2HopDiscovered.Keys;
        }

        public virtual ContactInfo2Hop GetContactInfoFor2Hop(IDiscoveryProtocol device)
        {
            return Neighbors2HopDiscovered.TryGetValue(device, out var value) ? value : null;
        }
    }
}
