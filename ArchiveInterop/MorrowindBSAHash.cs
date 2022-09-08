using System;

namespace ArchiveInterop
{
    /// <summary>
    /// Provides static methods for hashing strings for PC Morrowind BSAs.
    /// </summary>
    public static class MorrowindBSAHash
    {
        /// <summary>
        /// From <see href="https://en.uesp.net/wiki/Morrowind_Mod:BSA_File_Format#C.23">UESP wiki</see>.
        /// </summary>
        private static uint RotateRight(uint value, int numBits) => value << (32 - numBits) | value >> numBits;

        /// <summary>
        /// From <see href="https://en.uesp.net/wiki/Morrowind_Mod:BSA_File_Format#C.23">UESP wiki</see>.
        /// Hashes string based on PC hashing algorithm.
        /// </summary>
        /// <param name="name">Name (entry string of file) to hash.</param>
        /// <returns>Hashed name and extension.</returns>
        public static ulong GetPC(string name) => MorrowindBSAHash.GetPCLow(name) | (ulong)MorrowindBSAHash.GetPCHigh(name) << 32;

        /// <summary>
        /// From <see href="https://en.uesp.net/wiki/Morrowind_Mod:BSA_File_Format#C.23">UESP wiki</see>.
        /// Gets lower part of hash.
        /// </summary>
        /// <param name="name">Name (entry string of file) to hash.</param>
        /// <returns>Hashed name and extension (lower part).</returns>
        public static uint GetPCLow(string name)
        {
            name = name.ToLowerInvariant();
            int midPoint = name.Length >> 1;
            var low = new byte[4];

            for (var i = 0; i < midPoint; i++)
            {
                low[i & 3] ^= (byte)name[i];
            }

            return BitConverter.ToUInt32(low, 0);
        }

        /// <summary>
        /// From <see href="https://en.uesp.net/wiki/Morrowind_Mod:BSA_File_Format#C.23">UESP wiki</see>.
        /// Gets higher part of hash.
        /// </summary>
        /// <param name="name">Name (entry string of file) to hash.</param>
        /// <returns>Hashed name and extension (higher part).</returns>
        public static uint GetPCHigh(string name)
        {
            name = name.ToLowerInvariant();
            int midPoint = name.Length >> 1;

            uint high = 0U;
            for (var i = midPoint; i < name.Length; i++)
            {
                var temp = (uint)name[i] << (((i - midPoint) & 3) << 3);
                high = RotateRight(high ^ temp, (int)(temp & 0x1F));
            }

            return high;
        }
    }
}
