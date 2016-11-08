using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;

namespace WindowsApplication1
{
    public  class HeaderAppender
    {
        string _folder;
        List<string> _rawFilesNotFound = new List<string>();


        public List<string> PRNs { get; set; }

        public  void AddHeaders(string folder)
        {
            //find all .prn files in the selected directory 
            foreach (string s in Directory.EnumerateFiles(folder, "*.prn"))
            {
                string target=s.Replace(".prn",".asc");
                if (File.Exists(target))
                {
                    StreamReader readerPRN = new StreamReader(s);
                    StreamReader readerASC = new StreamReader(target);

                    string filename=Path.GetFileName(s);
                    StreamWriter writer = new StreamWriter(s.Replace(filename, "asc_header_" + filename),false);

                    List<string> header = new List<string>();
                    
                    //read first 4 lines of prn into list
                    for (int i = 0; i < 4; i++)
                    {
                        header.Add(readerPRN.ReadLine());
                    }
                    readerPRN.Close();

                    //write the header
                    header.ForEach(c => writer.WriteLine(c));
                    //write the rest of the asc of below the header
                    string line =string.Empty;
                    while (!readerASC.EndOfStream)
                    {
                        line = readerASC.ReadLine();
                        if(line!="")
                        writer.WriteLine(line);
                    }
                    readerASC.Close();
                    writer.Close();
                    Console.WriteLine(target);
                }
                else
                {
                    Console.WriteLine(target + "does not exist");
                }

                
            }
                MessageBox.Show("append header finished");
        }


    }
}
