
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;

namespace NeighborDiscovery.Protocols
{
    public abstract class BoundedProtocol:IDiscoveryProtocol
    {
        protected Dictionary<IDiscoveryProtocol, IContact> NeighborsDiscovered;
        public int Id { get; protected set; }
        public int InternalTimeSlot { get; private set; }
        public virtual int NumberOfNeighbors => NeighborsDiscovered.Count;
        public abstract int Bound { get; }
        public abstract int T { get; }

        protected BoundedProtocol(int id)
        {
            NeighborsDiscovered = new Dictionary<IDiscoveryProtocol, IContact>();
            Id = id;
            InternalTimeSlot = 0;
        }

        protected virtual void AddNeighbor(IDiscoveryProtocol device)
        {
            NeighborsDiscovered.Add(device, new ContactInfo(device, InternalTimeSlot));
        }

        //protected virtual void UpdateNeighborContactInfo(IDiscoveryProtocol device)
        //{
        //    NeighborsDiscovered[device].Update(InternalTimeSlot);
        //}

        public virtual ITransmission GetTransmission()
        {
            return !IsTransmitting() ? null : new Transmission(this);
        }

        public virtual IEnumerable<IDiscoveryProtocol> Neighbors()
        {
            return NeighborsDiscovered.Keys;
        }

        public virtual bool ContainsNeighbor(IDiscoveryProtocol device)
        {
            return NeighborsDiscovered.ContainsKey(device);
        }

        public abstract IDiscoveryProtocol Clone();

        public abstract override string ToString();

        public abstract bool IsListening();

        public abstract bool IsTransmitting();

        public virtual void ListenTo(ITransmission transmission)
        {
            if (!IsListening())
                return;
            
            if(!ContainsNeighbor(transmission.Sender))
                AddNeighbor(transmission.Sender);
            //else
                //UpdateNeighborContactInfo(transmission.Sender);
        }

        public IContact GetContactInfo(IDiscoveryProtocol device)
        {
            return NeighborsDiscovered.TryGetValue(device, out var value) ? value : null;
        }

        public virtual void MoveNext(int slot = 1)
        {
            if (slot < 0)
             throw new Exception("The Device can not move a negative number of slots");
            InternalTimeSlot += slot;
        }

        public abstract double GetDutyCycle();

        public abstract void SetDutyCycle(double value);

        public void Reset()
        {
            NeighborsDiscovered.Clear();
            InternalTimeSlot = 0;
        }

    }
}
