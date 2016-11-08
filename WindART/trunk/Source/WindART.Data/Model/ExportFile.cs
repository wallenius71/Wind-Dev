using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.IO;
using WindART.Properties;

namespace WindART
{
    public class ExportFile :IExportFile 
    {
        private ISessionColumnCollection _collection;
        private DataView _data;
        private List<int> _outputcols;
        public ExportFile(ISessionColumnCollection collection, DataView data,List<int> outputcols)
        {
            _collection = collection;
            _data = data;
            _outputcols = outputcols;
        }

        

        public bool OutputFile(string filename)
        {
            try
            {
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(filename, false);
                
                //column heads
                for (int i = 0; i < _outputcols.Count; i++)
                {
                    
                    sw.Write(_collection[_outputcols[i]].ColName.Replace ("#","_"));
                    Console.WriteLine(_collection[_outputcols[i]].ColName.Replace("#", "_"));
                    if (i < _outputcols.Count - 1)
                    {
                        sw.Write(",");
                    }

                }

                sw.Write(sw.NewLine);


                // Now write all the rows.
                foreach (DataRowView dr in _data)
                {
                    for (int i = 0; i < _outputcols.Count; i++)
                    {
                        if (!Convert.IsDBNull(dr[_outputcols[i]]) & dr[_outputcols[i]].ToString().Length >0 )
                        {
                            sw.Write(dr[_outputcols[i]].ToString());
                        }
                        else
                        {
                            sw.Write(Settings .Default .MissingValue );
                        }
                        if (i < _outputcols.Count - 1)
                        {
                            sw.Write(",");
                        }
                    }
                    sw.Write(sw.NewLine);
                }
                sw.Close();
                return true;
            }
            catch (Exception e)
            {
                throw e;
            }
        }

       
    }
}
