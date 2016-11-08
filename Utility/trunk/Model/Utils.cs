using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.OleDb;

namespace WindowsApplication1
{
    public static class Utils
      {
        public enum aggregate
        {
            VectorAvg,
            Avg,
            Max,
            Min

        }
        public static  string GetFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Description = "Select Folder";
            dialog.ShowNewFolderButton = false;
            dialog.SelectedPath = Properties.Settings.Default.LastFolderLocation;
            //@"C:\EHarvest\Management\ObserverExportFiles\Text Files";

            if (dialog.ShowDialog() == DialogResult.OK)
            { }

            Properties.Settings.Default.LastFolderLocation = dialog.SelectedPath;

            return dialog.SelectedPath;
        }

        public static OleDbConnection Connect()
        {

            OleDbConnection Cnn = new OleDbConnection(
           @"Provider=SQLNCLI;Data Source=10.128.10.8;Initial Catalog=WindART_SMI;User Id=Windart_Jesserichards; password=metteam555");
            Cnn.Open();
            return Cnn;


        }
    }
}
