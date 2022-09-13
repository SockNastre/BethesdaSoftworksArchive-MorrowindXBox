using System.IO;

namespace ArchiveInterop.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="BinaryWriter"/> class.
    /// </summary>
    public static class BinaryWriterExtensions
    {
        /// <summary>
        /// Writes null-terminated string using a <see cref="BinaryWriter"/>.
        /// </summary>
        /// <param name="writer">The <see cref="BinaryWriter"/> to write with.</param>
        /// <param name="value">The <see cref="string"/> to write as null-terminated string.</param>
        /// <remarks>
        /// See <see cref="BinaryReaderExtensions.ReadNullTerminatedString(BinaryReader)"/> for how to read null-terminated string with <see cref="BinaryReader"/>
        /// </remarks>
        public static void WriteNullTerminatedString(this BinaryWriter writer, string value)
        {
            foreach (char c in value)
            {
                writer.Write(c);
            }

            writer.Write((byte)0x00);
        }
    }
}
