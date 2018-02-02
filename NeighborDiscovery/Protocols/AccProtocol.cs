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
        public virtual int NumberOfNeighbors2Hop => Neighbors2HopDiscovered.Count;

        protected AccProtocol(int id) : base(id)
        {
            

        }

        protected abstract double SlotGain(int slot);

        //protected virtual int ExpectedDiscovery(int t0, BoundedProtocol device)
        //{
        //    int limit = int.MaxValue;
        //    var transmissions = GetDeviceNextTransmissionSlot(t0, limit, device).GetEnumerator();
        //    var listen = GetDeviceNextListeningSlots(t0, limit, this).GetEnumerator();
        //    transmissions.MoveNext();
        //    listen.MoveNext();
        //    while (listen.Current != transmissions.Current)
        //    {
        //        if (listen.Current < transmissions.Current)
        //            listen.MoveNext();
        //        else
        //        {
        //            transmissions.MoveNext();
        //        }
        //    }
        //    transmissions.Dispose();
        //    listen.Dispose();
        //    return listen.Current - t0;
        //}

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

        protected virtual IEnumerable<int> GetDeviceNextTransmissionSlot(int t0, int tn, IDiscoveryProtocol device)
        {
            var clone = device.Clone();
            clone.MoveNext(device.InternalTimeSlot + t0);
            int cnt = t0;
            while (cnt <= tn)
            {
                clone.MoveNext();
                if (clone.IsTransmitting())
                    yield return cnt;
                cnt++;
            }
        }

        protected virtual IEnumerable<int> GetDeviceNextListeningSlots(int t0, int tn, IDiscoveryProtocol device)
        {
            var clone = device.Clone();
            clone.MoveNext(device.InternalTimeSlot + t0);
            int cnt = t0;
            while (cnt <= tn)
            {
                clone.MoveNext();
                if (clone.IsListening())
                    yield return cnt;
                cnt++;
            }
        }

        protected virtual void AddNeighbor2Hop(IDiscoveryProtocol device)
        {

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

        public ContactInfo2Hop GetContactInfoFor2Hop(IDiscoveryProtocol device)
        {
            return Neighbors2HopDiscovered.TryGetValue(device, out var value) ? value : null;
        }
    }
}
