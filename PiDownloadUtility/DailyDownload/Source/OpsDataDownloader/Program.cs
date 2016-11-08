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
        static void Main(string[] args )
        {
            ProgramConsole pc;
            switch (args.Length)
            {
                case 0:
                     pc = new ProgramConsole();
                    pc.getData();
                    break;
                case 1:
                    pc = new ProgramConsole(args[0]);
                    pc.getData();
                    break;
                case 2:
                   pc = new ProgramConsole(args[0], args[1]);
                   pc.getData();
                   break;
                case 3:
                    pc = new ProgramConsole(args[0], args[1], args[2]);
                    pc.getData();
                    break;
            }
            
            
        }
    }
}
