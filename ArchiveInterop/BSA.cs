using ArchiveInterop.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ArchiveInterop
{
    public static class BSA
    {
        /// <summary>
        /// Writes BSA for Morrowind XBox.
        /// </summary>
        /// <param name="path">Real filesystem path to write BSA to.</param>
        /// <param name="assetList">List of <see cref="Asset"/> to write in BSA.</param>
        public static void Write(string path, List<Asset> assetList, bool isStringTableWritten = false)
        {
            // Used for counting progress for console printing
            uint count = 0;

            using (var writer = new BinaryWriter(File.Open(path, FileMode.Create, FileAccess.Write, FileShare.Read)))
            {
                writer.Write(0x00000100); // Archive magic
                writer.Write(-1); // Temporary, hash table offset (minus 12)
                writer.Write(assetList.Count);

                // Writing temporary asset records
                foreach (Asset asset in assetList)
                {
                    // Temporary, asset size + offset
                    writer.Write(-1);
                    writer.Write(-1);
                }

                // Whether to write the string table or not
                if (isStringTableWritten)
                {
                    // Sorting asset list by the low part of the hash, then by high part (required)
                    assetList = assetList.OrderBy(o => o.GenerateHashLow).ThenBy(o => o.GenerateHashHigh).ToList();

                    // Writing name table offsets (rather than write offsets we'll just keep incrementing a size and writing that)
                    uint currentNameTableSize = 0;
                    foreach (Asset asset in assetList)
                    {
                        writer.Write(currentNameTableSize);
                        currentNameTableSize += (uint)asset.EntryStr.Length + 1;
                    }

                    // Writing asset names in name table
                    foreach (Asset asset in assetList)
                    {
                        writer.WriteNullTerminatedString(asset.EntryStr);
                    }
                }

                // Will use this later to write back to header
                var hashTableOffset = (uint)writer.BaseStream.Position - 12;

                // Writing temporary asset hash table
                foreach (Asset asset in assetList)
                {
                    // Temporary, asset hash
                    writer.Write((long)-1);
                }

                // Will use this later when writing asset offsets (they are relative to data)
                var dataOffset = (uint)writer.BaseStream.Position - 12;

                // Sorting asset list by entry string (does not seem to matter much)
                assetList = assetList.OrderBy(o => o.EntryStr).ToList();

                // Writing asset byte data
                foreach (Asset asset in assetList)
                {
                    byte[] assetData = File.ReadAllBytes(asset.RealPath);

                    asset.Size = (uint)assetData.Length;
                    asset.Offset = (uint)writer.BaseStream.Position;

                    writer.Write(assetData);
                    Console.Write($"\r{++count} / {assetList.Count()}");
                }
                Console.Write('\r' + new string(' ', Console.WindowWidth - 1));
                count = 0;

                // Going back to hash table offset in header, writing new one
                writer.BaseStream.Position = 0x04;
                writer.Write(hashTableOffset);

                // Sorting asset list by the low part of the hash, then by high part (required)
                assetList = assetList.OrderBy(o => o.GenerateHashLow).ThenBy(o => o.GenerateHashHigh).ToList();

                // Going to asset records offset, writing new asset records
                writer.BaseStream.Position = 0x0C;
                foreach (Asset asset in assetList)
                {
                    writer.Write(asset.Size);
                    writer.Write(asset.Offset - dataOffset - 12);

                    Console.Write($"\r{++count} / {assetList.Count()}");
                }
                Console.Write('\r' + new string(' ', Console.WindowWidth - 1));
                count = 0;

                // Writing asset hash table
                writer.BaseStream.Position = hashTableOffset + 12;
                foreach (Asset asset in assetList)
                {
                    // This asset entry string name is special, and uses a different hash
                    if (asset.EntryStr.Equals("textures\\gambtns.dds"))
                    {
                        writer.Write(14777288032337621085);
                    }
                    else
                    {
                        writer.Write(MorrowindBSAHash.GetPC(asset.EntryStr));
                    }

                    Console.Write($"\r{++count} / {assetList.Count()}");
                }
                count = 0;
                Console.WriteLine();
            }
        }

        /// <summary>
        /// Unpacks BSA for Morrowind XBox.
        /// </summary>
        /// <param name="path">Real filesystem path to BSA to unpack.</param>
        /// <param name="output">Where to output all assets in BSA.</param>
        /// <param name="hashTablePath">Hash table to use for BSA file names.</param>
        public static void Unpack(string path, string output, string hashTablePath = "")
        {
            // Used for counting progress for console printing
            uint count = 0;

            // Creating output directory in case it does not exist
            Directory.CreateDirectory(output);

            using (var reader = new BinaryReader(File.Open(path, FileMode.Open, FileAccess.Read, FileShare.Read)))
            {
                // Checking header
                if (reader.ReadInt32() != 0x00000100)
                {
                    throw new Exception("Bad BSA header magic.");
                }

                // Reading header, setting up appropriate metadata
                uint hashTableOffset = reader.ReadUInt32() + 12;
                uint fileCount = reader.ReadUInt32();
                uint dataOffset = hashTableOffset + fileCount * 8;
                var assetList = new List<Asset>();
                bool isNameTable = false;

                // Reading asset record data
                for (uint fileIndex = 0; fileIndex < fileCount; fileIndex++)
                {
                    var asset = new Asset("", "")
                    {
                        Size = reader.ReadUInt32(),
                        Offset = reader.ReadUInt32() + dataOffset,
                        Index = fileIndex
                    };

                    assetList.Add(asset);
                    Console.Write($"\r{++count} / {assetList.Count()}");
                }
                Console.Write('\r' + new string(' ', Console.WindowWidth - 1));
                count = 0;

                // Reading hash table if a path was given
                var hashDict = new Dictionary<ulong, string>();
                if (!string.IsNullOrEmpty(hashTablePath) && File.Exists(hashTablePath))
                {
                    using (var htReader = new BinaryReader(File.Open(hashTablePath, FileMode.Open, FileAccess.Read, FileShare.Read)))
                    {
                        htReader.BaseStream.Position = 0x0C;
                        uint hashCount = htReader.ReadUInt32();

                        for (uint hashIndex = 0; hashIndex < hashCount; hashIndex++)
                        {
                            hashDict.Add(htReader.ReadUInt64(), htReader.ReadNullTerminatedString());
                            Console.Write($"\r{++count} / {assetList.Count()}");
                        }
                        Console.Write('\r' + new string(' ', Console.WindowWidth - 1));
                        count = 0;
                    }
                }

                // Checking for name table
                if (reader.ReadInt32() == 0)
                {
                    isNameTable = true;
                }
                reader.BaseStream.Position -= 4;

                // Handling name table if it is there
                if (isNameTable)
                {
                    // Getting name offsets
                    var nameOffsetArray = new uint[assetList.Count];
                    for (uint nameOffsetIndex = 0; nameOffsetIndex < assetList.Count; nameOffsetIndex++)
                    {
                        nameOffsetArray[nameOffsetIndex] = reader.ReadUInt32();
                    }

                    // Getting name strings
                    long nameTableOffset = reader.BaseStream.Position;
                    for (uint nameOffsetIndex = 0; nameOffsetIndex < assetList.Count; nameOffsetIndex++)
                    {
                        reader.BaseStream.Position = nameTableOffset + nameOffsetArray[nameOffsetIndex];
                        assetList[(int)nameOffsetIndex].EntryStr = reader.ReadNullTerminatedString();
                    }
                }

                // Getting asset hashes to find their entry strings, if no match is found hash is used as name
                reader.BaseStream.Position = hashTableOffset;
                foreach (Asset asset in assetList)
                {
                    asset.Hash = reader.ReadUInt64();

                    if (string.IsNullOrEmpty(asset.EntryStr))
                    {
                        if (hashDict.ContainsKey(asset.Hash))
                        {
                            asset.EntryStr = hashDict[asset.Hash];
                        }
                        else
                        {
                            asset.EntryStr = asset.Hash.ToString("X2");
                        }
                    }

                    Console.Write($"\r{++count} / {assetList.Count()}");
                }
                Console.Write('\r' + new string(' ', Console.WindowWidth - 1));
                count = 0;

                // Going to asset offsets and unpacking
                foreach(Asset asset in assetList)
                {
                    reader.BaseStream.Position = asset.Offset;

                    Directory.CreateDirectory(output + '\\' + Path.GetDirectoryName(asset.EntryStr));
                    File.WriteAllBytes(output + '\\' + asset.EntryStr, reader.ReadBytes((int)asset.Size));

                    Console.Write($"\r{++count} / {assetList.Count()}");
                }
                count = 0;
                Console.WriteLine();
            }
        }
    }
}
