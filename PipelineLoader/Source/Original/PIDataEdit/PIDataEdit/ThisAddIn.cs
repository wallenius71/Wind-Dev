using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;



namespace PIDataEdit
{
    public partial class ThisAddIn
    {
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            #region VSTO generated code

            this.Application = (Excel.Application)Microsoft.Office.Tools.Excel.ExcelLocale1033Proxy.Wrap(typeof(Excel.Application), this.Application);

            #endregion

            checkIfMenuBarExists();
            addMenuBar();


        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }

        #endregion

        private Office.CommandBarButton menuCommandUpdateData;
        private Office.CommandBarButton menuCommandGetPIData;
        private Office.CommandBarButton menuCommandAbout;
        
        private string menuTag = "PIEditData";
        private Excel.Worksheet ws = null;

        public enum menuTags
        {
            UpdateData,
            GetPIData,
            AboutPIDataEdit
        }

        // If the menu already exists, remove it.
        private void checkIfMenuBarExists()
        {
            try
            {
                Office.CommandBarPopup foundMenu = (Office.CommandBarPopup)
                    this.Application.CommandBars.ActiveMenuBar.FindControl(
                    Office.MsoControlType.msoControlPopup, System.Type.Missing, menuTag, true, true);

                if (foundMenu != null)
                {
                    foundMenu.Delete(true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void addMenuBar()
        {
            try
            {
                Office.CommandBarPopup cmdBarControl = null;
                Office.CommandBar menubar = (Office.CommandBar)Application.CommandBars.ActiveMenuBar;
                int controlCount = menubar.Controls.Count;
                string menuCaption = "PI D&ata Edit";

                // Add the menu.
                cmdBarControl = (Office.CommandBarPopup)menubar.Controls.Add(
                    Office.MsoControlType.msoControlPopup, missing, missing, controlCount, true);

                if (cmdBarControl != null)
                {
                    cmdBarControl.Caption = menuCaption;


                    // Add the menu command: Retrieve Data
                    menuCommandGetPIData = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandGetPIData.Caption = "&Retrieve PI Data";
                    menuCommandGetPIData.Tag = menuTags.GetPIData.ToString();
                    menuCommandGetPIData.FaceId = 23;

                    menuCommandGetPIData.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);

                    // Add the menu command: Update Data
                    menuCommandUpdateData = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandUpdateData.Caption = "&Update Data";
                    menuCommandUpdateData.Tag = menuTags.UpdateData.ToString();
                    menuCommandUpdateData.FaceId = 3;

                    menuCommandUpdateData.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);


                    // Add the menu command: About
                    menuCommandAbout = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandAbout.BeginGroup = true;
                    menuCommandAbout.Caption = "&About PI Data Edit";
                    menuCommandAbout.Tag = menuTags.AboutPIDataEdit.ToString();

                    menuCommandAbout.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);

                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        private void menuCommand_Click(Microsoft.Office.Core.CommandBarButton Ctrl, ref bool CancelDefault)
        {
            CommandBarButtonClass menu = (CommandBarButtonClass)Ctrl.Control;

            ws = (Excel.Worksheet)this.Application.ActiveSheet;
            //wb = (Excel.Workbook)this.Application.ActiveWorkbook;

            //ws.Cells[1,1] = "The menu command was clicked." + menu.Tag;

            if (menu.Tag == menuTags.UpdateData.ToString())
            {
                updatePIData();
            }
            else if (menu.Tag == menuTags.GetPIData.ToString())
            {
                getPIData();
                //something
            }
            else if (menu.Tag == menuTags.AboutPIDataEdit.ToString())
            {
                aboutClicked();
            }
        }

        private void getPIData()
        {
            GetData frmGetData = new GetData();
            frmGetData.WS = ws;

            long appCalc = (long)Application.Calculation;

            Application.Calculation = Microsoft.Office.Interop.Excel.XlCalculation.xlCalculationManual;
            Application.ScreenUpdating = false;
            Application.EnableEvents = false;
            Application.DisplayAlerts = false;

            frmGetData.ShowDialog();

            Application.Calculation = (Microsoft.Office.Interop.Excel.XlCalculation)appCalc;
            Application.ScreenUpdating = true;
            Application.EnableEvents = true;
            Application.DisplayAlerts = true;
        }


        private void updatePIData()
        {
            Excel.Range range = null;
            string piRef = string.Empty;
            string piDateTime = string.Empty;
            string piTag = string.Empty;
            string piInstrument = string.Empty;
            int piTagStart = 0;
            int piTagEnd = 0;
            string piVal = string.Empty;
            int row = 1;
            int updateCount = 0;
            int failCount = 0;

            bool dataUnchanged = false;

            long appCalc = (long)Application.Calculation;

            Application.Calculation = Microsoft.Office.Interop.Excel.XlCalculation.xlCalculationManual;
            Application.ScreenUpdating = false;
            Application.EnableEvents = false;
            Application.DisplayAlerts = false;

            DateTime startTime = DateTime.Now;

            db myDb = new db();

            ws = (Excel.Worksheet)this.Application.ActiveSheet;

            // Get initial cell
            range = (Excel.Range)ws.Cells[row, 1];
            piRef = (string)range.Formula;

            myDb.OLEConnect();

            
            myDb.SetFlushOff();

            // Is it a PI Data Grid?
            if (range.Text.ToString() == "PI Data Grid")
            {
                StringBuilder megaSQL = new StringBuilder();
                //bool blockUpdateSuccess = false;

                // get ws name, this is the PI towername
                string piTower = ws.Name;

                // check it looks like a PI tower
                if ((((Array)piTower.Split('.')).Length <= 3) || (piTower.Contains(" ")))
                {
                    MessageBox.Show(String.Format("Worksheet name '{0}' does not look like a valid PI tower name", piTower));
                    return;
                }

                
                // grid data starts at [2, 2]
                row = 2;
                int col = 2;

                // Get PI tag for this column
                //piTag = String.Format("{0}.{1}", piTower, cellToString(1, col));

                // Count the number of rows in the datetime column
                int numberOfRows = 2;
                while(cellToString(numberOfRows++, 1)!=String.Empty)
                {}

                // Back 1 from last increment
                // Back 1 as the current cell is the empty one
                // Back another 1 since we start on row 2
                numberOfRows-=3;

                // Count the number of cols 
                int numberOfCols = 2;
                while (cellToString(1, numberOfCols++) != String.Empty)
                { }

                // Back 1 from last increment
                // Back 1 as the current cell is the empty one
                // Back another 1 since we start on col 2
                numberOfCols -= 3;

                Object[,] excelArrayData = new Object[numberOfRows, 1];
                Object[,] excelArrayDateData = new Object[numberOfRows, 1];

                Excel.Range dataRange = (Excel.Range)ws.Cells[2, 1];
                dataRange = dataRange.get_Resize(numberOfRows, 1);
                dataRange.NumberFormat = "yyyy-MM-dd hh:mm";

                //excelArrayDateData = (Object[,])dataRange.Cells.Text;

                excelArrayDateData = (Object[,])dataRange.Value2;


                // Get original data - WORKING ON THIS BIT
                //List<PITagData> originalData = myDb.GetDataForSite(piTower, new DateTime(2007, 7, 1, 10, 0, 0), new DateTime(2007, 8, 1, 10, 0, 0));
                List<PITagData> originalData = myDb.GetDataForSite(piTower, DateTime.FromOADate((double)excelArrayDateData[1, 1]), DateTime.FromOADate((double)excelArrayDateData[excelArrayDateData.Length, 1]),true,false);


                //PITagData PIres = originalData.Find(new PITagData("DCL.29.WD.341.Avg", new DateTime(2007, 7, 1, 10, 0, 0), "347"));
                // End get original data

                //DateTime test = DateTime.FromOADate((double)excelArrayDateData[1,1]);
                //
                //stop
                //
                //return;

                Error frmError = new Error();
                frmError.Title = "Progress";
                frmError.ErrorMsg = "Sending data to PI";
                frmError.btnOkVisible = false;
                frmError.progressBarVisible = true;
                frmError.progressBarMaximum = numberOfCols+1;
                frmError.progressBarValue = 1;
                frmError.Show();

                frmError.Refresh();


                bool finished = false;
                while(!finished)
                {
                    // get time
                    //piDateTime = cellToString(row, 1);

                    // get column of data - include header 
                    dataRange = (Excel.Range)ws.Cells[1, col];
                    dataRange = dataRange.get_Resize(numberOfRows+1, 1);

                    excelArrayData = (Object[,])dataRange.Value2;

                    //piVal = cellToString(row++, col);

                    // Get PI tag for this column
                    piTag = String.Format("{0}.{1}", piTower, excelArrayData[1, 1].ToString());
                    piInstrument = excelArrayData[1, 1].ToString();


                    for (int i = 1; i<=numberOfRows; i++)
                    {
                        piDateTime = DateTime.FromOADate((double)excelArrayDateData[i, 1]).ToString("yyyy-MM-dd hh:mm");
                        piVal = excelArrayData[i+1, 1].ToString();
                        //piDateTime = cellToString(row, 1);

                    //old way
                        dataUnchanged = originalData.Exists(delegate(PITagData tag) { return tag.Tag == piInstrument && tag.DateTime == DateTime.FromOADate((double)excelArrayDateData[i, 1]) && tag.Value == piVal; });

                        if (!dataUnchanged)
                        {
                            updatePIAndIncrCount(piDateTime, piTag, piVal, ref updateCount, ref failCount, myDb);
                        }
                        // mega megaSQL.Append(myDb.CreateSQL(piTag, piDateTime, piVal));
                        // mega updateCount++;
                    }

                    // mega blockUpdateSuccess = myDb.updateDataInBlock(megaSQL);

                    // mega megaSQL = new StringBuilder();
                    //updatePIAndIncrCount(piDateTime, piTag, piVal, ref updateCount, ref failCount, myDb);

                    //if (cellToString(row, 1) == "")
                    //{
                        //// give the app time to breathe
                        //Application.EnableEvents = true;
                        ////Application.DoEvents();

                        // End of the time row, restart at next column?
                        col++;
                        row = 2;

                        //Application.ScreenUpdating = true;
                        //Application.EnableEvents = true;


                        frmError.progressBarValue = col-1;

                        frmError.Refresh();

                        //Application.ScreenUpdating = false;
                        //Application.EnableEvents = false;

                        //Application.DoEvents();

                        // reset PI tag name
                        //piTag = String.Format("{0}.{1}", piTower, cellToString(1, col));

                        if (cellToString(row, col) == "")
                        {
                            finished = true;
                        }
                    //}
                
                }

                frmError.Close();

            }
            else // it is a list - one of two formats are acceptable
            {
                while (piRef != "")
                {
                    // Does our piRef contain an array formula?
                    if (piRef.Contains("=PINCompDat"))
                    {
                        // This is a array of data from PI
                        // PITag+DateTime, Old Value, New Value
                        piTagStart = piRef.IndexOf("\"") + 1;
                        piTagEnd = piRef.IndexOf("\"", piTagStart + 1);
                        piTag = piRef.Substring(piTagStart, piTagEnd - piTagStart);

                        // Text of the current cell is the datetime
                        piDateTime = (string)range.Text;
                    }
                    else
                    {
                        // ...guess it is a user created pi tag
                        // PITag, DateTime, New Value
                        piTag = piRef;

                        // so datetime is in col 2
                        range = (Excel.Range)ws.Cells[row, 2];
                        piDateTime = (string)range.Text;
                    }

                    range = (Excel.Range)ws.Cells[row, 3];

                    piVal = (string)range.Text;

                    updatePIAndIncrCount(piDateTime, piTag, piVal, ref updateCount, ref failCount, myDb);

                    // get next 
                    row++;
                    range = (Excel.Range)ws.Cells[row, 1];
                    piRef = (string)range.Formula;

                }
            }

            Application.Calculation = (Microsoft.Office.Interop.Excel.XlCalculation)appCalc;
            Application.ScreenUpdating = true;
            Application.EnableEvents = true;
            Application.DisplayAlerts = true;

            myDb.SetFlushOn();

            myDb.OLEConnection.Close();

            if (row == 1)
            {
                MessageBox.Show(String.Format("No data found in cell A1 - please ensure the data is in columns A - C"));
            }
            else
            {
                TimeSpan duration = DateTime.Now - startTime;

                MessageBox.Show(String.Format("{0} record{1} updated\n\n{2} failure{3}", updateCount, updateCount != 1 ? "s" : "", failCount, failCount != 1 ? "s" : ""));

                MessageBox.Show(string.Format("Time taken: {0} minutes {1} seconds", duration.Minutes, duration.Seconds));
            }
            
        }

        private string cellToString(int row, int col)
        {
            return ((Excel.Range)ws.Cells[row, col]).Text.ToString();
        }

        private void updatePIAndIncrCount(string piDateTime, string piTag, string piVal, ref int updateCount, ref int failCount, db myDb)
        {
            if (piTag != string.Empty && piDateTime != string.Empty && piVal != string.Empty)
            {
                if (myDb.updateData(piTag, piDateTime, piVal, true))
                {
                    updateCount++;
                }
                else
                {
                    failCount++;
                }
            }
        }

            private static void aboutClicked()
        {
            About frmAbout = new About();
            frmAbout.ShowDialog();
        }

    }

}
