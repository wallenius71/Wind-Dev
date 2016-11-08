using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.DAL;
using TurbineDataUtility.Model;

namespace OpsDataDownloader
{
    public class ProgramConsole
    {
        string _tagfile = string.Empty;
        string _output = @" \\10.92.83.205\public\JesseRichards\Operations\Operations Raw Data\Download Application Output";
        DataTable _data = new DataTable();
        List<Tag> _tags = new List<Tag>();
        DateTime _start = default(DateTime);
        DateTime _end = default(DateTime);
        int _numDays = 0;

        public ProgramConsole(string start, string end, string tagfile)
        {
            _start = DateTime.Parse(start);
            _end = DateTime.Parse(end);
            _tagfile = tagfile;

        }
                
        public ProgramConsole(string arg1, string arg2)
        {
            DateTime dt;
            if (DateTime.TryParse(arg1, out dt))
            {
                _start = DateTime.Parse(arg1);
                _end = DateTime.Parse(arg2);
                _tagfile = @"\\10.92.83.205\public\JesseRichards\Operations\SourceTags\OLEDB\All_WAEI_Tags_NoMet.csv";
            }
            else //we have num days and tag file location
            {
                _numDays = Int16.Parse(arg1);
                _tagfile = arg2.ToString();

            }

        }
        public ProgramConsole(string NumDays)
        {
            _numDays = Int16.Parse(NumDays);
            _tagfile = @"\\10.92.83.205\public\JesseRichards\Operations\SourceTags\OLEDB\All_WAEI_Tags_NoMet.csv";

        }
        public ProgramConsole()
        {

        }
       public void getData()
        {  
           TestDownloadcs ts=null;
           if (_start == default(DateTime) || _end == default(DateTime))
           {
               if (_numDays == 0) ts = new TestDownloadcs();//default to previous month 
               else ts = new TestDownloadcs(_numDays, _tagfile);
           }
           else
           {
               ts = new TestDownloadcs(_start, _end, _tagfile);
           }
            TestDownloadcs.setup();
            ts.testOutputData(_output);
        }
    }
}

