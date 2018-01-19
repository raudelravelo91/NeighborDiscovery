using System;
using NeighborDiscovery.Utils;

namespace NeighborDiscovery.Networks
{
    public class Network2DNode
    {
        public static double Epsilon = 1e-6;
        public int Node2DId { get; }
        public MyPair Position { get; set; }
        public double Speed { get; set; }
        public int CommunicationRange { get; set; }
        public MyPair Direction { get; private set; }
        

        public Network2DNode(int id, MyPair position, MyPair direction, int commRange, double speed = 0)
        {
            Node2DId = id;
            Position = new MyPair(position.X, position.Y);
            CommunicationRange = commRange;
            Speed = speed;
            Direction = new MyPair(direction.X, direction.Y);
        }

        public void SetDirection(double x, double y)
        {
            Direction = new MyPair(x, y);
            if (Math.Abs(Speed) < Epsilon)
                return;
            UpdateDirection();
        }

        public bool NodeIsInRange(Network2DNode other)
        {
            return Position.DistanceTo(other.Position) <= CommunicationRange;
        }

        public void Move()
        {
            if (Math.Abs(Speed) < Epsilon)
                return;

            if (Math.Abs(Speed - GetVectorLength()) > Epsilon)
                UpdateDirection();

            Position.X += Direction.X;
            Position.Y += Direction.Y;
        }

        private void UpdateDirection()
        {
            //todo
        }

        private double GetVectorLength()
        {
            var xx = Direction.X * Direction.X;
            var yy = Direction.Y * Direction.Y;
            var sqrt = Math.Sqrt(xx + yy);
            return sqrt;
        }

        public override string ToString()
        {
            return "Node2DId " + Node2DId  + " Position: (" + Position.ToString() + ")";
        }
    }
}