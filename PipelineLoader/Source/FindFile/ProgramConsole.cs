using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FindFile
{
    class ProgramConsole
    {

        static void Main(string[] args)
        {
             FindFileFromFileSystem findFile = new FindFileFromFileSystem();

            findFile.Start();
        }

    }
}
