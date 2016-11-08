using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VerifyInputFile
{
    class ProgramConsole
    {
        static void Main(string[] args)
        {
            //TestVerifyInputFile vif = new TestVerifyInputFile();

            VerifyInputFile vif = new VerifyInputFile();
            
            vif.Start();
            
        }
    }
}
