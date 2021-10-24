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

        ViGEmClient client;
        IDualShock4Controller controller;

        public void Start()
        {
            //Console.WriteLine("Press Ctrl+C to stop.");

            client = new ViGEmClient();

            controller = client.CreateDualShock4Controller();

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

            controller.Connect();

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
                if (b5) // left shoulder
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, true);
                else
                    controller.SetButtonState(DualShock4Button.ShoulderLeft, false);
                if (b6) // right trigger
                    controller.SetSliderValue(DualShock4Slider.RightTrigger, 255);
                else
                    controller.SetSliderValue(DualShock4Slider.RightTrigger, 0);
                if (b6) // left trigger
                    controller.SetSliderValue(DualShock4Slider.LeftTrigger, 255);
                else
                    controller.SetSliderValue(DualShock4Slider.LeftTrigger, 0);
            }
            else
                Console.WriteLine("waiting for valid data.");
        }

        public void OnApplicationExit(object s, EventArgs e)
        {
            controller.Disconnect();
            Environment.Exit(0);
        }
    }
}
