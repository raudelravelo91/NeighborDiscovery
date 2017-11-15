
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
        public int ID { get; protected set; }
        public int CommunicationRange { get; private set; }
        public int T { get; protected set; }
        public int NumberOfNeighbors { get { return neighbors.Count; } }

        public int StartUpTime { get; private set; }

        protected double desiredDutyCycle;//use to store the desired duty cycle. To get the real duty cycle call getDutyCycle()
        protected int internalTimeSlot;
        protected Dictionary<IDiscovery, ContactInfo> neighbors;

        public Node(int id, double dutyCyclePercentage, int communicationRange, int startUpTime)
        {
            neighbors = new Dictionary<IDiscovery, ContactInfo>();
            ID = id;
            internalTimeSlot = 0;
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
            desiredDutyCycle = dutyCyclePercentage;
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
            return neighbors.Select(x => new Tuple<IDiscovery, ContactInfo>(x.Key, x.Value));
        }

        public virtual bool WasDiscovered(IDiscovery neighbor)
        {
            return neighbors.ContainsKey(neighbor);
        }

        public virtual bool ListenTo(Transmission transmission, out List<IDiscovery> discoveredNodes)
        {
            discoveredNodes = new List<IDiscovery>();
            if (transmission != null)
            {
                if (IsListening(transmission.Slot))
                {
                    if (!WasDiscovered(transmission.Sender))
                    {
                        neighbors.Add(transmission.Sender, new ContactInfo(transmission.Slot));
                        discoveredNodes.Add(transmission.Sender);
                    }
                    else
                    {
                        neighbors[transmission.Sender].Update(transmission.Slot);
                    }
                    return true;
                }
            }
            else throw new ArgumentException("Null transmission received.");

            return false;
        }

        public override string ToString()
        {
            return "NodeId: " + ID + " Duty: " + Math.Round(GetDutyCycle(),2);
        }

        public virtual IDiscovery Clone()
        {
            throw new NotImplementedException();
        }

        public virtual void Reset(int startUpTime)
        {
            internalTimeSlot = 0;
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
