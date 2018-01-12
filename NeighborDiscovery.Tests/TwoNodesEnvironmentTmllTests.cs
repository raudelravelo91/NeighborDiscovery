using System;
using NeighborDiscovery.Environment;
using NeighborDiscovery.Nodes;
using NUnit.Framework;

namespace NeighborDiscovery.Tests
{
    [TestFixture]
    public class TwoNodesEnvironmentTmllTests
    {

        [Test]
        public void SameOffSetAndSameStartUpSlot()
        {
            //Arrange
            IDiscovery node1 = new BNihao(0, 10, 100, 10, 1);
            IDiscovery node2 = new BNihao(1, 10, 100, 10, 1);
            var env = new TwoNodesEnvironmentTmll(node1, node2);
            
            //Act
            var latency = env.RunTwoNodesSimulation(node1, node2, 0, 1, 100);
            
            //Assert
            Assert.That(latency.Item1, Is.EqualTo(0));
            Assert.AreEqual(latency.Item1, latency.Item2);
        }

    }
}
