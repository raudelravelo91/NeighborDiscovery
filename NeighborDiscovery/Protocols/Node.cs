
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;

namespace NeighborDiscovery.Protocols
{
    public abstract class Node:IDiscoveryProtocol
    {
        public int Id { get; protected set; }
        public int T { get; protected set; }
        //public override int NumberOfNeighbors => Neighbors.Count;
        //public override int StartUpTime { get; protected set; }
        //public override int CommunicationRange { get; protected set; }

        //protected double DesiredDutyCycle;//use to store the desired duty cycle. To get the real duty cycle call getDutyCycle()
        //protected int InternalTimeSlot;
        //protected Dictionary<DiscoverableDevice, ContactInfo> Neighbors;

        protected Node(int id, double dutyCyclePercentage, int communicationRange, int startUpTime)
        {
            //Neighbors = new Dictionary<DiscoverableDevice, ContactInfo>();
            //Id = id;
            //InternalTimeSlot = 0;
            //CommunicationRange = communicationRange;
            //StartUpTime = startUpTime;
            //SetDutyCycle(dutyCyclePercentage);
        }
        
        public Transmission GetTransmission()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<IDiscoveryProtocol, ContactInfo>> Neighbors()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IDiscoveryProtocol> ListenTo(Transmission transmission)
        {
            throw new NotImplementedException();
        }

        public bool IsListening()
        {
            throw new NotImplementedException();
        }

        public bool IsTransmitting()
        {
            throw new NotImplementedException();
        }

        public void MoveNext()
        {
            throw new NotImplementedException();
        }

        public virtual void SetDutyCycle(double dutyCyclePercentage)//an integer between 2 and 10 representing the percentage of work
        {
            if (dutyCyclePercentage < 1 || dutyCyclePercentage > 10)
                throw new ArgumentException("Invalid Duty Cycle");
            DesiredDutyCycle = dutyCyclePercentage;
        }


        double IDiscoveryProtocol.SetDutyCycle(double value)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Tuple<DiscoverableDevice, ContactInfo>> NeighborsDiscovered()
        {
            return Neighbors.Select(x => new Tuple<DiscoverableDevice, ContactInfo>(x.Key, x.Value));
        }

        public virtual bool WasDiscovered(DiscoverableDevice neighbor)
        {
            return Neighbors.ContainsKey(neighbor);
        }

        public bool ListenTo(Transmission transmission, out List<DiscoverableDevice> discoveredNodes)
        {
            discoveredNodes = new List<DiscoverableDevice>();
            if (transmission == null)
                throw new ArgumentException("Null transmission received.");

            if (!IsListening(transmission.Slot))
                return false;

            if (!WasDiscovered(transmission.Sender))
            {
                //add new neighbor
                Neighbors.Add(transmission.Sender, new ContactInfo(transmission.Slot));
                discoveredNodes.Add(transmission.Sender);
            }
            else
            {
                //update neighbor's contact info
                Neighbors[transmission.Sender].Update(transmission.Slot);
            }
            return true;
        }

        public string ToString()
        {
            return "NodeId: " + Id + " Duty: " + Math.Round(GetDutyCycle(),2);
        }

        public void Reset()
        {
            InternalTimeSlot = 0;
        }
    }
}
