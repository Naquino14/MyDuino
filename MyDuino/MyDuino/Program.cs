using Nefarius.ViGEm.Client;
using Nefarius.ViGEm.Client.Targets;
using Nefarius.ViGEm.Client.Targets.DualShock4;
using System;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Threading;

namespace MyDuino
{
    class Program
    {
        static void Main(string[] args)
        {
            ArduinoReader arduinoReader = new ArduinoReader();
            if (args.Length != 0)
            {
                bool useDRPEverything = args[0] == "-drp";
                if (args[0] == "-drp")
                {
                    ProcessStartInfo drpPSI = new ProcessStartInfo // -drp, path to tool, argument 0, argument 1
                    {
                        FileName = args[1],
                        Arguments = $"\"{args[2]}\" \"{args[3]}\""
                    };
                    Process drp = Process.Start(drpPSI);
                    drp.EnableRaisingEvents = true;
                    drp.Exited += arduinoReader.OnApplicationExit;
                }
                else
                {
                    Process gameProcess = Process.Start(args[0]);
                    gameProcess.EnableRaisingEvents = true;
                    gameProcess.Exited += arduinoReader.OnApplicationExit;
                }
            }
            arduinoReader.Start();
        }
    }

    public class ArduinoReader 
    {
        public ArduinoReader(Process toolProcess = null) => this.toolProcess = toolProcess;

        Process toolProcess;
        SerialPort serialPort;
        int tryCom = 0;
        readonly int maxTryComs = 6;

        readonly string signature = "g";

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

        ViGEmClient client;
        IDualShock4Controller controller;
        Thread readThread;

        readonly int baudRate = 74880;
        int payloadSize;

        public void Start()
        {
            payloadSize = signature.Length * 8 + 88;
            client = new ViGEmClient();
            controller = client.CreateDualShock4Controller();
            try
            {
                serialPort = new SerialPort($"COM{tryCom}", baudRate, Parity.None, 8, StopBits.One);
                serialPort.Open();
            }
            catch (IOException ex)
            {
                try 
                {
                    if (tryCom > maxTryComs)
                        throw new Exception("No boards found or connected to this device.");
                } catch (Exception te)
                { 
                    Console.WriteLine($"{te.ToString()}\nPress any key to close."); 
                    Console.ReadLine();
                    Environment.Exit(-1);
                }
                Console.WriteLine($"Port at COM{tryCom} not found.");
                tryCom++;
                serialPort.Close();
                serialPort.Dispose();
                Thread.Sleep(500);
                Start();
            }
            controller.Connect();
            serialPort.ReceivedBytesThreshold = payloadSize;
            Console.WriteLine("Board Found!\nPress enter to stop");
            readThread = new Thread(OnReadData);
            readThread.Start();
            Console.ReadLine();
            OnApplicationExit(null, null);
        }

        void OnReadData()
        {
            while (true)
            {
                string payload = serialPort.ReadLine();
                //Console.WriteLine(payload);
                if (payload.Contains(signature))
                {
                    #region parsing

                    bu = payload[0 + signature.Length] == '1';
                    bd = payload[1 + signature.Length] == '1';
                    bl = payload[2 + signature.Length] == '1';
                    br = payload[3 + signature.Length] == '1';
                    b1 = payload[4 + signature.Length] == '1';
                    b2 = payload[5 + signature.Length] == '1';
                    b3 = payload[6 + signature.Length] == '1';
                    b4 = payload[7 + signature.Length] == '1';
                    b5 = payload[8 + signature.Length] == '1';
                    b6 = payload[9 + signature.Length] == '1';
                    b7 = payload[10 + signature.Length] == '1';

                    #endregion

                    #region D Pad

                    if (bu && bl)
                        controller.SetDPadDirection(DualShock4DPadDirection.Northwest);
                    else if (bu && br)
                        controller.SetDPadDirection(DualShock4DPadDirection.Northeast);
                    else if (bd && bl)
                        controller.SetDPadDirection(DualShock4DPadDirection.Southwest);
                    else if (bd && br)
                        controller.SetDPadDirection(DualShock4DPadDirection.Southeast);
                    else if (bu)
                        controller.SetDPadDirection(DualShock4DPadDirection.North);
                    else if (bd)
                        controller.SetDPadDirection(DualShock4DPadDirection.South);
                    else if (br)
                        controller.SetDPadDirection(DualShock4DPadDirection.East);
                    else if (bl)
                        controller.SetDPadDirection(DualShock4DPadDirection.West);
                    else
                        controller.SetDPadDirection(DualShock4DPadDirection.None);

                    #endregion

                    #region Buttons

                    if (b1) // square
                        controller.SetButtonState(DualShock4Button.Square, true);
                    else
                        controller.SetButtonState(DualShock4Button.Square, false);
                    if (b2) // cross
                        controller.SetButtonState(DualShock4Button.Cross, true);
                    else
                        controller.SetButtonState(DualShock4Button.Cross, false);
                    if (b3) // triangle
                        controller.SetButtonState(DualShock4Button.Triangle, true);
                    else
                        controller.SetButtonState(DualShock4Button.Triangle, false);
                    if (b4) // circle
                        controller.SetButtonState(DualShock4Button.Circle, true);
                    else
                        controller.SetButtonState(DualShock4Button.Circle, false);
                    if (b5) // right shoulder
                        controller.SetButtonState(DualShock4Button.ShoulderRight, true);
                    else
                        controller.SetButtonState(DualShock4Button.ShoulderRight, false);
                    if (b6) // left shoulder
                        controller.SetButtonState(DualShock4Button.ShoulderLeft, true);
                    else
                        controller.SetButtonState(DualShock4Button.ShoulderLeft, false);

                    #endregion
                }
                else
                    Console.WriteLine("waiting for valid data.");
            }
        }

        public void OnApplicationExit(object s, EventArgs u)
        {
            readThread.Abort();
            controller.Disconnect();
            if (toolProcess != null)
                toolProcess.Kill();
            Environment.Exit(0);
        }
    }
}
