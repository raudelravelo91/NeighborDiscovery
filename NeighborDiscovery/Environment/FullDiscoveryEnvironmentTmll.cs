using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Utils;
using NeighborDiscovery.Statistics;
using Priority_Queue;
using NeighborDiscovery.Protocols;

namespace NeighborDiscovery.Environment
{
    public sealed class FullDiscoveryEnvironmentTmll
    {
        public int CurrentTimeSlot { get; private set; }
        public int CurrentNumberOfDevices => _binding.Count;

        private readonly Network2D _network;
        private readonly Queue<Event> _events;
        private readonly Dictionary<DiscoverableDevice, Network2DNode> _binding;

        public FullDiscoveryEnvironmentTmll(IEnumerable<Event> events)
        {
            CurrentTimeSlot = 0;
            _events =  new Queue<Event>(events);
            _network = new Network2D();
            _binding = new Dictionary<Network2DNode, DiscoverableDevice>();
        }

        private void UpdatePhysicalPartOfDiscoverableDevice(DiscoverableDevice device, Network2DNode node)
        {
            device.Position.X = node.Position.X;
            device.Position.Y = node.Position.Y;
            device.Direction.X = node.Direction.X;
            device.Direction.Y = node.Direction.Y;
            device.Speed = node.Speed;
        }

        private Network2DNode Get2DNodeFromDiscoverableDevice(DiscoverableDevice device)
        {
            return new Network2DNode(device.DeviceLogic.Id, device.Position, device.Direction, device.CommunicationRange, device.Speed);
        }

        private IEnumerable<Event> FetchNextEvents()
        {
            while (_events.Peek().TimeSlot == CurrentTimeSlot)
            {
                yield return _events.Dequeue();
            }
        }

        private void RemoveDevice(DiscoverableDevice device)
        {
            var physicalNode = _binding[device];
            _network.RemoveNode(physicalNode);
            _binding.Remove(device);
        }

        public void MoveNext()
        {
            foreach (var iEvent in FetchNextEvents())
            {
                switch (iEvent.EventType)
                {
                    case EventType.IncomingDevice:
                        var device = iEvent.Device;
                        var newPhysicalNode = Get2DNodeFromDiscoverableDevice(device);
                        _network.AddNode(newPhysicalNode);
                        _binding.Add(device, newPhysicalNode);
                        break;
                    case EventType.DeviceGone:
                        RemoveDevice(iEvent.Device);
                        break;
                }
            }

            //send transmissions
            foreach (var kvPair in _binding)
            {
                var currentDevice = kvPair.Key;
                var currentPhysicalDevice = kvPair.Value;

                if (!currentDevice.DeviceLogic.IsTransmitting()) 
                    continue;
                
                var transmission = currentDevice.DeviceLogic.GetTransmission();
                foreach (var device in _binding.Keys)
                {
                    var deviceLogic = device.DeviceLogic;
                    var physicalDevice = _binding[device];
                    if (device.Equals(currentDevice) || !deviceLogic.IsListening() ||
                        !currentPhysicalDevice.NodeIsInRange(physicalDevice))
                        continue;
                    deviceLogic.ListenTo(transmission);
                }
            }

            //move next both the phisical part and the logical part
            foreach (var kvPair in _binding)
            {
                var device = kvPair.Key;
                var physicalNode = kvPair.Value;
                physicalNode.Move();
                UpdatePhysicalPartOfDiscoverableDevice(device, physicalNode);
                device.DeviceLogic.MoveNext();
            }

            CurrentTimeSlot++;
        }
    }
}
