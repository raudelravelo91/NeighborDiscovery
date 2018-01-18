using System;

namespace NeighborDiscovery.Protocols
{
    public class Transmission:ITransmission
    {
        //public int Slot { get; protected set; }
        public IDiscoveryProtocol Sender { get; protected set; }

        public Transmission(IDiscoveryProtocol sender)
        {
            Sender = sender;
        }

        public override string ToString()
        {
            return Sender.ToString();
        }
    }

    //public class UConnectTransmission : Transmission<DiscoverableDevice>
    //{
    //    public int P { get; protected set; }
    //    public UConnectTransmission(int timeSlot, DiscoverableDevice node, int p) : base(timeSlot, node)
    //    {
    //        P = p;
    //    }
    //}

    //public class GNihaoTransmission : Transmission<DiscoverableDevice>
    //{
    //    public GNihaoTransmission(int timeSlot, DiscoverableDevice node) : base(timeSlot, node)
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


    //public class TransmissionCompararer : IComparer<Transmission>
    //{
    //    public int Compare(Transmission x, Transmission y)
    //    {
    //        return x.Slot.CompareTo(y.Slot);
    //    }
    //}
}
