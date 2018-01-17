using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NeighborDiscovery.Environment;

namespace NeighborDiscovery.Protocols
{
    public interface IDiscoveryProtocol
    {
        /// <summary>
        /// the Discovery Device's ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// The Discovery Device's HyperPeriod
        /// </summary>
        int T { get; }

        /// <returns>The Discovery Device's next transmission</returns>
        /// //It is supposed that a calling to this method modifies the internal state of a Discovery Device
        Transmission GetTransmission();

        /// <summary>
        /// The list of tuples composed by the Discovered Device and the slot when they last met
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<IDiscoveryProtocol, ContactInfo>> Neighbors();

        /// <summary>
        /// make a node listen to a given transmission at a given time
        /// </summary>
        /// <param name="transmission">the given transmission</param>
        /// <returns>return the list of discovered nodes by the listened transmission</returns>
        IEnumerable<IDiscoveryProtocol> ListenTo(Transmission transmission);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the node is listening in the current moment, false otherwise</returns>
        bool IsListening();

        /// <summary>
        /// if this methods returns false then the GetTransmission method returns null
        /// </summary>
        /// <returns>true if the node is transmitting at the current moment, false otherwise</returns>
        bool IsTransmitting();

        /// <summary>
        /// Move next the internal state of the node
        /// </summary>
        void MoveNext();

        /// <summary>
        /// get the current duty cycle the device is working at
        /// </summary>
        /// <returns></returns>
        double GetDutyCycle();

        /// <summary>
        /// puts the device to work at the desired duty cycle
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        double SetDutyCycle(double value);

        /// <summary>
        /// reset a Discovery Device to its initial state
        /// </summary>
        void Reset();
    }
}
