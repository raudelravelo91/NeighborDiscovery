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
        private class DeviceInfo
        {
            public int InitialState { get; }
            public int EnviromentState { get; }

            public DeviceInfo(int initialState, int enviromentState)
            {
                InitialState = initialState;
                EnviromentState = enviromentState;
            }
        }

        private readonly Network2D _network;
        private readonly Queue<Event> _events;
        private readonly Dictionary<DiscoverableDevice, Network2DNode> _deviceToLocation;
        private readonly Dictionary<Network2DNode, DiscoverableDevice> _locationToDevice;
        private readonly Dictionary<int, DiscoverableDevice> _deviceById;
        private readonly Dictionary<IDiscoveryProtocol, DeviceInfo> _eventArrival;

        public int CurrentTimeSlot { get; private set; }
        public int CurrentNumberOfDevices => _deviceToLocation.Count;
        public RunningMode RunningMode { get; }

        public FullDiscoveryEnvironmentTmll(RunningMode runningMode)
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
            
            _deviceToLocation = new Dictionary<DiscoverableDevice, Network2DNode>();
            _locationToDevice = new Dictionary<Network2DNode, DiscoverableDevice>();
            _deviceById = new Dictionary<int, DiscoverableDevice>();
            _eventArrival = new Dictionary<IDiscoveryProtocol, DeviceInfo>();
            _events = new Queue<Event>();
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
                        _deviceById.Add(device.DeviceLogic.Id, device);
                        _deviceToLocation.Add(device, newPhysicalNode);
                        _locationToDevice.Add(newPhysicalNode, device);
                        break;
                    case EventType.DeviceGone:
                        RemoveDevice(iEvent.Device);
                        break;
                }
            }

            //send transmissions
            foreach (var kvPair in _deviceToLocation)
            {
                var currentDevice = kvPair.Key;
                var currentPhysicalDevice = kvPair.Value;

                if (!currentDevice.DeviceLogic.IsTransmitting()) 
                    continue;
                
                var transmission = currentDevice.DeviceLogic.GetTransmission();
                
                foreach (var neighborLocation in _network.NeighborsOf(currentPhysicalDevice))//neighbors in range
                {
                    var neighborLogic = _locationToDevice[neighborLocation].DeviceLogic;
                    neighborLogic.ListenTo(transmission);
                }
            }
    
            MoveAll();
        }

        public void AddEvent(Event newEvent)
        {
            _events.Enqueue(newEvent);
            var logic = newEvent.Device.DeviceLogic;
            _eventArrival.Add(logic, new DeviceInfo(logic.InternalTimeSlot, CurrentTimeSlot));
        }
        
        public StatisticTestResult GetCurrentResult()
        {
            var statistics = new StatisticTestResult();
            foreach (var kvPair in _locationToDevice)
            {
                var device = kvPair.Value;
                foreach (var neighbor in device.DeviceLogic.Neighbors())
                {
                    var neighborDevice = _deviceById[neighbor.Id];
                    var latency = GetDiscoveryLatencyInStaticNetwork(device, neighborDevice);
                    statistics.AddDiscovery(latency);
                }
            }
            return statistics;
        }

        public StatisticTestResult GetCurrentResult(int deviceId)
        {
            if(!_deviceById.ContainsKey(deviceId))
                throw new Exception("The environment does not contains any device whith the given Id");
            
            var device = _deviceById[deviceId];
            var statistics = new StatisticTestResult();

            foreach (var neighbor in device.DeviceLogic.Neighbors())
            {
                var neighborDevice = _deviceById[neighbor.Id];
                var latency = GetDiscoveryLatencyInStaticNetwork(device, neighborDevice);
                statistics.AddDiscovery(latency);
            }
            
            return statistics;
        }

        #region private methods
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
            while (_events.Count > 0)
                yield return _events.Dequeue();
        }

        private void RemoveDevice(DiscoverableDevice device)
        {
            var physicalNode = _deviceToLocation[device];
            _network.RemoveNode(physicalNode);
            _deviceToLocation.Remove(device);
        }

        private void MoveAll()
        {
            //move next both the phisical part and the logical part
            _network.MoveAllNodes();//move the physical part first
            
            foreach (var kvPair in _deviceToLocation)
            {
                var device = kvPair.Key;
                var physicalNode = kvPair.Value;
                UpdatePhysicalPartOfDiscoverableDevice(device, physicalNode);//update 
                device.DeviceLogic.MoveNext();
            }

            CurrentTimeSlot++;
        }

        private int ToEnviromentTime(IDiscoveryProtocol device, int deviceSlot)
        {
            return _eventArrival[device].EnviromentState + (deviceSlot - _eventArrival[device].InitialState);
        }

        private int GetDiscoveryLatencyInStaticNetwork(DiscoverableDevice listener, DiscoverableDevice transmitter)
        {
            var contactInfo = listener.DeviceLogic.GetContactInfo(transmitter.DeviceLogic);
            if (contactInfo == null)
                throw new Exception("Devices did not discover each other");
            
            var listenerLocation = _deviceToLocation[listener];
            var transmitterLocation = _deviceToLocation[transmitter];
            
            //todo => fix this
            int listenedIn = ToEnviromentTime(listener.DeviceLogic, contactInfo.FirstContact);//environment time
            int gotInRange = _network.GotInRange(transmitterLocation, listenerLocation);//double check if the returned value is in environment time
            int latency = listenedIn - gotInRange;

            if (latency > 200)
            {
                Console.WriteLine("Something wrong");
                ToEnviromentTime(listener.DeviceLogic, contactInfo.FirstContact);
                _network.GotInRange(transmitterLocation, listenerLocation);
            }

            return latency;
        }
        #endregion
    }
}
