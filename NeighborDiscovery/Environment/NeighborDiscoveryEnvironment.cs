using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Protocols;
using NeighborDiscovery.Statistics;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Environment
{
    public sealed class NeighborDiscoveryEnvironment
    {
        private NodeType _protocolType;

        private Random _random = new Random();

        private int EndsAt(int startUpSlot, BoundedProtocol device)
        {
            return startUpSlot + device.Bound;
        }

        private DiscoverableDevice FromDeviceDataToDiscoverableDevice(DeviceData data)
        {
            var logic = CreateProtocol(data.Id, data.DutyCycle);
            logic.MoveNext(_random.Next(logic.T));
            return new DiscoverableDevice(logic, data.Position, new MyPair(0,0), 0, data.CommunicationRange);
        }

        private Event CreateIncomingEvent(DeviceData data)
        {
            return new Event(FromDeviceDataToDiscoverableDevice(data), EventType.IncomingDevice);
        }

        public BoundedProtocol CreateProtocol(int id, int dutyCycle)
        {
            switch (_protocolType)
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
                    return null;
                case NodeType.BalancedNihao:
                    return new BalancedNihaoTmll(id, dutyCycle);
                case NodeType.AccGossipGNihao:
                    return null;
                case NodeType.AccGossipPNihao:
                    return null;
                default:
                {
                    throw new ArgumentException(_protocolType.ToString() + "(protocol) not supported");
                }
            }
        }

        public StatisticTestResult RunSingleSimulation(IEnumerable<DeviceData> data, NodeType protocolType)
        {
            _protocolType = protocolType;
            List<DeviceData> _data = data.ToList();
            _data.Sort();
            Queue<DeviceData> events = new Queue<DeviceData>(_data);
            int maxSlot = _data[_data.Count - 1].StartUpSlot * 2  + 1;//todo, improve the way to calculate the limit
            int currentSlot = 0;
            FullDiscoveryEnvironmentTmll fullEnv = new FullDiscoveryEnvironmentTmll(RunningMode.StaticDevices);
            while (currentSlot < maxSlot)
            {
                while (events.Count > 0 && events.Peek().StartUpSlot == currentSlot)
                {
                    fullEnv.AddEvent(CreateIncomingEvent(events.Dequeue()));
                }
                fullEnv.MoveNext();
                currentSlot++;
            }

            return fullEnv.GetCurrentResult();
        }
    }
}
