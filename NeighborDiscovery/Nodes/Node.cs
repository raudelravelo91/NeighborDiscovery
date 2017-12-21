
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;

namespace NeighborDiscovery.Nodes
{
    public abstract class Node:IDiscovery
    {
        public int Id { get; protected set; }
        public int CommunicationRange { get; private set; }
        public int T { get; protected set; }
        public int NumberOfNeighbors => Neighbors.Count;

        public int StartUpTime { get; private set; }

        protected double DesiredDutyCycle;//use to store the desired duty cycle. To get the real duty cycle call getDutyCycle()
        protected int InternalTimeSlot;
        protected Dictionary<IDiscovery, ContactInfo> Neighbors;

        protected Node(int id, double dutyCyclePercentage, int communicationRange, int startUpTime)
        {
            Neighbors = new Dictionary<IDiscovery, ContactInfo>();
            Id = id;
            InternalTimeSlot = 0;
            CommunicationRange = communicationRange;
            StartUpTime = startUpTime;
            SetDutyCycle(dutyCyclePercentage);
        }

        public virtual int ToRealTimeSlot(int internalTimeSlot)
        {
            return StartUpTime + internalTimeSlot;
        }

        public virtual int FromRealTimeSlot(int realTimeSlot)
        {
            return realTimeSlot - StartUpTime;
        }

        public virtual void SetDutyCycle(double dutyCyclePercentage)//an integer between 2 and 10 representing the percentage of work
        {
            if (dutyCyclePercentage < 1 || dutyCyclePercentage > 10)
                throw new ArgumentException("Invalid Duty Cycle");
            DesiredDutyCycle = dutyCyclePercentage;
        }

        /// <summary>
        /// Get the actual duty cycle the Node is working at
        /// </summary>
        /// <returns></returns>
        public virtual double GetDutyCycle()
        {
            throw new NotImplementedException();
        }

        public virtual bool IsListening(int realTimeSlot)
        {
            throw new NotImplementedException();
        }

        public virtual bool IsTransmitting(int realTimeSlot)
        {
            throw new NotImplementedException();
        }

        public virtual IEnumerable<Tuple<IDiscovery, ContactInfo>> NeighborsDiscovered()
        {
            return Neighbors.Select(x => new Tuple<IDiscovery, ContactInfo>(x.Key, x.Value));
        }

        public virtual bool WasDiscovered(IDiscovery neighbor)
        {
            return Neighbors.ContainsKey(neighbor);
        }

        public virtual bool ListenTo(Transmission transmission, out List<IDiscovery> discoveredNodes)
        {
            discoveredNodes = new List<IDiscovery>();
            if (transmission == null)
                throw new ArgumentException("Null transmission received.");

            if (!IsListening(transmission.Slot))
                return false;

            if (!WasDiscovered(transmission.Sender))
            {
                Neighbors.Add(transmission.Sender, new ContactInfo(transmission.Slot));
                discoveredNodes.Add(transmission.Sender);
            }
            else
            {
                Neighbors[transmission.Sender].Update(transmission.Slot);
            }
            return true;
        }

        public override string ToString()
        {
            return "NodeId: " + Id + " Duty: " + Math.Round(GetDutyCycle(),2);
        }

        public virtual IDiscovery Clone()
        {
            throw new NotImplementedException();
        }

        public virtual void Reset(int startUpTime)
        {
            InternalTimeSlot = 0;
            StartUpTime = startUpTime;
        }

        public virtual Transmission NextTransmission()
        {
            throw new NotImplementedException();
        }

        public virtual Transmission FirstTransmissionAfter(int slot)
        {
            throw new NotImplementedException();
        }
    }
}
