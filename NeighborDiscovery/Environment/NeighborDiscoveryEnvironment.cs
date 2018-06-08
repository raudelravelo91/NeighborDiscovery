using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public sealed class NeighborDiscoveryEnvironment
    {
        private NodeType _protocolType;
        private readonly Random _random = new Random();

        private int EndsAt(int startUpSlot, BoundedProtocol device)
        {
            return startUpSlot + device.Bound;
        }
        
        private DiscoverableDevice FromDeviceDataToDiscoverableDevice(DeviceData data)
        {
            //if (data.Id == _trackId)
            var logic = CreateProtocol(data.Id, data.DutyCycle, _protocolType, DeviceData.COR);
            int startUpInternalState = _random.Next(logic.T);
            while(logic.InternalTimeSlot < startUpInternalState)
                logic.MoveNext();
            
            return new DiscoverableDevice(logic, data.Position, data.Direction, data.Speed, data.CommunicationRange);
        }

        private Event CreateIncomingEvent(DeviceData data)
        {
            return new Event(FromDeviceDataToDiscoverableDevice(data), EventType.IncomingDevice);
        }

        private BoundedProtocol CreateProtocol(int id, int dutyCycle, NodeType nodeType, int m)
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
                case NodeType.GNihao:
                    return new GNihao(id, dutyCycle, m);
                case NodeType.THL2H:
                    return new THL2H(id, dutyCycle, m);
                case NodeType.THL2HExtended:
                    return new THL2HExtended(id, dutyCycle, m);
                default:
                {
                    throw new ArgumentException(_protocolType.ToString() + "(protocol) not supported");
                }
            }
        }

        public StatisticTestResult RunSingleSimulation(IEnumerable<DeviceData> data, NodeType protocolType)
        {
            _protocolType = protocolType;
            List<DeviceData> dataList = data.ToList();
            dataList.Sort();
            Queue<DeviceData> events = new Queue<DeviceData>(dataList);
            int maxSlot = SimulationLimit(dataList);//todo, improve the way to calculate the simulation limit
            
            int currentSlot = 0;
            FullDiscoveryEnvironmentTmll fullEnv = new FullDiscoveryEnvironmentTmll(RunningMode.StaticDevices);//parameter trackFirst = true by default
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

        public int SimulationLimit(List<DeviceData> sortedData)
        {
            int duty = sortedData.Min(node => node.DutyCycle);
            switch (duty)
            {
                case 1:
                    return 20000;
                case 5:
                    return 4000;
                case 10:
                    return 2000;
                default:
                    throw new Exception("duty cycle not supported");
            }
        }
    }
}
