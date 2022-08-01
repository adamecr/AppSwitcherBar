using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace net.adamec.ui.AppSwitcherBar.Win32.Services.JumpLists
{
    /// <summary>
    /// Application ID (AMUI) CRC64 algorithm
    /// </summary>
    public class AppIdCrc64 : HashAlgorithm
    {
        public const ulong DefaultSeed = 0x0;
        /// <summary>
        /// Non standard polynomial is used for AppId CRC64
        /// </summary>
        public const ulong AppIdPolynomial = 0x92C64265D32139A4;

        /// <summary>
        /// CRC Table
        /// </summary>
        private static readonly ulong[] Table;

        /// <summary>
        /// calculated hash (CRC64)
        /// </summary>
        private ulong hash;

        /// <summary>
        /// static CTOR
        /// </summary>
        static AppIdCrc64()
        {
            Table = InitializeTable();
        }

        /// <summary>
        /// CTOR
        /// </summary>
        /// <exception cref="PlatformNotSupportedException">Not supported on Big Endian processors</exception>
        public AppIdCrc64()
        {
            if (!BitConverter.IsLittleEndian) throw new PlatformNotSupportedException("Not supported on Big Endian processors");

            hash = DefaultSeed;
        }


        /// <summary>
        /// Resets the algorithm to its original state
        /// </summary>
        public override void Initialize()
        {
            hash = DefaultSeed;
        }

        /// <summary>
        /// Size (in bits) of calculated CRC64
        /// </summary>
        public override int HashSize => 64;

        /// <inheritdoc/>
        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            hash = CalculateHash(array, ibStart, cbSize);
        }

        /// <summary>
        /// Finalizes the CRC64 computation
        /// </summary>
        protected override byte[] HashFinal()
        {
            var hashBuffer = UInt64ToBigEndianBytes(hash);
            HashValue = hashBuffer;
            return hashBuffer;
        }

        /// <summary>
        /// Computes the AppId's CRC64 from given unicode string <paramref name="buffer"/> with AppId
        /// </summary>
        /// <param name="buffer">Unicode string <paramref name="buffer"/> with AppId</param>
        /// <returns>AppId's CRC64</returns>
        public static ulong Compute(byte[] buffer)
        {
            return CalculateHash(buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Internal AppId CRC64 has calculation
        /// </summary>
        /// <param name="buffer">Source buffer</param>
        /// <param name="start">Start index of the source buffer</param>
        /// <param name="size">Source buffer size</param>
        /// <returns></returns>
        private static ulong CalculateHash(IList<byte> buffer, int start, int size)
        {
            var hash = 0xFFFFFFFFFFFFFFFF; //DefaultSeed;
            for (var i = start; i < start + size; i++)
                unchecked
                {
                    hash = hash >> 8 ^ Table[(hash ^ buffer[i]) & 0xff];//buffer[i] ^ hash
                }
            return hash;
        }

        /// <summary>
        /// Converts the CRC64 from <see cref="ulong"/> (number) to big endian buffer that can be further transformed to "hex string"
        /// </summary>
        /// <param name="value">CRC64 value</param>
        /// <returns>CRC64 value as big endian bytes buffer</returns>
        private static byte[] UInt64ToBigEndianBytes(ulong value)
        {
            var result = BitConverter.GetBytes(value);

            if (BitConverter.IsLittleEndian) Array.Reverse(result);

            return result;
        }

        /// <summary>
        /// Initialize CRC table
        /// </summary>
        /// <returns>CRC table</returns>
        protected static ulong[] InitializeTable()
        {
            var createTable = new ulong[256];
            for (var i = 0; i < 256; ++i)
            {
                var entry = (ulong)i;
                for (var j = 0; j < 8; ++j)
                    if ((entry & 1) == 1)
                        entry = entry >> 1 ^ AppIdPolynomial;
                    else
                        entry >>= 1;
                createTable[i] = entry;
            }
            return createTable;
        }
    }
}
