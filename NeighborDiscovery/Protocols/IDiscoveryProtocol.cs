using System;
using System.Collections.Generic;

namespace NeighborDiscovery.Protocols
{
    public interface IDiscoveryProtocol
    {
        /// <summary>
        /// the Discovery Device's ID
        /// </summary>
        int Id { get; }

        /// <summary>
        /// represent the internal time slot the device is currently at
        /// </summary>
        int InternalTimeSlot { get; }

        /// <summary>
        /// returns the device's current transmission, null if the device is not trasmitting at the moment
        /// A good way to know if it is transmitting is to call the IsTransmitting() method before
        /// </summary>
        /// <returns></returns>
        ITransmission GetTransmission();

        /// <summary>
        /// The current neighbors
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDiscoveryProtocol> Neighbors();


        /// <summary>
        /// returns true if the given node have been discovered before
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        bool ContainsNeighbor(IDiscoveryProtocol device);

        /// <summary>
        /// Get the IContact info of the given device
        /// </summary>
        /// <param name="device"></param>
        /// <returns>if there is no contact info about the given device, the method returns null</returns>
        IContact GetContactInfo(IDiscoveryProtocol device);

        /// <summary>
        /// make a node listen to a given transmission
        /// </summary>
        /// <param name="transmission">the given transmission</param>
        void ListenTo(ITransmission transmission);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>true if the node is listening in the current moment, false otherwise</returns>
        bool IsListening();

        /// <summary>
        /// if this methods returns false then a call to the GetTransmission() method returns null
        /// </summary>
        /// <returns>true if the node is transmitting at the current moment, false otherwise</returns>
        bool IsTransmitting();

        /// <summary>
        /// Move the internal state of the node to the next time slot (the device's InternalTimeSlot should increase by one)
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
        void SetDutyCycle(double value);

        /// <summary>
        /// reset a Discovery Device to its initial state
        /// </summary>
        void Reset();

        /// <summary>
        /// just clone it up
        /// </summary>
        /// <returns></returns>
        IDiscoveryProtocol Clone();

        /// <summary>
        /// 
        /// </summary>
        event EventHandler<INodeResult> OnDeviceDiscovered;

    }
}
