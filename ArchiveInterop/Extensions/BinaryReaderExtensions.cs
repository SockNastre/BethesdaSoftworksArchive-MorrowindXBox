using System.IO;
using System.Text;

namespace ArchiveInterop.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="BinaryReader"/> class.
    /// </summary>
    public static class BinaryReaderExtensions
    {
        /// <summary>
        /// Reads null-terminated string using a <see cref="BinaryReader"/>.
        /// </summary>
        /// <param name="reader">The <see cref="BinaryReader"/> to read with.</param>
        /// <returns>Null-terminated string as <see cref="string"/> object.</returns>
        public static string ReadNullTerminatedString(this BinaryReader reader)
        {
            var sb = new StringBuilder();
            byte charByte = reader.ReadByte();

            // 0x00 is a null-terminator byte, marking end of character array
            while (charByte != 0x00)
            {
                sb.Append((char)charByte);
                charByte = reader.ReadByte();
            }

            return sb.ToString();
        }
    }
}
