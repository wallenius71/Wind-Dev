using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;

namespace OpsDataDownloader
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            ProgramConsole pc;
            switch (args.Length)
            {
                case 0:
                    pc = new ProgramConsole();
                    pc.getData();
                    break;

                case 1:
                    int days = Int16.Parse(args[0]);
                    pc = new ProgramConsole(days);
                    pc.getData();
                    break;

                case 2:
                    pc = new ProgramConsole();
                    pc.getData(args[0], args[1]);
                    break;
            }


        }
    }
}
