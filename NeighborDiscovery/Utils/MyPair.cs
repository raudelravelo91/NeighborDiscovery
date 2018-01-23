using System;

namespace NeighborDiscovery.Utils
{
    [Serializable]
    public class MyPair
    {
        public double X { get; set; }
        public double Y { get; set; }

        public MyPair(double x, double y)
        {
            X = x;
            Y = y;
        }

        public double DistanceTo(MyPair p)
        {
            return Math.Sqrt(Math.Pow(Math.Abs(X - p.X),2)+Math.Pow(Y - p.Y,2));
        }

        public override string ToString()
        {
            return Math.Round(X,2) + " ; " + Math.Round(Y,2);
        }
    }
}