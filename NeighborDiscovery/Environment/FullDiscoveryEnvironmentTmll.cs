using System;
using System.Collections.Generic;
using System.Linq;
using NeighborDiscovery.Networks;
using NeighborDiscovery.Statistics;
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
        private readonly StatisticTestResult _trackedStatistics;
        private readonly bool _trackFirst;
        private DiscoverableDevice _trakedDevice;
        public int CurrentTimeSlot { get; private set; }
        public int CurrentNumberOfDevices => _deviceToLocation.Count;
        public RunningMode RunningMode { get; }
        public double AvgNoNeighbors => _network.AvgNoNeighbor;
        public double AvgNoNeighborsTracked => _network.NeighborsOf(_deviceToLocation[_trakedDevice]).Count();
        public int TotalTransmissionsSent { get; private set; }
        public int TotalTransmissionsSentTracked { get; private set; }

        public FullDiscoveryEnvironmentTmll(RunningMode runningMode, bool trackFirst = true)
        {
            _trackedStatistics = new StatisticTestResult();
            _trakedDevice = null;
            _trackFirst = trackFirst;
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
                ProccessEvent(iEvent);
            }

            SendTranssmissions();

            MoveAll();
        }

        public void AddEvent(Event newEvent)
        {
            _events.Enqueue(newEvent);
            var logic = newEvent.Device.DeviceLogic;
            _eventArrival.Add(logic, new DeviceInfo(logic.InternalTimeSlot, CurrentTimeSlot));

            if(_trackFirst && _trakedDevice == null)
            {
                _trakedDevice = newEvent.Device;
                logic.OnDeviceDiscovered += Logic_OnDeviceDiscovered;
            }
        }

        public StatisticTestResult GetCurrentResult()
        {
            if (_trakedDevice != null)
            {
                //_eventArrival.Sum(x => (x.Key.InternalTimeSlot - x.Value.InitialState));
                int sumOfSlotsTracked = _trakedDevice.DeviceLogic.InternalTimeSlot - _eventArrival[_trakedDevice.DeviceLogic].InitialState + 1;
                _trackedStatistics.AvgTransmissionsPerPeriod = sumOfSlotsTracked * 1.0 / TotalTransmissionsSentTracked;
                _trackedStatistics.AvgNoNeighbors = AvgNoNeighborsTracked;
                _trackedStatistics.AvgNoNeighborsPerSlot = AvgNoNeighborsTracked / sumOfSlotsTracked;
                return _trackedStatistics;
            }

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

            //int sumOfSlots = _deviceById.Values.Sum(x => x.DeviceLogic.InternalTimeSlot - _eventArrival[x].InitialState);
            var sumOfSlots = _eventArrival.Sum(x => (x.Key.InternalTimeSlot - x.Value.InitialState));
            statistics.AvgNoNeighbors = AvgNoNeighbors;
            statistics.AvgTransmissionsPerPeriod = sumOfSlots * 1.0 / TotalTransmissionsSent;
            return statistics;
        }

        #region private methods
        
        private void Logic_OnDeviceDiscovered(object sender, INodeResult e)
        {
            IDiscoveryProtocol deviceSender = sender as IDiscoveryProtocol;
            var device = _deviceById[deviceSender.Id];
            foreach (var neighbor in e.NewDiscoveries)
            {
                var neighborDevice = _deviceById[neighbor.Device.Id];
                if (!_deviceToLocation[device].NodeIsInRange(_deviceToLocation[neighborDevice]))
                {
                    throw new Exception("WTF!");
                    //return;
                }

                var latency = GetDiscoveryLatencyInStaticNetwork(device, neighborDevice);
                _trackedStatistics.AddDiscovery(latency);
            }
        }

        private void SendTranssmissions()
        {
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
                TotalTransmissionsSent++;
                if (_trakedDevice.DeviceLogic.Equals(currentDevice.DeviceLogic))
                    TotalTransmissionsSentTracked++;
            }
        }

        private void ProccessEvent(Event iEvent)
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
            IContact contactInfo = null;
            if(listener.DeviceLogic.ContainsNeighbor(transmitter.DeviceLogic))
                contactInfo = listener.DeviceLogic.GetContactInfo(transmitter.DeviceLogic);
            else
                throw new Exception("Devices did not discover each other");
            
            var listenerLocation = _deviceToLocation[listener];
            var transmitterLocation = _deviceToLocation[transmitter];

            return this.CurrentTimeSlot - _eventArrival[transmitter.DeviceLogic].EnviromentState;

            ////todo => fix this
            //int listenedIn = ToEnviromentTime(listener.DeviceLogic, contactInfo.FirstContact);//in environment time
            //int gotInRange = _network.GotInRange(transmitterLocation, listenerLocation);//in environment time
            //int latency = listenedIn - gotInRange + 1;

            //if (latency > 200)
            //{
            //    Console.WriteLine("Something wrong");
            //    ToEnviromentTime(listener.DeviceLogic, contactInfo.FirstContact);
            //    _network.GotInRange(transmitterLocation, listenerLocation);
            //}

            //return latency;
        }
        #endregion
    }
}
