using System;
using System.Collections.Generic;
using System.Linq;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public sealed class NeighborDiscoveryEnvironment
    {
        private NodeType _protocolType;
        private int _trackId;
        
        private readonly Random _random = new Random();

        private int EndsAt(int startUpSlot, BoundedProtocol device)
        {
            return startUpSlot + device.Bound;
        }
        
        private DiscoverableDevice FromDeviceDataToDiscoverableDevice(DeviceData data)
        {
            //if (data.Id == _trackId)
            var logic = CreateProtocol(data.Id, data.DutyCycle, _protocolType);
            int startUpInternalState = _random.Next(logic.T);
            while(logic.InternalTimeSlot < startUpInternalState)
                logic.MoveNext();
            
            return new DiscoverableDevice(logic, data.Position, data.Direction, data.Speed, data.CommunicationRange);
        }

        private Event CreateIncomingEvent(DeviceData data)
        {
            return new Event(FromDeviceDataToDiscoverableDevice(data), EventType.IncomingDevice);
        }

        private BoundedProtocol CreateProtocol(int id, int dutyCycle, NodeType nodeType)
        {
            switch (nodeType)
            {
                case NodeType.Birthday:
                    return null;
                case NodeType.Disco:
                    return new Disco(id, dutyCycle);
                case NodeType.UConnect:
                    return new UConnect(id, dutyCycle);
                case NodeType.Searchlight:
                    return null;
                case NodeType.StripedSearchlight:
                    return null;
                case NodeType.Hello:
                    return null;
                case NodeType.TestAlgorithm:
                    return null;
                case NodeType.BalancedNihao:
                    return new BalancedNihao(id, dutyCycle);
                case NodeType.AccBalancedNihao:
                    return new AccBalancedNihao(id, dutyCycle);
                case NodeType.AccBalancedNihaoExtended:
                    return new AccBalancedNihaoExtended(id, dutyCycle);
                default:
                {
                    throw new ArgumentException(_protocolType.ToString() + "(protocol) not supported");
                }
            }
        }

        public StatisticTestResult RunSingleSimulation(IEnumerable<DeviceData> data, NodeType protocolType, int accId = -1)
        {
            _protocolType = protocolType;
            _trackId = accId;
            List<DeviceData> dataList = data.ToList();
            dataList.Sort();
            Queue<DeviceData> events = new Queue<DeviceData>(dataList);
            int maxSlot = 200;//todo, improve the way to calculate the limit
            //int maxSlot = 1000;
            int currentSlot = 0;
            FullDiscoveryEnvironmentTmll fullEnv = new FullDiscoveryEnvironmentTmll(RunningMode.StaticDevices, accId);
            while (currentSlot < maxSlot)
            {
                while (events.Count > 0 && events.Peek().StartUpSlot == currentSlot)
                {
                    var nextEvent = CreateIncomingEvent(events.Dequeue());
                    fullEnv.AddEvent(nextEvent);
                }
                fullEnv.MoveNext();
                currentSlot++;
            }
            
            return fullEnv.GetCurrentResult();
        }
    }
}
