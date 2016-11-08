using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using WindART;
using System.Windows;
using System.Windows.Forms;


namespace MultiConfigTool_UI
{
    public static class PersistSettings
    {
       public static void Save(object objgraph)
        {
            BinaryFormatter bFormat = new BinaryFormatter();

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Title = "Save Set Up File";
            saveFileDialog1.DefaultExt = ".txt";
            saveFileDialog1.Filter = "Text File|*.txt";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {

                using (Stream stream = (FileStream)saveFileDialog1.OpenFile())
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


        }
       public static object Load(string filename)
        {
            if (filename == string.Empty) return null;
            if (Path.GetExtension(filename) != ".txt")
            {
                System.Windows.MessageBox.Show("File should be a *.txt file " + Path.GetExtension(filename));
                return null;
            }
            BinaryFormatter bformat = new BinaryFormatter();

            using (Stream stream = File.OpenRead(filename))
            {
                stream.Position = 0;

                try
                {
                    return bformat.Deserialize(stream);
                }
                catch (SerializationException e)
                {
                    System.Windows.MessageBox.Show(e.Message + " " + e.Source );
                    return null;
                }
                
             }
        }

    }
}
