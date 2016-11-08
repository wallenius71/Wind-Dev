using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GemBox.Spreadsheet;
using System.IO;

namespace WindART
{
    public abstract class AbstractExcelWorkSheet
    {
        public delegate void  writeNotificationHandler(string note);
        public event writeNotificationHandler WriteNotification;
        public AbstractAlpha GridCollection { get; set; }
        protected void OnWriteNotification(string note)
        {
            if (WriteNotification!=null)
            WriteNotification(note);
        }
        public abstract ExcelWorksheet  BuildWorkSheet();
       
        protected List<List<SummaryGridColumn>> uppercountgrid;
        protected List<List<SummaryGridColumn>> lowercountgrid;
        protected List<List<SummaryGridColumn>> uppergrid;
        protected List<List<SummaryGridColumn>> lowergrid;
        protected List<List<SummaryGridColumn>> alpha;
        protected List<List<List<SummaryGridColumn>>> grids = new List<List<List<SummaryGridColumn>>>();

        public void PrintSingleGrids(string outputfilename)
        {
             string targetLocation=string.Empty;
             int i = 1;
            if (outputfilename.Length >0)
            {
                foreach (List<List<SummaryGridColumn>> grid in grids)
                {
                    targetLocation = Path.GetDirectoryName(outputfilename) + @"\" +  Path.GetFileNameWithoutExtension (outputfilename)  + "_" + GridCollection.Xaxis.AxisType + "x" + GridCollection.Yaxis.AxisType + "_Grid_" + i + ".csv";
                    i++;

                    Utils.OutputFile(grid, targetLocation);
                }
            }
        }
    }
}
