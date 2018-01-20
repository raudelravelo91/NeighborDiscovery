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
    public enum RunningMode
    {
        StaticDevices,
        MovingDevices
    }

    public sealed class FullDiscoveryEnvironmentTmll
    {
        private readonly Network2D _network;
        private readonly Queue<Event> _events;
        private readonly Dictionary<DiscoverableDevice, Network2DNode> _binding;

        public int CurrentTimeSlot { get; private set; }
        public int CurrentNumberOfDevices => _binding.Count;
        public RunningMode RunningMode { get; }

        public FullDiscoveryEnvironmentTmll(RunningMode runningMode, IEnumerable<Event> initialEvents = default(IEnumerable<Event>))
        {
            CurrentTimeSlot = 0;
            RunningMode = runningMode;
            
            switch (runningMode)
            {
                case RunningMode.StaticDevices:
                    RunningMode = RunningMode.StaticDevices;
                    break;
                case RunningMode.MovingDevices:
                    RunningMode = RunningMode.MovingDevices;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(runningMode), runningMode, null);
            }
            
            var staticDevices = RunningMode == RunningMode.StaticDevices;
            _network = new Network2D(staticDevices);
            
            _binding = new Dictionary<DiscoverableDevice, Network2DNode>();
            
            if (initialEvents != null) 
                _events = new Queue<Event>(initialEvents);
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
            return _events;
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

        public void AddEvent(Event newEvent)
        {
            _events.Enqueue(newEvent);
        }
    }
}
