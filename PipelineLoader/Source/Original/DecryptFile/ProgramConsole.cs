using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DecryptFile
{
    class ProgramConsole
    {
        static void Main(string[] args)
        {
            //DecryptFileTest df = new DecryptFileTest();

            //df.ProcessEncryptedFilesTest();
            
            DecryptFile df = new DecryptFile();

            df.Decrypt();
             
        }
    }
}
