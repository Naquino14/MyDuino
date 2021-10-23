using System;
using System.Diagnostics;
using Soopah.Xna.Input;
using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Input;
namespace MyDuino
{
    class Program
    {
        static void Main(string[] args)
        {
            Process gameProcess = Process.Start(args[0]);
            Backer backer = new Backer();
            backer.Start();
            gameProcess.Exited += backer.OnApplicationExit;
        }

        
    }

    public class Backer 
    {
        public void Start()
        {
            Console.WriteLine("Press Ctrl+C to stop.");
            DirectInputGamepad dInp;
            
            while (true)
            {
                // get serial data from arduino
                // parse
                // feed
            }
        }

        public void OnApplicationExit(object s, EventArgs e) => Environment.Exit(0);
    }
}
