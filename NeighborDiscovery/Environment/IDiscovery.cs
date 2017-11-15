using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeighborDiscovery.Environment
{
    public interface IDiscovery
    {
        /// <summary>
        /// the Discovery Device's ID
        /// </summary>
        int ID { get;}

        /// <summary>
        /// The Discovery Device's HyperPeriod
        /// </summary>
        int T { get; }

        int StartUpTime { get; }

        /// </summary>
        /// <returns>The Discovery Device's next transmission</returns>
        /// //It is supposed that a calling to this method modifies the internal state of a Discovery Device
        Transmission NextTransmission();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        Transmission FirstTransmissionAfter(int slot);

        /// <summary>
        /// The list of tuples composed by the Discovered Device and the slot when they last met
        /// </summary>
        /// <returns></returns>
        IEnumerable<Tuple<IDiscovery, ContactInfo>> NeighborsDiscovered();

        int NumberOfNeighbors { get; }

        /// <summary>
        /// make a node listen to a given transmission at a given time
        /// </summary>
        /// <param name="transmission">the given transmission</param>
        /// <returns>return true if a node is discovered by listening to the given transmission, false otherwise</returns>
        bool ListenTo(Transmission transmission, out List<IDiscovery> discoveredNodes);

        bool IsListening(int slot);

        bool IsTransmitting(int slot);

        double GetDutyCycle();

        /// <summary>
        /// reset a Discovery Device to its initial state
        /// </summary>
        void Reset(int startUpTime);

        IDiscovery Clone();
    }
}
