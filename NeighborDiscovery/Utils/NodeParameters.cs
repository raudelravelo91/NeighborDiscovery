using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Utils
{
    public class BasicParameters
    {
        public static int numberOfDutyCyclesToHandle { get; set; }
        public int Id { get; private set; }
        public int DutyCyclePercentage { get; private set; }
        public int CommunicationRange { get; private set; }

        public int StartUpTime { get; private set; }

        public BasicParameters(int id, int dutyCyclePercentage, int communicationRange, int startUpTime)
        {
            Id = id;
            DutyCyclePercentage = dutyCyclePercentage;
            CommunicationRange = communicationRange;
            StartUpTime = startUpTime;
        }
    }

    public class NodeParameters
    {
        public NodeType Type { get; private set; }
        public int Id { get; private set; }
        public double DutyCyclePercentage { get; private set; }
        public int CommunicationRange { get; private set; }
        
        public NodeParameters(NodeType type, int id, double dutyCyclePercentage, int communicationRange)
        {
            Type = type;
            Id = id;
            DutyCyclePercentage = dutyCyclePercentage;
            CommunicationRange = communicationRange;
        }

    }

    public class GNihaoParameters : NodeParameters
    {
        public bool RandomInitialState { get; private set; }
        public int M { get; private set; }
        public GNihaoParameters(int id, double dutyCyclePercentage, int communicationRange, int m, bool randomInitialState) : base(NodeType.GNihao, id, dutyCyclePercentage, communicationRange)
        {
            M = m;
            RandomInitialState = randomInitialState;
        }
    }

    public class PNihaoParameters : NodeParameters
    {
        public bool RandomInitialState { get; private set; }
        public int M { get; private set; }
        public PNihaoParameters(int id, double dutyCyclePercentage, int communicationRange, int m, bool randomInitialState) : base(NodeType.GNihao, id, dutyCyclePercentage, communicationRange)
        {
            M = m;
            RandomInitialState = randomInitialState;
        }
    }

    public class DiscoParameters : NodeParameters
    {
        public bool Balanced { get; private set; }
        public DiscoParameters(int id, double dutyCyclePercentage, int communicationRange, bool balanced) : base(NodeType.Disco, id, dutyCyclePercentage, communicationRange)
        {
            Balanced = balanced;
        }
    }

    public class HelloParameters : NodeParameters
    {
        public bool Symmetric { get; private set; }
        public HelloParameters(int id, int dutyCyclePercentage, int communicationRange, bool symmetric) : base(NodeType.Disco, id, dutyCyclePercentage, communicationRange)
        {
            Symmetric = symmetric;
        }
    }

}
