using System;
using System.Windows.Forms;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Core;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace ExcelApplication
{
    public partial class ThisAddIn
    {
        public enum menuTags
        {
            ImportData,
            LoadDetails,
            SaveDetails,
            SaveAsDetails,
            InsertColumnHeader,
            GenerateUFL,
            About
        }

        const int xlShiftDown = -4121;


        private Excel.Workbook wb = null;
        private Excel.Worksheet ws = null;

        private bool LoadDetailsDone = false;

        private void ThisAddIn_Startup_Original(object sender, System.EventArgs e)
        {
            #region VSTO generated code

            //this.Application = (Excel.Application)Microsoft.Office.Tools.Excel.ExcelLocale1033Proxy.Wrap(typeof(Excel.Application), this.Application);

            #endregion

            //MessageBox.Show("The document has been deployed successfully.");
        }

        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
        }

        #region MyCode
        // Declare the menu variable at the class level.
        private Office.CommandBarButton menuCommandImportData;
        private Office.CommandBarButton menuCommandLoadDetails;
        private Office.CommandBarButton menuCommandSaveDetails;
        private Office.CommandBarButton menuCommandSaveAsDetails;
        private Office.CommandBarButton menuCommandInsertColumnHeader;
        private Office.CommandBarButton menuCommandGenerateUFL;
        private Office.CommandBarButton menuCommandAbout;


        private string menuTag = "DefineUFL";


        // Call AddMenu from the Startup event of ThisWorkbook.
        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            checkIfMenuBarExists();
            addMenuBar();
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


        // Create the menu, if it does not exist.
        private void addMenuBar()
        {
            try
            {
                Office.CommandBarPopup cmdBarControl = null;
                Office.CommandBar menubar = (Office.CommandBar)Application.CommandBars.ActiveMenuBar;
                int controlCount = menubar.Controls.Count;
                string menuCaption = "Define &UFL";

                // Add the menu.
                cmdBarControl = (Office.CommandBarPopup)menubar.Controls.Add(
                    Office.MsoControlType.msoControlPopup, missing, missing, controlCount, true);

                if (cmdBarControl != null)
                {
                    cmdBarControl.Caption = menuCaption;

                    // Add the menu command: Import Data
                    menuCommandImportData = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandImportData.Caption = "&Add UFL Areas";
                    menuCommandImportData.Tag = menuTags.ImportData.ToString();
                    menuCommandImportData.FaceId = 4388;

                    menuCommandImportData.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);


                    // Add the menu command: Insert Column Header
                    menuCommandInsertColumnHeader = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandInsertColumnHeader.BeginGroup = true;
                    menuCommandInsertColumnHeader.Caption = "&Insert PI Tag Header";
                    menuCommandInsertColumnHeader.Tag = menuTags.InsertColumnHeader.ToString();

                    menuCommandInsertColumnHeader.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);

                    // Add the menu command: Load Details
                    menuCommandLoadDetails = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandLoadDetails.BeginGroup = true;

                    menuCommandLoadDetails.Caption = "&Load Details From Database";
                    menuCommandLoadDetails.Tag = menuTags.LoadDetails.ToString();
                    menuCommandLoadDetails.FaceId = 23;


                    menuCommandLoadDetails.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);


                    // Add the menu command: Save Details
                    menuCommandSaveDetails = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandSaveDetails.Caption = "&Save Details To Database";
                    menuCommandSaveDetails.Tag = menuTags.SaveDetails.ToString();
                    menuCommandSaveDetails.FaceId = 3;

                    menuCommandSaveDetails.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);

                    // Add the menu command: Save As New Format
                    menuCommandSaveAsDetails = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandSaveAsDetails.Caption = "Save As New &Format";
                    menuCommandSaveAsDetails.Tag = menuTags.SaveAsDetails.ToString();
                    menuCommandSaveAsDetails.FaceId = 0;

                    menuCommandSaveAsDetails.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);

                    // Add the menu command: GenerateUFL
                    menuCommandGenerateUFL = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandGenerateUFL.BeginGroup = true;
                    menuCommandGenerateUFL.Caption = "&Generate UFL";
                    menuCommandGenerateUFL.Tag = menuTags.GenerateUFL.ToString();
                    menuCommandGenerateUFL.FaceId = 65;

                    menuCommandGenerateUFL.Click += new Microsoft.Office.Core._CommandBarButtonEvents_ClickEventHandler(
                        menuCommand_Click);

                    // Add the menu command: About
                    menuCommandAbout = (Office.CommandBarButton)cmdBarControl.Controls.Add(
                        Office.MsoControlType.msoControlButton, missing, missing, missing, true);

                    menuCommandAbout.BeginGroup = true;
                    menuCommandAbout.Caption = "&About Define UFL";
                    menuCommandAbout.Tag = menuTags.About.ToString();

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
            wb = (Excel.Workbook)this.Application.ActiveWorkbook;

            //ws.Cells[1,1] = "The menu command was clicked." + menu.Tag;

            if (menu.Tag == menuTags.About.ToString())
            {
                aboutClicked();
            }
            else if (menu.Tag == menuTags.ImportData.ToString())
            {
                importDataClicked();
            }
            else if (menu.Tag == menuTags.InsertColumnHeader.ToString())
            {
                insertColumnHeadingClicked();
            }
            else if (menu.Tag == menuTags.GenerateUFL.ToString())
            {
                generateUFLClicked();
            }
            else if (menu.Tag == menuTags.SaveDetails.ToString())
            {
                saveDetails();
            }
            else if (menu.Tag == menuTags.SaveAsDetails.ToString())
            {
                resetFileFormatId();
                saveDetails();
            }
            else if (menu.Tag == menuTags.LoadDetails.ToString())
            {
                loadDetails();
            }
        }

        private void resetFileFormatId()
        {
            // Set format to -1
            // The code will then generate a new index
            ws.Cells[8, 3] = -1;
        }

        private void loadDetails()
        {

            Error frmError = new Error();
            UFLDetails ufl = new UFLDetails();
            UFLDetails oldUfl = new UFLDetails();
            string errMsg = string.Empty;
            db myDb = new db();
            Load frmLoad = new Load();
            Excel.Range range = null;

            if (LoadDetailsDone)
            {

                frmLoad.ShowDialog();

                ufl.UFLDetailId = frmLoad.UFLDetailId;

                if (ufl.UFLDetailId <= 0)
                {
                    //frmError.ErrorMsg = "No UFL details could be found.\n\nHave you created a UFL Area and selected a unique id?";
                    //frmError.ShowDialog();
                    return;
                }

                oldUfl.GetPiTags(ws);

                // Clear PI Tags row
                if (oldUfl.PiTagRow > 0)
                {
                    range = (Excel.Range)ws.get_Range("A" + oldUfl.PiTagRow, "A" + oldUfl.PiTagRow);
                    range.EntireRow.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                }

                // Clear contents of col 1
                range = (Excel.Range)ws.get_Range("A1", "A1");

                range.EntireColumn.ClearContents();
                //.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                //insertCol(1);

                //myDb.loadUFL(ufl);
                ufl.LoadUFL();

                ufl.SetHeaderValues(ref ws);

                setHeaderMarkerRow(ufl);

                setHeaderAdditionalDetailsRows(ufl);

                createPiTagsNamedRangeForSite(ufl.PiTowerName);

                insertPiTagsIntoHeaderRow(ufl.PiTagRow);



                ufl.SetPiTags(ref ws);

                frmError.ErrorMsg = String.Format("Successfully loaded details for {0} ({1}).", ufl.PiTowerName, ufl.UFLDetailId);
                frmError.Title = "Loaded";
                frmError.ShowDialog();
            }
            else
            {
                // Please select 'Insert PI Tag Header' first
                frmError.ErrorMsg = "Please use the menu option 'Add UFL Areas' first.";
                frmError.ShowDialog();
            }
        }


        private void saveDetails()
        {

            Error frmError = new Error();
            UFLDetails ufl = new UFLDetails();
            string errMsg = string.Empty;
            db myDb = new db();

            ufl.GetHeaderValues(ws);

            // should we verify on save?



            ufl.HeaderMarkerRow = getHeaderMarkerRow();

            ufl.HeaderAdditionalDetailRows = getHeaderAdditionalDetailsRows();

            ufl.GetPiTags(ws);



            // Save, save, save
            ufl.SaveUFL();

            // Update Row Id
            ws.Cells[8, 3] = ufl.UFLDetailId;

            frmError.ErrorMsg = "Successfully saved details.";
            frmError.Title = "Saved";
            frmError.ShowDialog();

        }



        private List<string> getHeaderAdditionalDetailsRows()
        {
            List<string> rows = new List<string>();
            int row = 1;
            int col = 1;
            Excel.Range cell = null;
            string firstFoundAddress = string.Empty;

            while (true)
            {
                try
                {
                    if (cell == null)
                    {
                        cell = (Excel.Range)ws.Cells.Find("Additional Detail", ws.Cells[row, col], Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart, missing, Excel.XlSearchDirection.xlNext, false, missing, missing).Cells;
                        firstFoundAddress = cell.get_Address(true, true, Excel.XlReferenceStyle.xlA1, missing, missing);
                    }
                    else
                    {
                        cell = ws.Cells.FindNext(cell);
                        if (cell.get_Address(true, true, Excel.XlReferenceStyle.xlA1, missing, missing) == firstFoundAddress)
                        {
                            // we have looped
                            break;
                        }
                    }
                }
                catch
                {
                    // if not found, throws an exception!
                    break;
                }

                row = cell.Row;
                col = cell.Column;
                if (col == 1)
                {
                    rows.Add(row.ToString());
                }
            }
            return rows;
        }

        private void setHeaderAdditionalDetailsRows(UFLDetails ufl)
        {
            foreach (string row in ufl.HeaderAdditionalDetailRows)
            {
                if (row != "")
                {
                    ws.Cells[row, 1] = "Additional Detail";
                }
            }
        }


        private int getHeaderMarkerRow()
        {
            Utils u = new Utils();
            return u.FindStringInSpreadsheet(ws, "Header Match");
        }

        private void setHeaderMarkerRow(UFLDetails ufl)
        {
            ws.Cells[ufl.HeaderMarkerRow, 1] = "Header Match";
        }


        private bool verifyHeaderValues(UFLDetails ufl, ref string errMsg)
        {
            // Saves having to check for this dummy contents as well as string empty
            if (ufl.DateTimeFormat == "eg MM/dd/yyyy hh:mm:ss")
            {
                ufl.DateTimeFormat = String.Empty;
            }

            // valid header info

            if (ufl.Delimiter == String.Empty)
            {
                errMsg = "Please select a delimiter.";
                return false;
            }


            if ((ufl.DateTimeFormat != String.Empty) && (ufl.DateFormat != String.Empty) && (ufl.TimeFormat != String.Empty))
            {
                errMsg = "Please select either a DateTime Format or a Date and a Time Format.";
                return false;
            }

            if (ufl.PiTowerName == String.Empty)
            {
                errMsg = "Please select a Tower Name.";
                return false;
            }

            return true;
        }

        //private void getHeaderValues(UFLDetails ufl)
        //{
        //    ufl.Delimitor = (String)((Excel.Range)ws.Cells[2, 3]).Text;
        //    ufl.DateTimeFormat = (String)((Excel.Range)ws.Cells[3, 3]).Text;
        //    ufl.DateFormat = (String)((Excel.Range)ws.Cells[4, 3]).Text;
        //    ufl.TimeFormat = (String)((Excel.Range)ws.Cells[5, 3]).Text;
        //    ufl.PiTowerName = (String)((Excel.Range)ws.Cells[7, 3]).Text;
        //    if(((Excel.Range)ws.Cells[8, 3]).Text.ToString()!="")
        //    {
        //     ufl.UFLDetailId = int.Parse(((Excel.Range)ws.Cells[8, 3]).Text.ToString());
        //    }

        //}

        //private void setHeaderValues(UFLDetails ufl)
        //{
        //    ws.Cells[2, 3] = ufl.Delimitor;
        //    ws.Cells[3, 3] = ufl.DateTimeFormat;
        //    ws.Cells[4, 3] = ufl.DateFormat;
        //    ws.Cells[5, 3] = ufl.TimeFormat;
        //    ws.Cells[7, 3] = ufl.PiTowerName;
        //    ws.Cells[8, 3] = ufl.UFLDetailId;
        //}

        private void generateUFLClicked()
        {
            UFLDetails ufl = new UFLDetails();
            List<String> iniTemplate = new List<string>();
            Utils u = new Utils();
            Error frmError = new Error();
            string outFilename = string.Empty;
            db myDb = new db();

            // get all the UFL details

            ufl.GetHeaderValues(ws);

            if (ufl.UFLDetailId <= 0)
            {

                frmError.ErrorMsg = "Please use 'Save Details to Database' before generating the UFL.";
                frmError.ShowDialog();
                return;

            }

            if (ufl.DateTimeFormat.IndexOf("mm") != ufl.DateTimeFormat.LastIndexOf("mm"))
            {
                frmError.ErrorMsg = String.Format("Warning: It looks like the date format contains 'mm' twice.\n\nMonth should use 'MMM', minutes should us 'mm'.");
                frmError.Title = "Warning";
                frmError.ShowDialog();
            }

            ufl.HeaderMarkerRow = getHeaderMarkerRow();

            ufl.HeaderAdditionalDetailRows = getHeaderAdditionalDetailsRows();

            ufl.GetPiTags(ws);

            ufl.GetHeaderRowContents(ws);

            ufl.GetHeaderAdditionalDetailsRowsContents(ws);

            ufl.UpdateFileFormatDetails();

            DateTime dateval = default(DateTime);
            int row = ufl.HeaderMarkerRow + 2;
            if (!ufl.HeaderMarkerRowContents.Contains("JulianDay"))
            {

                Excel.Range rng = ws.Cells[row, 2] as Excel.Range;
                string datevalstring = rng.Value.ToString();

                dateval = DateTime.Parse(datevalstring);

            }
            else
            {
                //assemble date from julian date 
                Excel.Range rng = ws.Cells[row, 4] as Excel.Range;
                string year = rng.Value.ToString();

                rng = ws.Cells[row, 5] as Excel.Range;
                string doy = rng.Value.ToString();

                rng = ws.Cells[row, 6] as Excel.Range;
                string hourmin = rng.Value.ToString();

                dateval = u.JulianToDateTime(year, doy, hourmin);

            }

            string sensorError = ufl.CheckMappedSensors(ufl.UFLDetailId, dateval);
            if (sensorError.Length > 0)
            {
                frmError.ErrorMsg = sensorError;
                frmError.Title = "Save Sensor Mappings";
                frmError.Height = 500;
                frmError.Width = 700;
                frmError.ShowDialog();

                return;
            }
            frmError.ErrorMsg = "Successfully saved sensor mappings ";
            frmError.Title = "Save Sensor Mappings";
            frmError.Width = 500;
            frmError.ShowDialog();

        }


        private void insertColumnHeadingClicked()
        {
            string site = null;

            if (LoadDetailsDone)
            {
                site = siteSelected();
                if (site != string.Empty)
                {
                    createPiTagsNamedRangeForSite(site);
                    insertPiTagsIntoHeaderRow(-1);
                }
                else
                {
                    // No site selected
                    Error frmError = new Error();
                    frmError.ErrorMsg = "Please select a site from the PI Tower Name list.";
                    frmError.ShowDialog();
                }
            }
            else
            {
                // Please select 'Insert PI Tag Header' first
                Error frmError = new Error();
                frmError.ErrorMsg = "Please use the menu option 'Add UFL Areas' first.";
                frmError.ShowDialog();
            }
        }

        private void importDataClicked()
        {
            // To enable if I can figure out how to force it into the CSV import wizard
            //showFileOpen();
            //showFileImportText();

            db myDb = new db();
            if (!myDb.verifyCanConnectToDb())
            {
                return;
            }


            if (alreadyInsertedUFLHeader())
            {
                insertStuff();
                addWorksheet();
                insertDataEntryHeader();

                LoadDetailsDone = true;
            }
        }

        private static void aboutClicked()
        {
            About frmAbout = new About();
            frmAbout.ShowDialog();
        }

        private void insertPiTagsIntoHeaderRow(int row)
        {
            Excel.Range range = null;

            if (row == -1)
            {
                row = this.Application.ActiveCell.Cells.Row + 1;
            }

            insertRow(row);

            ws.Cells[row, 1] = "PITags";

            for (int i = 2; i <= 101; i++)
            {
                range = (Excel.Range)ws.Cells[row, i];
                range.Validation.Delete();
                range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
                      Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=PITags", missing);

            }
        }

        private void createPiTagsNamedRangeForSite(string site)
        {
            db myDb = new db();
            List<String> piTags = new List<string>();

            myDb.getSitePITags(site, piTags);

            // Insert constants
            piTags.Insert(0, "Height");
            piTags.Insert(0, "JulianDay");
            piTags.Insert(0, "Year");
            piTags.Insert(0, "Time");
            piTags.Insert(0, "Date");
            piTags.Insert(0, "DateTime");
            piTags.Insert(0, "N/A");

            // Insert our named range in Tags worksheet
            Excel.Worksheet tagWs = (Excel.Worksheet)wb.Sheets["Tags"];
            tagWs.Select(missing);
            insertNamedRange(tagWs, piTags.ToArray(), "PITags", 1);

            // Switch back to original
            ws.Select(missing);
        }

        private string siteSelected()
        {
            Excel.Range range = (Excel.Range)ws.Cells[7, 3];
            string val = (string)range.Text;
            return (val);
        }

        private void showFileImportText()
        {
            Microsoft.Office.Core.FileDialog fd =
                this.Application.get_FileDialog(Microsoft.Office.Core.MsoFileDialogType.msoFileDialogOpen);
        }

        private void addWorksheet()
        {
            // remeber where we are
            string currentSheet = ws.Name;

            // Remove any sheets called 'Tags'
            foreach (Excel.Worksheet lastTagWs in wb.Sheets)
            {
                if (lastTagWs.Name == "Tags")
                {
                    lastTagWs.Delete();
                }
            }
            // create new sheet callde 'Tags'
            wb.Worksheets.Add(missing, missing, missing, missing);

            Excel.Worksheet tagWs = (Excel.Worksheet)wb.Sheets[1];

            tagWs.Select(missing);
            tagWs.Name = "Tags";


            // Insert lookups
            String[] ddlDelimit = new string[] { "TAB", "SPACE", ",", ";" };
            String[] ddlHeaderDetail = new string[] { "Header Match", "Additional Detail" }; //,"Data Header"};
            String[] ddlYesNo = new string[] { "Yes", "No" };
            String[] ddlDataType = new string[] { "DAT", "DPD" };

            List<string> ddlMetTowers = new List<string>();
            List<string> locations = new List<string>();

            getMetTowers(ddlMetTowers);
            getLocations(locations);

            insertNamedRange(tagWs, ddlDelimit, "Delimiters", 3);
            insertNamedRange(tagWs, ddlYesNo, "YesNo", 4);
            insertNamedRange(tagWs, ddlHeaderDetail, "HeaderDetails", 5);
            //insertNamedRange(tagWs, locations.ToArray(), "Locations", 6);
            insertNamedRange(tagWs, ddlMetTowers.ToArray(), "MetTowers", 7);
            insertNamedRange(tagWs, ddlDataType, "DataType", 8);

            // swap back to original sheet
            ((Excel.Worksheet)this.Application.Sheets[currentSheet]).Select(missing);

        }

        private void getMetTowers(List<string> metTowers)
        {
            db myDb = new db();

            myDb.getSites(metTowers);
        }

        private void getLocations(List<string> locations)
        {
            db myDb = new db();

            myDb.getLocations(locations);
        }


        private void insertNamedRange(Excel.Worksheet tagWs, String[] ddlPITags, string name, int col)
        {
            int i = 1;
            foreach (string tagName in ddlPITags)
            {
                tagWs.Cells[i++, col] = tagName;
            }

            // wind the counter back one, ready for defining the named range
            i = i - 1;

            wb.Names.Add(name, missing, missing, missing, missing, missing, missing, missing, missing, "=Tags!R1C" + col + ":R" + i + "C" + col, missing);
        }

        private void insertStuff()
        {
            for (int i = 1; i <= UFLDetails.UFL_DETAIL_HEADER_SIZE; i++)
            {
                insertRow(1);
            }
            insertCol(1);
        }

        private bool alreadyInsertedUFLHeader()
        {
            Excel.Range range = (Excel.Range)ws.Cells[1, 1];
            return ((int)range.Interior.ColorIndex != 6);
        }


        private void insertDataEntryHeader()
        {
            Excel.Range range = null;

            range = (Excel.Range)ws.Cells[1, 1];
            range.ColumnWidth = 24;


            // Header text
            ws.Cells[2, 2] = "Delimiter";
            ws.Cells[3, 2] = "DateTime Format";
            //ws.Cells[3,3] = "eg MM/dd/yyyy hh:mm:ss";
            ws.Cells[3, 3] = "dd-MMM-yyyy hh:mm";



            ws.Cells[4, 2] = "Date Format";
            ws.Cells[5, 2] = "Time Format";
            ws.Cells[7, 2] = "PI Tower Name";
            ws.Cells[8, 2] = "Unique Id (auto assigned)";
            ws.Cells[10, 2] = "File Mask";
            ws.Cells[11, 2] = "Folder";
            ws.Cells[12, 2] = "Recurse";
            ws.Cells[13, 2] = "Encrypted";
            ws.Cells[14, 2] = "Data Type";

            ws.Cells[8, 3] = "-1";
            ws.Cells[12, 3] = "No";
            ws.Cells[13, 3] = "No";
            ws.Cells[14, 3] = "DPD";


            // set text column width
            range = (Excel.Range)ws.Cells[2, 2];
            range.ColumnWidth = 24;


            // Add Delimiters ddl + width
            range = (Excel.Range)ws.Cells[2, 3];
            range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
                  Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=Delimiters", missing);
            range.ColumnWidth = 24;

            // Add Location
            // range = (Excel.Range)ws.Cells[11, 3];
            //range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
            //      Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=Locations", missing);


            // Add Yes No
            range = (Excel.Range)ws.Cells[12, 3];
            range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
                  Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=YesNo", missing);

            range = (Excel.Range)ws.Cells[13, 3];
            range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
                  Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=YesNo", missing);

            // Add Data Type
            range = (Excel.Range)ws.Cells[14, 3];
            range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
                  Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=DataType", missing);

            // Add Towers ddl
            range = (Excel.Range)ws.Cells[7, 3];
            range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
                  Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=MetTowers", missing);

            // Add Data Header
            for (int i = 1; i <= 1000; i++)
            {
                range = (Excel.Range)ws.Cells[i, 1];
                range.Validation.Add(Microsoft.Office.Interop.Excel.XlDVType.xlValidateList,
                      Excel.XlDVAlertStyle.xlValidAlertStop, Excel.XlFormatConditionOperator.xlBetween, "=HeaderDetails", missing);
            }
        }

        private void insertRow(int row)
        {
            Excel.Range range = (Excel.Range)ws.Cells[row, 1];
            range = range.EntireRow;
            range.Insert(Excel.XlInsertShiftDirection.xlShiftDown, System.Type.Missing);

            range = (Excel.Range)ws.Cells[row, 1];
            range = range.EntireRow;

            range.Interior.ColorIndex = 6;
            range.Interior.Pattern = Excel.Constants.xlSolid;
        }

        private void insertCol(int col)
        {
            Excel.Range range = (Excel.Range)ws.Cells[1, col];
            range = range.EntireColumn;
            try
            {
                range.Insert(Excel.XlInsertShiftDirection.xlShiftToRight, System.Type.Missing);
            }
            catch (Exception e)
            {
                string dummy = e.Message; // what is happening here?
                MessageBox.Show(e.Message);
            }
            range = (Excel.Range)ws.Cells[1, col];
            range = range.EntireColumn;

            range.Interior.ColorIndex = 6;
            range.Interior.Pattern = Excel.Constants.xlSolid;

        }

        private void showFileOpen()
        {
            string loadFile = string.Empty;

            //Microsoft.Office.Core.FileDialog fd =
            //    this.Application.get_FileDialog(Microsoft.Office.Core.MsoFileDialogType.msoFileDialogOpen);

            Microsoft.Office.Core.FileDialog fd =
                this.Application.get_FileDialog(Microsoft.Office.Core.MsoFileDialogType.msoFileDialogFilePicker);

            fd.AllowMultiSelect = false;
            fd.Filters.Clear();
            fd.Filters.Add("Text Files", "*.prn;*.txt;*.csv", missing);
            fd.Filters.Add("All Files", "*.*", missing);

            if (fd.Show() != 0)
            {
                //fd.Execute();

                loadFile = fd.SelectedItems.Item(1);

                // show Import Wizard

            }
        }


        #endregion

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
    }
}
