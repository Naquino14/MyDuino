using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using System;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace MyDuino
{
    class Program
    {
        static void Main(string[] args)
        {
            //Process gameProcess = Process.Start(args[0]);
            Backer backer = new Backer();
            backer.Start();
            //gameProcess.Exited += backer.OnApplicationExit;
        }
    }

    public class Backer 
    {
        SerialPort serialPort;
        int tryCom = 0;

        readonly string signature = "gda";

        bool bu = false,
            bd = false,
            bl = false,
            br = false,
            b1 = false,
            b2 = false,
            b3 = false,
            b4 = false,
            b5 = false,
            b6 = false,
            b7= false;

        public void Start()
        {
            //Console.WriteLine("Press Ctrl+C to stop.");

            var client = new ViGEmClient();

            var controller = client.CreateDualShock4Controller();

            try
            {
                serialPort = new SerialPort($"COM{tryCom}", 9600, Parity.None, 8, StopBits.One);

                serialPort.Open();
            } catch (IOException ex)
            {
                if (tryCom > 6)
                    throw new Exception();
                Console.WriteLine($"Port at COM{tryCom} not found.");
                tryCom++;
                serialPort.Close();
                serialPort.Dispose();
                Thread.Sleep(500);
                Start();
            }

            serialPort.DataReceived += new SerialDataReceivedEventHandler(OnPayloadRecieved);

            Console.WriteLine("Press any key to stop");
            Console.ReadLine();
            OnApplicationExit(null, null);
        }

        void OnPayloadRecieved(object o, SerialDataReceivedEventArgs e)
        {
            string payload = serialPort.ReadLine();
            //Console.WriteLine(payload);
            if (payload.Contains(signature))
            {
                // parse and feed
                bu = payload[0] == 1;
                bd = payload[1] == 1;
                bl = payload[2] == 1;
                br = payload[3] == 1;
                b1 = payload[4] == 1;
                b2 = payload[5] == 1;
                b3 = payload[6] == 1;
                b4 = payload[7] == 1;
                b5 = payload[8] == 1;
                b6 = payload[9] == 1;
                b7 = payload[10] == 1;
            }
            else
                Console.WriteLine("waiting for valid data.");
        }

        public void OnApplicationExit(object s, EventArgs e) => Environment.Exit(0);
    }
}
