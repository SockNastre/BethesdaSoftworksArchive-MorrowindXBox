namespace ArchiveInterop
{
    /// <summary>
    /// Contains metadata that aids with the BSA packing process.
    /// </summary>
    public class Asset
    {
        public string EntryStr { get; set; }
        public string RealPath { get; set; }

        public uint Index { get; set; }
        public ulong Hash { get; set; }

        public ulong GenerateHash => MorrowindBSAHash.GetPC(this.EntryStr);
        public uint GenerateHashLow => this.EntryStr.Equals("textures\\gambtns.dds") ? 119369821 : MorrowindBSAHash.GetPCLow(this.EntryStr);
        public uint GenerateHashHigh => this.EntryStr.Equals("textures\\gambtns.dds") ? 3440605484 : MorrowindBSAHash.GetPCHigh(this.EntryStr);

        // These fields get set during packing
        public uint Size { get; set; }
        public uint Offset { get; set; }

        /// <summary>
        /// Creates asset class with entry string and real filesystem path fields set.
        /// </summary>
        /// <param name="entryStr">Path to be written in BSA.</param>
        /// <param name="realPath">Real filesystem path to find asset.</param>
        public Asset(string entryStr, string realPath)
        {
            this.EntryStr = entryStr;
            this.RealPath = realPath;

            // Old code for checking if hash was the name
            /*if (isCheckPrePackedHash)
            {
                string[] nameParts = this.EntryStr.Split('_');

                if (nameParts.Length != 2)
                {
                    return;
                }

                uint index = UInt32.Parse(nameParts[0]);
                this.Index = index;
                string hash = nameParts[1];

                string loweredEntryStr = Path.GetFileNameWithoutExtension(hash).ToLowerInvariant();

                if (loweredEntryStr.Length <= 16 && loweredEntryStr.Length >= 14)
                {
                    string allowableCharacters = "0123456789abcdef";
                    this.IsPrePackedHash = true;

                    // https://stackoverflow.com/a/3293315
                    foreach (char c in loweredEntryStr)
                    {
                        if (!allowableCharacters.Contains(c.ToString()))
                        {
                            this.IsPrePackedHash = false;
                            break;
                        }
                    }

                    // Padding entry string with leading zeros
                    if (loweredEntryStr.Length <= 16)
                    {
                        this.EntryStr = loweredEntryStr.PadLeft(16, '0');
                    }

                    for (int i = 0; i < 16; i += 2)
                    {
                        string byteChars = this.EntryStr[i].ToString() + this.EntryStr[i + 1].ToString();
                        this.PrePackedHash[8 - (i/2) - 1] = Convert.ToByte(byteChars, 16);
                    }
                }
            }*/
        }
    }
}
