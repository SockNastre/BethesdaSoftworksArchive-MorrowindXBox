using ArchiveInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PackerCLI
{
    class Program
    {
        static void Main(string[] args)
        {
            // If there are no arguments passed then it acts as if help command is ran
            if (args.Count() == 0)
            {
                Program.ShowHelpText();
                return;
            }

            switch (args[0].ToLower())
            {
                default:
                    {
                        if (args.Count() == 1 && Directory.Exists(args[0]))
                        {
                            // If a single existing folder is inputted, the tool will autopack that
                            // The following codes sets up that process

                            string inputDir = args[0];
                            args = new string[5];
                            args[0] = string.Empty; // Can equal blank string value

                            args[1] = "-i";
                            args[2] = inputDir;

                            args[3] = "-o";
                            args[4] = Path.GetDirectoryName(inputDir) + "\\output.bsa";

                            goto case "-p";
                        }
                        else if (args.Count() == 1 && File.Exists(args[0]))
                        {
                            // If a single existing file is inputted, the tool will unpack that
                            // The following codes sets up that process

                            string inputBSA = args[0];
                            args = new string[7];
                            args[0] = string.Empty; // Can equal blank string value

                            args[1] = "-hashtable";
                            args[2] = AppDomain.CurrentDomain.BaseDirectory + "MW_Hash_Table.bin";

                            args[3] = "-i";
                            args[4] = inputBSA;

                            args[5] = "-o";
                            args[6] = Path.GetDirectoryName(inputBSA) + "\\" + Path.GetFileNameWithoutExtension(inputBSA) + "_DataFiles";

                            goto case "-u";
                        }

                        Program.ShowHelpText(true);
                        break;
                    }

                case "-h":
                case "-help":
                    {
                        Program.ShowHelpText();
                        break;
                    }

                case "-p":
                case "-pack":
                    {
                        string inputDir = string.Empty;
                        string outputPath = string.Empty;

                        for (uint i = 1; i < args.Count(); i++)
                        {
                            string option = args[i].ToLower();

                            switch (option)
                            {
                                default:
                                    {
                                        Program.ShowHelpText(true);
                                        return;
                                    }

                                case "-i":
                                case "-indir":
                                    {
                                        i++;
                                        inputDir = args[i];
                                        break;
                                    }

                                case "-o":
                                case "-out":
                                    {
                                        i++;
                                        outputPath = args[i];
                                        break;
                                    }
                            }
                        }

                        if (string.IsNullOrEmpty(inputDir) || string.IsNullOrEmpty(outputPath))
                        {
                            Program.ShowHelpText(true);
                            return;
                        }

                        Console.WriteLine("Reading and verifying files...");
                        var assetList = new List<Asset>();

                        foreach (string file in Directory.GetFiles(inputDir, "*", SearchOption.AllDirectories))
                        {
                            var assetFile = new Asset(file.Substring(inputDir.Length + 1), file);
                            assetList.Add(assetFile);
                        }
                        assetList = assetList.OrderBy(asset => asset.Index).ToList();

                        Console.WriteLine("Packing...");
                        BSA.Write(outputPath, assetList);

                        Console.WriteLine("Done!");
                        break;
                    }

                case "-u":
                case "-unpack":
                    {
                        string inputBSA = string.Empty;
                        string outputPath = string.Empty;

                        string hashTablePath = string.Empty;

                        for (uint i = 1; i < args.Count(); i++)
                        {
                            string option = args[i].ToLower();

                            switch (option)
                            {
                                default:
                                    {
                                        Program.ShowHelpText(true);
                                        return;
                                    }

                                case "-hashtable":
                                    {
                                        i++;
                                        hashTablePath = args[i];
                                        break;
                                    }

                                case "-i":
                                case "-inbsa":
                                    {
                                        i++;
                                        inputBSA = args[i];
                                        break;
                                    }

                                case "-o":
                                case "-outdir":
                                    {
                                        i++;
                                        outputPath = args[i];
                                        break;
                                    }
                            }
                        }

                        if (string.IsNullOrEmpty(inputBSA) || string.IsNullOrEmpty(outputPath))
                        {
                            Program.ShowHelpText(true);
                            return;
                        }

                        Console.WriteLine("Unpacking...");
                        BSA.Unpack(inputBSA, outputPath, hashTablePath);

                        Console.WriteLine("Done!");
                        break;
                    }
            }
        }

        private static void ShowHelpText(bool isInvalidUsage = false)
        {
            if (isInvalidUsage)
            {
                Console.WriteLine("Invalid usage\n");
            }

            Console.WriteLine("BethesdaSoftworksArchive MorrowindXBox Packer Cli\nCopyright (c) 2022  SockNastre\nVersion: 1.0.0.0\n\n" + new string('-', 50) +
                "\n\nUsage: \"BSA MorrowindXBox Packer Cli.exe\" <Command> <Options>\n\nCommands:\n-pack (-p)\n-unpack (-u)\n-help (-h)\n\n" +
                "Pack Options:\n-indir (-i)\n-out (-o)\n\n" +
                "Unpack Options:\n-inbsa (-i)\n-outdir (-o)\n-hashtable\n\n" +
                "Examples:\n\n\"BSA MorrowindXBox Packer Cli.exe\" -pack -i \"C:\\DataFiles\" -o \"C:\\xboxoutput.bsa\"\n" +
                "\"BSA MorrowindXBox Packer Cli.exe\" -unpack -i \"C:\\Morrowind.bsa\" -o \"C:\\DataFiles\"");
        }
    }
}
