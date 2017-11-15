using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Nodes;

namespace NeighborDiscovery.Environment
{
    public class Transmission
    {
        public int Slot { get; protected set; }
        public IDiscovery Sender { get; protected set; }

        public Transmission(int timeSlot, IDiscovery sender)
        {
            Slot = timeSlot;
            Sender = sender;
        }

        public override string ToString()
        {
            return "TimeSlot: " + Slot + " " + Sender;
        }
    }

    //public class UConnectTransmission : Transmission<IDiscovery>
    //{
    //    public int P { get; protected set; }
    //    public UConnectTransmission(int timeSlot, IDiscovery node, int p) : base(timeSlot, node)
    //    {
    //        P = p;
    //    }
    //}

    //public class GNihaoTransmission : Transmission<IDiscovery>
    //{
    //    public GNihaoTransmission(int timeSlot, IDiscovery node) : base(timeSlot, node)
    //    {

    //    }

    //}

    //public class DiscoTransmission:Transmission<Node>
    //{
    //    public int P1 { get; protected set; }
    //    public int P2 { get; protected set; }
    //    public DiscoTransmission(int TimeSlot, Node node, double dutyCycle, int p1, int p2):base(TimeSlot, node, dutyCycle)
    //    {
    //        P1 = p1;
    //        P2 = p2;
    //    }

    //}

    //public class DiscoAccTransmission : Transmission<Node>
    //{
    //    public int P1 { get; protected set; }
    //    public int P2 { get; protected set; }

    //    /// <summary>
    //    /// period size of the budget
    //    /// </summary>
    //    public int Bp { get; protected set; }

    //    public DiscoAccTransmission(int TimeSlot, Node node, double dutyCycle, int p1, int p2) : base(TimeSlot, node, dutyCycle)
    //    {
    //        P1 = p1;
    //        P2 = p2;
    //    }

    //}

    //public class BirthdayTransmission : Transmission<Node>
    //{
    //    public BirthdayTransmission(int TimeSlot, Node node, double dutyCycle):base(TimeSlot, node, dutyCycle)
    //    {

    //    }
    //}



    //public class SearchlightTransmission : Transmission<Node>
    //{
    //    public int T { get; protected set; }
    //    public SearchlightTransmission(int TimeSlot, Node node, double dutyCycle, int t) : base(TimeSlot, node, dutyCycle)
    //    {
    //        T = t;
    //    }
    //}

    //public class TestAlgorithmTransmission : Transmission<Node>
    //{
    //    public int M { get; protected set; }


    //    public TestAlgorithmTransmission(int TimeSlot, Node node, double dutyCycle, int m) : base(TimeSlot, node, dutyCycle)
    //    {
    //        M = m;
    //    }
    //}

    //public class HelloTransmission : Transmission<Node>
    //{
    //    public int C { get; protected set; }
    //    public int N { get; protected set; }

    //    public HelloTransmission(int TimeSlot, Node node, double dutyCycle, int c, int n) : base(TimeSlot, node, dutyCycle)
    //    {
    //        C = c;
    //        N = n;

    //    }
    //}



    //public class PNihaoTransmission : Transmission<Node>
    //{
    //    //new properties here

    //    public PNihaoTransmission(int timeSlot, Node node, double dutyCycle) : base(timeSlot, node, dutyCycle)
    //    {//add parameters when needed

    //        //set necessary info here!
    //    }

    //}


    public class TransmissionCompararer : IComparer<Transmission>
    {
        public int Compare(Transmission x, Transmission y)
        {
            return x.Slot.CompareTo(y.Slot);
        }
    }
}
