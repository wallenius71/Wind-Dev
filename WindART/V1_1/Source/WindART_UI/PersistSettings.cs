using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using WindART;


namespace WindART_UI
{
    public static class PersistSettings
    {
       public static void Save(object objgraph, string filename)
        {
            BinaryFormatter bFormat = new BinaryFormatter();

            using (Stream stream = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                try
                {
                    stream.Position = 0;
                    bFormat.Serialize(stream, objgraph);
                }
                catch
                { }
               
            }


        }
       public static object Load(string filename)
        {
            if (filename == string.Empty) return null;
            BinaryFormatter bformat = new BinaryFormatter();

            using (Stream stream = File.OpenRead(filename))
            {
                stream.Position = 0;
                
                    return bformat.Deserialize(stream);
                
            }
        }

    }
}
