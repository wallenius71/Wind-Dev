using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Core;

//using System.Collections;
//using System.IO;

namespace PIDataEdit
{
    public partial class GetData : Form
    {
        public GetData()
        {
            InitializeComponent();
        }

        private Excel.Worksheet _ws;

        public Excel.Worksheet WS
        {
            get { return _ws; }
            set { _ws = value; }
        }
	

        public Excel.Worksheet ws;

        private void GetData_Load(object sender, EventArgs e)
        {
            db myDb = new db();

            Cursor.Current = Cursors.WaitCursor;
            
            object[] sites = myDb.getSites();

            Cursor.Current = Cursors.Default;
            
            if (sites != null)
            {
                lbxSites.Items.AddRange(sites);
            }

            // Test mode
            dtpFrom.Text = "1 July 2006 10:00";
            dtpTo.Text = "31 December 2006 11:00";
            //if (lbxSites.Items.Count >= 8)
            //{
            //    lbxSites.SelectedIndex = 8;
            //}
            lbxSites.SelectedItem = "BPMT.US.OR.00007";
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            db myDb =new db();
            string site = lbxSites.SelectedItem.ToString();
            DateTime dtFrom = dtpFrom.Value;
            DateTime dtTo = dtpTo.Value;

            this.Hide();

            Cursor.Current = Cursors.WaitCursor;

            Error frmError = new Error();
            frmError.Title = "Progress";
            frmError.ErrorMsg = "Downloading data from PI Sever";
            frmError.btnOkVisible = false;
            frmError.Show();


            Application.DoEvents();

            // ADODB Test Idea
            //ADODB.Connection adoCn = new ADODB.Connection();

            //adoCn.Provider = "PIOLEDB";
            //myDb.OLEConnect();

            //DataSet objDataSet = new DataSet();
            
            //OleDbDataAdapter objAdapter = new OleDbDataAdapter(@"SELECT tag, [time], [value] FROM piarchive..picomp2 where tag = 'BPMT.US.OR.00007.DCL.29.WD.341.Avg' and time = '2006-07-01 10:00'", myDb.OLEConnection);

            //objAdapter.Fill(objDataSet, "PIData");

            //DataView objDataView = new DataView(objDataSet.Tables["PIData"]);
            // End Test Idea


            List<PITagData> data = myDb.GetDataForSite(site, dtFrom, dtTo, false, true);

            DateTime startTime = DateTime.Now;

            //int row;
            int col = 1;
            int i = 1;
            string lastTag = String.Empty;

            List<String> cellRange = new List<String>();
            List<DateTime> dateTimeRange = new List<DateTime>();
            

            Excel.Range dataRange = (Excel.Range)_ws.Cells[1, 1];

            frmError.Close();

            frmError = new Error();
            frmError.Title = "Progress";
            frmError.ErrorMsg = "Populating spreadsheet";
            frmError.btnOkVisible = false;
            frmError.progressBarVisible = true;
            frmError.progressBarMaximum = data.Count;
            frmError.Show();

            Application.DoEvents(); 

            foreach(PITagData d in data)
            {
                // update progress bar every 100 items
                // this is to prevent too much slowdown
                i++;
                if (i % 100 == 0)
                {
                    frmError.progressBarValue = i;
                    Application.DoEvents();
                }

                if (d.Tag != lastTag)
                {
                    // If this is the first tag, no need to flush cos
                    if (lastTag != String.Empty)
                    {
                        // We are on a new tag - flush the buffer to the spreadsheet

                        Object[,] excelArrayData = new Object[cellRange.Count, 1];

                        if (col == 2)
                        {
                            // Better put the date/times in
                            int dateLoop = 0;
                            foreach (DateTime item in dateTimeRange)
                            {
                                excelArrayData[dateLoop++, 0] = item;
                            }
                            dataRange = (Excel.Range)_ws.Cells[2, 1]; // First position of datetime data
                            dataRange = dataRange.get_Resize(cellRange.Count, 1);
                            dataRange.Value2 = excelArrayData;
                            dataRange.NumberFormat = "yyyy-MM-dd hh:mm";

                        }

                        int loop = 0;
                        foreach (Object item in cellRange)
                        {
                            excelArrayData[loop++, 0] = item;
                        }

                        dataRange = (Excel.Range)_ws.Cells[1, col];
                        dataRange = dataRange.get_Resize(cellRange.Count, 1);
                        dataRange.Value2 = excelArrayData;

                        // Make sure it doesn't try to auto format the cells
                        dataRange.NumberFormat = "@";

                        cellRange.Clear();
                        excelArrayData = null;  // Hopefully save some memory by forcing garbage collection
                    }

                    // reset
                    //row = 1;
                    col++;

                    cellRange.Add(d.Tag);
                }

                // Only store time in when we are in col 2, i.e. the first column of data
                if (col == 2)
                {
                    dateTimeRange.Add(d.DateTime);
                }

                cellRange.Add(d.Value);
                
                lastTag = d.Tag;
            }

            // Put marker in cell [1, 1] to indicate it is a grid of data
            _ws.Cells[1, 1] = "PI Data Grid";

            // Name the worksheet, so we can reload the data
            try
            {
                _ws.Name = site;
            }
            catch
            {
                MessageBox.Show(String.Format("Could not name the spreadsheet {0}.  This will need to be done manually if the data is to be uploaded", site));
            }

            // set column A to a set width so we can see the datatime stamps properly, rather than #####
            Excel.Range range = (Excel.Range)_ws.Cells[1, 1];
            range.ColumnWidth = 20;

            Cursor.Current = Cursors.Default;

            frmError.Close();

            TimeSpan duration = DateTime.Now - startTime;

            //MessageBox.Show(string.Format("Populate spreadsheet: {0} minutes {1} seconds", duration.Minutes, duration.Seconds));



            this.Close();
        }

    }
}