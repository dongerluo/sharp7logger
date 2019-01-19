using System;
using System.Text;
using Sharp7;

namespace sharp7logger
{
    class Program
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        static S7Client Client;

        static void Usage()
        {
            Console.WriteLine("Usage");
            Console.WriteLine("  client <IP> [Rack=0 Slot=2]");
            Console.WriteLine("Example");
            Console.WriteLine("  client 192.168.1.101 0 2");
            Console.WriteLine("or");
            Console.WriteLine("  client 192.168.1.101");
            Console.ReadKey();
        }

        //------------------------------------------------------------------------------
        // HexDump, a very nice function, it's not mine.
        // I found it on the net somewhere some time ago... thanks to the author ;-)
        //------------------------------------------------------------------------------
        static void HexDump(byte[] bytes, int Size)
        {
            if (bytes == null)
                return;
            int bytesLength = Size;
            int bytesPerLine = 16;

            char[] HexChars = "0123456789ABCDEF".ToCharArray();

            int firstHexColumn =
                8                   // 8 characters for the address
                + 3;                  // 3 spaces

            int firstCharColumn = firstHexColumn
                + bytesPerLine * 3       // - 2 digit for the hexadecimal value and 1 space
                + (bytesPerLine - 1) / 8 // - 1 extra space every 8 characters from the 9th
                + 2;                  // 2 spaces 

            int lineLength = firstCharColumn
                + bytesPerLine           // - characters to show the ascii value
                + Environment.NewLine.Length; // Carriage return and line feed (should normally be 2)

            char[] line = (new String(' ', lineLength - 2) + Environment.NewLine).ToCharArray();
            int expectedLines = (bytesLength + bytesPerLine - 1) / bytesPerLine;
            StringBuilder result = new StringBuilder(expectedLines * lineLength);

            for (int i = 0; i < bytesLength; i += bytesPerLine)
            {
                line[0] = HexChars[(i >> 28) & 0xF];
                line[1] = HexChars[(i >> 24) & 0xF];
                line[2] = HexChars[(i >> 20) & 0xF];
                line[3] = HexChars[(i >> 16) & 0xF];
                line[4] = HexChars[(i >> 12) & 0xF];
                line[5] = HexChars[(i >> 8) & 0xF];
                line[6] = HexChars[(i >> 4) & 0xF];
                line[7] = HexChars[(i >> 0) & 0xF];

                int hexColumn = firstHexColumn;
                int charColumn = firstCharColumn;

                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (j > 0 && (j & 7) == 0) hexColumn++;
                    if (i + j >= bytesLength)
                    {
                        line[hexColumn] = ' ';
                        line[hexColumn + 1] = ' ';
                        line[charColumn] = ' ';
                    }
                    else
                    {
                        byte b = bytes[i + j];
                        line[hexColumn] = HexChars[(b >> 4) & 0xF];
                        line[hexColumn + 1] = HexChars[b & 0xF];
                        line[charColumn] = (b < 32 ? '·' : (char)b);
                    }
                    hexColumn += 3;
                    charColumn++;
                }
                result.Append(line);
    #if __MonoCS__
                result.Append('\n');
    #endif
            }
            Console.WriteLine(result.ToString());
        }
    
        static bool PlcConnect(string ip, int rack, int slot)
        {
            int result = Client.ConnectTo(ip, rack, slot);
            return result == 0;
        }

        static void PerformTasks()
        {
            S7Client.S7CpuInfo Info = new S7Client.S7CpuInfo();
            int result;
            
            result = Client.GetCpuInfo(ref Info);
            if (result == 0)
            {
                Console.WriteLine("  Module Type Name : " + Info.ModuleTypeName);
                Console.WriteLine("  Serial Number    : " + Info.SerialNumber);
                Console.WriteLine("  AS Name          : " + Info.ASName);
                Console.WriteLine("  Module Name      : " + Info.ModuleName);
            };

            DateTime datetime = new DateTime();
            result = Client.GetPlcDateTime(ref datetime);
            if (result == 0)
            {
                Console.WriteLine(datetime.ToLongDateString() + " - " 
                                  + datetime.ToLongTimeString());
            }

            int Size = 32;
            byte[] Buffer = new byte[32]; 
            result = Client.DBRead(1566, 80, Size, Buffer);
            if (result == 0)
            {
                Console.WriteLine("Dump : " + Size.ToString() + " bytes");
                HexDump(Buffer, Size);
            }
        }

        static void Main(string[] args)
        {
            logger.Info("Program starting...");

            // Default for S7-1500
            int rack = 0, slot = 1;

            // Get program parameters
            if ((args.Length != 1) && (args.Length != 3))
            {
                Usage();
                return;
            }
            if (args.Length == 3)
            {
                rack = Convert.ToInt32(args[1]);
                slot = Convert.ToInt32(args[2]);
            }

            Client = new S7Client();

            if (PlcConnect(args[0], rack, slot))
            {
                Console.WriteLine("PLC is connected");
                PerformTasks();
                Client.Disconnect();
            }

            logger.Info("Program finish");
        }
    }
}