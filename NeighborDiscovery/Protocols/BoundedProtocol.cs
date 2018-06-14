
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Protocols
{
    public abstract class BoundedProtocol:IDiscoveryProtocol
    {
        
        public Dictionary<IDiscoveryProtocol, ContactInfo> NeighborsDiscovered;
        public int Id { get; protected set; }
        public int InternalTimeSlot { get; protected set; }
        public int NoOfTransmissionsSent { get; protected set; }
        public virtual int NumberOfNeighbors => NeighborsDiscovered.Count;
        public abstract int T { get; }
        public event EventHandler<INodeResult> OnDeviceDiscovered;

        protected BoundedProtocol(int id)
        {
            NeighborsDiscovered = new Dictionary<IDiscoveryProtocol, ContactInfo>();
            Id = id;
            InternalTimeSlot = 0;
        }

        protected void RaiseOnDeviceDiscovered(INodeResult data)
        {
            if(OnDeviceDiscovered != null)
                OnDeviceDiscovered(this, data);

        }

        protected virtual void AddNeighbor(IDiscoveryProtocol device)
        {
            NeighborsDiscovered.Add(device, new ContactInfo(device, InternalTimeSlot));
        }

        protected virtual void RemoveNeihbor(IDiscoveryProtocol device)
        {
            if (ContainsNeighbor(device))
                NeighborsDiscovered.Remove(device);
        }

        protected virtual void UpdateNeighborContactInfo(IDiscoveryProtocol device)
        {
            NeighborsDiscovered[device].Update(InternalTimeSlot);
        }

        public virtual ITransmission GetTransmission()
        {
            if (!IsTransmitting())
                return null;
            NoOfTransmissionsSent++;
            return new Transmission(this);
        }

        public virtual IEnumerable<IDiscoveryProtocol> Neighbors()
        {
            return NeighborsDiscovered.Keys;
        }

        public virtual bool ContainsNeighbor(IDiscoveryProtocol device)
        {
            return NeighborsDiscovered.ContainsKey(device);
        }

        public IDiscoveryProtocol Clone()
        {
            return MemberwiseClone() as BoundedProtocol;
        }

        public abstract override string ToString();

        public abstract bool IsListening();

        public abstract bool IsTransmitting();

        public virtual void ListenTo(ITransmission transmission)
        {
            if (!IsListening())
                return;
            
            if(!ContainsNeighbor(transmission.Sender))
            {
                AddNeighbor(transmission.Sender);
                var data = new NodeResult();
                data.AddDiscovery(GetContactInfo(transmission.Sender));
                RaiseOnDeviceDiscovered(data);
            }
        }

        public IContact GetContactInfo(IDiscoveryProtocol device)
        {
            return NeighborsDiscovered.TryGetValue(device, out var value) ? value : null;
        }

        public virtual void MoveNext()
        {
            InternalTimeSlot ++;
        }

        public abstract double GetDutyCycle();

        public abstract void SetDutyCycle(double value);

        public void Reset()
        {
            NeighborsDiscovered.Clear();
            NoOfTransmissionsSent = 0;
            InternalTimeSlot = 0;
        }

    }
}
