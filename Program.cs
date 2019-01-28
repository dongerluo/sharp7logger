using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    
        static bool PlcConnect(string ip, int rack, int slot)
        {
            int result = Client.ConnectTo(ip, rack, slot);
            return result == 0;
        }

        static void PerformTasks()
        {
            int result;
            int startAddress = 14;
            int numByteInEachEntry = 418;
            int numEntries = 100;
            int bufferSize = numByteInEachEntry * numEntries;
            byte[] heartbeat = { 0, 1 };
            byte[] logBuffer = new byte[bufferSize];
            HashSet<int> currentLogEntrySet = new HashSet<int>();
            HashSet<int> lastLogEntrySet = new HashSet<int>();
            LogEntry logEntry = new LogEntry();

            // TODO: Improve with aync using IHostedService from .NET Core 2.1
            // PollingFromDBAsync();
            while (true)
            {
                // Toggle the heartbeat byte
                heartbeat[1] = (byte)((heartbeat[1] == 0) ? 1 : 0);
                result = Client.DBWrite(1566, 0, 2, heartbeat);

                // Read the whole DB block
                result = Client.DBRead(1566, startAddress, bufferSize, logBuffer);

                currentLogEntrySet.Clear();
                for (int i=0; i<numEntries; i++)
                {
                    // Parse each log entry according to start location
                    logEntry.Parse(logBuffer, i * numByteInEachEntry);

                    // LogEntryNumber is only valid between 0..65535
                    if (logEntry.LogEntryNumber > 0) 
                    {
                        currentLogEntrySet.Add(logEntry.LogEntryNumber);
                        // Log the entry if not contained in the last cycle
                        if (!lastLogEntrySet.Contains(logEntry.LogEntryNumber))
                        {
                            logger.Info(logEntry.ToString());
                        }
                    }
                }
                lastLogEntrySet = new HashSet<int>(currentLogEntrySet);

                Thread.Sleep(1000);
            }
        }

        static void Main(string[] args)
        {
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
            else
            {
                Console.WriteLine("Cannot connect");
            }
        }
    }
}