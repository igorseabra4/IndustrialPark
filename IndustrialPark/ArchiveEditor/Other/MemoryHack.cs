using HipHopFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndustrialPark
{

    /// <summary>
    /// EventHack provides functionality to utilise the UpgradePowerUp exploit, allowing
    /// arbitrary memory addresses to be incremented in The SpongeBob SquarePants Movie.
    /// </summary>
    public static class MemoryHack
    {
        /// <summary>
        /// The base address for UpgradePowerUp - the argument will increase/decrease the address
        /// that the link writes to.
        /// </summary>
        public const uint BASE_ADDRESS_TSSM = 0x804AE559;

        /// <summary>
        /// The minimum address in GameCube memory.
        /// </summary>
        public const uint MIN_ADDRESS = 0x80000000;

        /// <summary>
        /// Gets the difference between two byte arrays.
        /// </summary>
        /// <param name="modification">The memory modification</param>
        /// <returns>The byte array containing the differences between corresponding bytes</returns>
        public static byte[] GetByteDifference(MemoryModification modification)
        {
            int numBytes = modification.DataType == MemoryHackDataType.UINT8 ? 1 : 4;

            byte[] result = new byte[numBytes];

            for (int i = 0; i < numBytes; i++)
            {
                // Calculate difference between bytes; this is the number of increments
                // required for the UpgradePowerUp hack.
                result[i] = (byte)(modification.NewValue[i] - modification.OldValue[i]);
            }

            return result;
        }

        /// <summary>
        /// Generates the UpgradePowerUp Links from a list of arguments.
        /// </summary>
        /// <param name="arguments">The arguments (offsets for the exploit)</param>
        /// <param name="game">The game</param>
        /// <param name="eventReceiveID">The event receive ID</param>
        /// <param name="playerAssetID">The Player asset ID</param>
        /// <returns></returns>
        public static List<Link> GetLinks(List<float> arguments, Game game,
            ushort eventReceiveID, AssetID playerAssetID)
        {
            List<Link> links = new List<Link>();

            foreach (var argument in arguments)
            {
                Link newLink = new Link(game);
                newLink.FloatParameter1 = argument;
                newLink.TargetAsset =  playerAssetID;
                newLink.EventReceiveID = eventReceiveID;
                newLink.EventSendID = (ushort)EventTSSM.UpgradePowerUp;

                links.Add(newLink);
            }

            return links;
        }

        /// <summary>
        /// Gets the three events needed to enable the UpgradePowerUp exploit.
        /// If these events are not used beforehand, the UpgradePowerUp will not work if the byte reaches the cap.
        /// </summary>
        /// <param name="playerAssetID">The Player asset ID</param>
        /// <param name="eventReceiveID">The event receive ID</param>
        /// <returns></returns>
        public static List<Link> GetUpgradePowerUpHackEvents(AssetID playerAssetID, ushort eventReceiveID)
        {
            float[] arguments = new float[]
            {
                -3613300.0000f,
                -3613291.0000f,
                -3613263.0000f
            };

            List<Link> links = new List<Link>();
            
            foreach (var argument in arguments)
            {
                Link newLink = new Link(Game.Incredibles);
                newLink.FloatParameter1 = argument;
                newLink.TargetAsset = playerAssetID;
                newLink.EventReceiveID = eventReceiveID;
                newLink.EventSendID = (ushort)EventTSSM.GivePowerUp;

                links.Add(newLink);
            }
            
            return links;
        }

        /// <summary>
        /// Gets the offset between the provided memory address and <see cref="BASE_ADDRESS_TSSM"/>.
        /// This is used as the argument for the <see cref="Link"/>.
        /// </summary>
        /// <param name="memoryAddress">The memory address</param>
        /// <returns>The offset from <see cref="BASE_ADDRESS_TSSM"/></returns>
        /// <exception cref="ArgumentException"></exception>
        public static long GetOffset(uint memoryAddress)
        {
            if (memoryAddress < MIN_ADDRESS)
            {
                throw new ArgumentException("Memory address too small. Must be at least 0x80000000", 
                    nameof(memoryAddress));
            }

            return (long)memoryAddress - (long)BASE_ADDRESS_TSSM;
        }

        /// <summary>
        /// Gets the offsets needed to change the memory value from the old value to the new value.
        /// The offsets are provided as arguments to the <see cref="Link"/>s.
        /// </summary>
        /// <param name="modification"></param>
        /// <param name="byteDifference"></param>
        /// <returns></returns>
        public static List<float> GetEventOffsetsAsFloats(MemoryModification modification,
            byte[] byteDifference)
        {
            List<float> arguments = new List<float>();
            byte currentByteDifference;
            uint byteMemoryAddress;

            for (uint subByte = 0; subByte < byteDifference.Length; subByte++)
            {
                currentByteDifference = byteDifference[subByte];

                // The bytes in the old and new values are the same (difference is 0).
                if (currentByteDifference == 0)
                    continue;

                // Get the memory address of the byte.
                byteMemoryAddress = modification.MemoryAddress + subByte;

                for (int i = 0; i < currentByteDifference; i++)
                {
                    // Get the argument for each link, and append to the List<float>
                    // They are floats because link arguments are floats.
                    arguments.Add((float)GetOffset(byteMemoryAddress));
                }
            }

            return arguments;
        }

        /// <summary>
        /// Gets the bytes used to represent the given numeric value.
        /// </summary>
        /// <typeparam name="T">The numeric datatype (byte/int32/single)</typeparam>
        /// <param name="value">The value</param>
        /// <param name="type">The datatype of the value</param>
        /// <returns>The bytes used to represent the value</returns>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] GetBytes<T>(T value, MemoryHackDataType type)
        {
            // Check endianness for int32/float32
            bool isLittleEndian = BitConverter.IsLittleEndian;

            switch (type)
            {
                case MemoryHackDataType.UINT8:
                    return BitConverter.GetBytes(Convert.ToByte(value));
                    // Endianness doesn't matter for one byte
                case MemoryHackDataType.INT32:
                    byte[] bytesInt32 = BitConverter.GetBytes(Convert.ToInt32(value));
                    if (isLittleEndian)
                    {
                        Array.Reverse(bytesInt32);
                    }
                    return bytesInt32;
                case MemoryHackDataType.FLOAT32:
                    byte[] bytesFloat32 = BitConverter.GetBytes(Convert.ToSingle(value));
                    if (isLittleEndian)
                    {
                        Array.Reverse(bytesFloat32);
                    }
                    return bytesFloat32;
                default:
                    throw new ArgumentException("Invalid datatype.", nameof(type));
            }
        }
    }
}
