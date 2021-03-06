using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
//using Etools = Microsoft.Office.Tools.Excel;
using System.Data.OleDb;
using System.Diagnostics;
using WindART;
using WindART.DAL;
using System.Threading;



namespace WindowsApplication1
{

    
    
    

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            InitializeComponent();
        }
        private void CommaDelimToCols(Excel.Worksheet ws)
        {
            Excel.Range rgSheet;
            
            rgSheet = (Excel.Range)ws.Columns["A", Type.Missing];
            if (rgSheet != null)
            {

                rgSheet.TextToColumns(rgSheet, Excel.XlTextParsingType.xlDelimited, Excel.XlTextQualifier.xlTextQualifierNone, Missing.Value,
                    Missing.Value, Missing.Value, true, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                    Missing.Value, Missing.Value);

            }

        }
        private void RestoreRawTowerShadowVals()
        {


            Excel._Application oXL;
            Excel._Workbook oWBQC=null;
            Excel._Workbook oWBRaw=null;
            Excel.Range oRng;
            

            //Start Excel and get Application object.
            oXL = new Excel.Application();
            oXL.DisplayAlerts = false;
            oXL.Visible = true;

            //dictionary to store column mappings
            Dictionary<string, string> ColMap = new Dictionary<string, string>();


            string site = @"7004";
            string QCfiles = @" Select filename,startdate,enddate from siteinventory where site='" + site + "' and datatype='QC' order by startdate";

            string cnnString = @"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=C:\Documents and Settings\rich01\Desktop\Proton.mdb";
            OleDbConnection cnn = new OleDbConnection(cnnString);
            OleDbCommand QCCommand = new OleDbCommand(QCfiles, cnn);


            cnn.Open();
            OleDbDataReader QCReader;

            QCReader = QCCommand.ExecuteReader();

            // Always call Read before accessing data.
            //load raw and qc files into test folder before running this  
            string path = "F:\\test\\";
            string QCfilename = "";
            DateTime QCDateStart = Convert.ToDateTime("09/09/1899");
            DateTime QCDateEnd = Convert.ToDateTime("09/09/1899");
            string RawFileName = "";
            string RawFiles = "";
            
            
            StreamWriter TF = new StreamWriter(path + @"\" + site + @"ReprocessLog.txt");

            Excel.Workbook newWorkbook =
                    oXL.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            Excel.Worksheet NewWksht = (Excel.Worksheet)newWorkbook.Sheets[1];

            //when onhly one raw file open it here 
            /*RawFileName = @"7002_030804_081408.txt";
            oWBRaw = oXL.Workbooks.Open(path + RawFileName,
             Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
             Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

            Excel.Worksheet RawWs = (Excel.Worksheet)oWBRaw.Sheets[1];
            CommaDelimToCols(RawWs);*/
            
            while (QCReader.Read())
            {
                QCfilename = QCReader.GetString(0);
                QCDateStart = QCReader.GetDateTime(1);
                RawFiles =@" Select filename from siteinventory where site='" + site + "' and Datatype='Raw' and StartDate=#" + QCDateStart + "#";
                OleDbCommand RawCommand = new OleDbCommand(RawFiles, cnn);
                //get filename of raw file using the date start of the qc file we are on
                

                try
                {
                    RawFileName = RawCommand.ExecuteScalar().ToString();



                    //open both worksheets 
                    oWBQC = oXL.Workbooks.Open(path + QCfilename,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);


                    Excel.Worksheet QCws = (Excel.Worksheet)oWBQC.Sheets[1];
                    CommaDelimToCols(QCws);
                    oRng = QCws.UsedRange;
                    //replace missings with -9999
                    string DateAddr = FindDateTimeCells(QCws);
                    Excel.Range RngFound = oRng.get_Range(DateAddr, Missing.Value);
                    //RngFound = RngFound.get_Offset(1, 0);
                    //Debug.Print(RngFound.Row.ToString() + ":" + RngFound.Column.ToString());
                    Excel.Range Alldata = oRng.get_Range(RngFound,
                       oRng.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Missing.Value));
                    //Debug.Print(Alldata.get_End(Excel.XlDirection.xlToRight).Column.ToString () + ":"
                    //    + Alldata.get_End(Excel.XlDirection.xlDown).Row.ToString());

                    Alldata.Replace(@"", @"-9999", Excel.XlLookAt.xlWhole, Missing.Value, false, Missing.Value,
                                                      Missing.Value, Missing.Value);
                    Alldata.Replace(@"ice", @"-9999", Excel.XlLookAt.xlWhole, Missing.Value, false, Missing.Value,
                               Missing.Value, Missing.Value);
                    Alldata.Replace(@"msn", @"-9999", Excel.XlLookAt.xlWhole, Missing.Value, false, Missing.Value,
                                   Missing.Value, Missing.Value);
                    Alldata.Replace(@"mal", @"-9999", Excel.XlLookAt.xlWhole, Missing.Value, false, Missing.Value,
                                   Missing.Value, Missing.Value);
                    
                    

                    
                    
                    oWBRaw = oXL.Workbooks.Open(path + RawFileName,
             Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
             Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                    Excel.Worksheet RawWs = (Excel.Worksheet)oWBRaw.Sheets[1];
                    CommaDelimToCols(RawWs);
                    
                    

                    //find each instance of "twr"

                    Excel.Range currentFind = oRng.Find("twr", Missing.Value,
                    Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                    Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                    Missing.Value, Missing.Value);

                    Excel.Range DateFind = null;
                    //for each instance of "twr" found catalog the address
                    string twrAddress = "";
                    int twrCol = 0;
                    while (currentFind != null)
                    {
                        //get address, date, and col header of "twr" value found  in validated 

                        

                        twrAddress = currentFind.get_Address(true, true, Excel.XlReferenceStyle.xlR1C1, Missing.Value, Missing.Value);
                        twrCol = currentFind.Column;
                        DateTime TwrDate = DateTime.FromOADate(Convert.ToDouble(currentFind.get_Offset(0, -1 * (twrCol - 1)).Value2.ToString()));


                        
                        if (DateFind == null)
                        {
                            DateFind = RawWs.Cells.get_Range ("A1",Missing.Value);
                        }
                        
                        //find date in raw file and move to same col as in val file
                        DateFind = RawWs.Cells.Find(TwrDate, DateFind ,
                                       Excel.XlFindLookIn.xlFormulas, Excel.XlLookAt.xlPart ,
                                       Excel.XlSearchOrder.xlByRows , Excel.XlSearchDirection.xlNext, false,
                                       Missing.Value, Missing.Value);

                        //Debug.Print(DateFind.Row.ToString());
                        // Debug.Print(DateFind.Column.ToString());
                        double ReplaceVal = -8888;
                        if (DateFind != null)
                        { ReplaceVal = Convert.ToDouble(DateFind.get_Offset(0, twrCol - 1).Value2.ToString()); }
                        //replace the twr cell found with the value retreived fromt he raw sheet
                        QCws.Cells[currentFind.Row, twrCol] = ReplaceVal;



                        currentFind = oRng.Find("twr",currentFind,
                   Excel.XlFindLookIn.xlValues, Excel.XlLookAt.xlPart,
                   Excel.XlSearchOrder.xlByRows, Excel.XlSearchDirection.xlNext, false,
                   Missing.Value, Missing.Value);
                        if (currentFind != null)
                        {


                            Debug.Print(currentFind.Row.ToString());
                            //  Debug.Print(currentFind.Column.ToString());
                        }


                    }
                    // always call Close when done reading.
                    TF.WriteLine(@"Processed " + QCfilename + @" and " + RawFileName);
                    
                    RawFileName = @"";
                    Excel.Range CopyDest = NewWksht.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Missing.Value);
                    CopyDest = CopyDest.get_Offset(2, -1 * (CopyDest.Column -1));


                    Debug.Print(CopyDest.Row.ToString() + ":" + CopyDest.Column.ToString());

                    Alldata.Copy(CopyDest); 
                    oWBQC.Close(true, path + QCfilename, Missing.Value);
                    oWBRaw.Close(false, Missing.Value, Missing.Value);
                }
                catch (Exception theException)
                {
                    String errorMessage;
                    errorMessage = "Error: ";
                    errorMessage = String.Concat(errorMessage, theException.Message);
                    errorMessage = String.Concat(errorMessage, " Line: ");
                    errorMessage = String.Concat(errorMessage, theException.Source);

                    TF.WriteLine(errorMessage + @"QC File: " + QCfilename  + @" RawFile: " + RawFileName , "Error");
                    
                }
                
                
            
        }

        newWorkbook.Close(true, path + site + @"_AllValidated.xls", Missing.Value);
            QCReader.Close();
                // Close the connection when done with it.
        
                cnn.Close();
                oXL.Quit();
                TF.Close();
            
        }
        private void TestExcelOLEDB()
        {
            string xlsheet = GetFile();
            string cnnString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + xlsheet + ";Extended Properties=" 
                + (char)34 + "Excel 8.0;HDR=NO;" + (char)34;

            OleDbConnection Cnn = new OleDbConnection(cnnString );
            OleDbDataAdapter da;
            da = new OleDbDataAdapter("Select [1] FROM [Sheet1$] ", Cnn);

            DataSet ds = new DataSet();
            ds.Tables.Add("XlData");

            da.Fill(ds, "XlData");

            
            

        }
        private void StackExcelFiles()
        {
            //append many raw files into 1 
            bool HeaderHasBeenCopied=false;
            bool ShouldRunTextToColumnsFirst=false;

            Excel._Application oXL;
            Excel._Workbook oWBRaw = null;
            Excel.Range oRng;
            
            string path = GetFolder();
            string[] files= Directory.GetFiles (path);
            
            
            //Start Excel and get Application object.
            Type t = Type.GetTypeFromProgID("Excel.Application.14");
            oXL = (Excel.Application)Activator .CreateInstance (t);
            oXL.DisplayAlerts = false;
            oXL.Visible = true;
      // Always call Read before accessing data.
            //load raw and qc files into test folder before running this  
            
            Excel.Workbook newWorkbook =
                    oXL.Workbooks.Add(Excel.XlWBATemplate.xlWBATWorksheet);
            Excel.Worksheet NewWksht = (Excel.Worksheet)newWorkbook.Sheets[1];
            

            foreach (string file in files)
                { 
                try
                {
                    

                    //open both worksheets 
                    oWBRaw = oXL.Workbooks.Open(file,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                    Excel.Worksheet RawWs = (Excel.Worksheet)oWBRaw.Sheets[1];
                    oRng = RawWs.UsedRange;
                    string ext=Path.GetExtension(file).ToLower();
                    //text to columns first if the file is a prn
                    if (ext == @".prn" || ext == @".txt" ||
                        ext == @".asc" || ext == @".dat" ||
                        ext.Contains("val"))
                    {
                        ShouldRunTextToColumnsFirst = true;
                        CommaDelimToCols(RawWs); 
                    }


                    string DateHeader = string.Empty; 
                    string DateAddr = string.Empty;
                    
                    if (ext == ".asc" || ext == ".dat")
                    {
                        DateHeader = @"$A$1";
                        DateAddr = @"$A$1";
                    }
                    else
                    {
                        DateHeader=FindDateTimeHeader(RawWs);
                        DateAddr=FindDateTimeCells(RawWs);
                    }


                    //delete rows between header and first actual date field
                    Excel.Range DeleteRowsBetween = oRng.get_Range(oRng.get_Range(DateHeader, Missing.Value).get_Offset(1, 0),
                       oRng.get_Range(DateAddr, Missing.Value).get_Offset(1, 100));
                    if (Math.Abs(oRng.get_Range(DateHeader, Missing.Value).Row - DeleteRowsBetween.Row) > 1)
                        DeleteRowsBetween.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                    
                    //delete all rows above the header row 
                   // try
                   // {
                    //Excel.Range DeleteRowsAbove = oRng.get_Range(oRng.get_Range(DateHeader, Missing.Value).get_Offset(-1, 0) as Excel.Range, RawWs.Cells[1, 100] as Excel.Range);
                    //DeleteRowsAbove.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                   // }
                   //catch { }

                    if (ShouldRunTextToColumnsFirst == false)
                    {
                       CommaDelimToCols(RawWs);
                    }
                    
                    //header row is now the first row 
                    Excel.Range HeaderRow;
                    
                    
                        
                    
                    
                    //clean unprintable characters 
                    //Excel.Range lastrow = oRng.get_Range(RawWs.Cells[oRng.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, 
                    //    Missing.Value).Row, 1], oRng.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Missing.Value));

                    //foreach (Excel.Range r in lastrow)
                    //{
                    //    r.Value2   = oXL.WorksheetFunction.Clean(r.Value2.ToString () );
                    //}
                    
                   
                    Excel.Range CopyDest = NewWksht.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Missing.Value);
                    CopyDest = CopyDest.get_Offset(1, -1 * (CopyDest.Column - 1));
                    

                    Debug.Print(CopyDest.Row.ToString() + ":" + CopyDest.Column.ToString());
                    //if (!HeaderHasBeenCopied)
                    //{
                    //    Excel.Range CopyHeaderDest = NewWksht.Cells.get_Range(NewWksht.Cells[1, 1] as Excel.Range, NewWksht.Cells[1, 100] as Excel.Range);


                    //    HeaderRow = RawWs.Cells.get_Range
                    //        (RawWs.Cells[1, oRng.get_Range(DateHeader, Missing.Value).Column] as Excel.Range, RawWs.Cells[1, 100] as Excel.Range);
                    //    Debug.Print(oRng.get_Range(DateHeader, Missing.Value).Row + " " + oRng.get_Range(DateHeader, Missing.Value).Column);
                    //    HeaderRow.Copy(CopyHeaderDest);
                    //    HeaderHasBeenCopied = true;
                    //}

                    Excel.Range Alldata = oRng.get_Range(RawWs.Cells[1, oRng.get_Range(DateHeader, Missing.Value).Column] as Excel.Range,
                            oRng.SpecialCells(Excel.XlCellType.xlCellTypeLastCell, Missing.Value) as Excel.Range);
                    Alldata.Copy(CopyDest);
                    oWBRaw.Close(true, file, Missing.Value);
                    
                }
                catch (Exception theException)
                {
                    String errorMessage;
                    errorMessage = "Error: ";
                    errorMessage = String.Concat(errorMessage, theException.Message);
                    errorMessage = String.Concat(errorMessage, " Line: ");
                    errorMessage = String.Concat(errorMessage, theException.Source);

                    Debug.Print(errorMessage);

                }



            }

            newWorkbook.Close(true, path +  @"\AllRaw.xlsx", Missing.Value);
           
            // Close the connection when done with it.

           
            oXL.Quit();
            

        }
        private void ExportWorksheets()
        {

            Excel._Application oXL;
            Excel._Workbook WB;
            string folderPath = GetFolder();
            string SaveToFolder = GetFolder();
            oXL =new Excel.Application() ;
            oXL.DisplayAlerts = false;
            oXL.Visible = true;
            string[] fileEntries = Directory.GetFiles(folderPath);
            foreach (string fileName in fileEntries)
            {
                
                WB = oXL.Workbooks.Open(fileName,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                string wbName = WB.Name.ToString ();
                foreach (Excel.Worksheet ws in WB.Worksheets)
                {
                    Debug.Print(wbName );
                    Debug.Print(ws.Name.ToString ());
                    
                    string SaveTo = SaveToFolder + @"\" + ws.Name.ToString() + @"_" + wbName;
                     ws._SaveAs(SaveTo , Excel.XlFileFormat.xlCSV, Missing.Value, Missing.Value,
                            Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);
                    


                }
                WB.Close(false, fileName, Missing.Value);
            }
            oXL.Quit();
        }
        private void ReorderExcelColumns()
        {
            //take one excel file and apply the order of its columns to a target excel file 

            string sourcefile = string.Empty;
            string targetfile = string.Empty;

            sourcefile = GetFile();
            targetfile = GetFile();
            
            StreamReader sr = new StreamReader(sourcefile );
            
            List<string> SourceCols = new List<string>();
            Dictionary <string, List<double>> TargetCols = new Dictionary<string, List<double>>();
            
            //loop the source datatable, find each column in the target table, output to csv
            SourceCols.AddRange (sr.ReadLine().Split(','));

            SourceCols.ForEach(c => c.Trim());
            
            sr.Close();

           // sourcedr = null;

            StreamReader tr = new StreamReader(targetfile);
            DataTable targetData = new DataTable();
            List<string> colHeads = new List<string>();
            List<string> thisline = new List<string>();
            List<double> thisVal = new List<double>();
            List<DateTime> thisDate = new List<DateTime>();
            List<DateTime> thisData = new List<DateTime>();
            int count = 0;
            while (!tr.EndOfStream)
            {
                if (count == 0)
                {
                    colHeads.AddRange(tr.ReadLine().Split(','));
                    colHeads.ForEach(c => TargetCols.Add( c, new List<double>()));
                    colHeads.ForEach(c => c.Trim(' '));
                }
                else
                {
                    thisline.AddRange(tr.ReadLine().Split(','));

                    for (int i = 0; i < colHeads.Count; i++)
                    {
                        if (i == 0)
                        {
                            //date

                            thisDate.Add(DateTime.Parse(thisline[i]));
                        }

                        else
                        {
                            double outD = 0.0;
                            if (double.TryParse(thisline[i].ToString(), out outD))
                            {
                                TargetCols[colHeads[i]].Add(outD);
                            }
                            else
                            {
                                TargetCols[colHeads[i]].Add(-9999.00);
                            }



                        }

                    }

                    thisline.Clear();
                    
                }
count++;
            }
          

            // write the results to csv
                    
                     string filename=Path.GetDirectoryName (targetfile ) + @"\" + Path.GetFileNameWithoutExtension(targetfile ) + "_Reordered.csv"; 
                     
                     StreamWriter sw = new StreamWriter(filename, false);

                     //column heads
                     foreach (string col in  SourceCols )
                     {
                         sw.Write(col.Replace("#", "."));
                        
                         //if we are at the end of the line don't add comma 
                         if (SourceCols.IndexOf(col) != (SourceCols.Count - 1))
                         {
                             sw.Write(",");
                         }
                     }



                     sw.Write(sw.NewLine);


                      //Now write all the rows.
                    //loop rows
                         for (int i = 0; i < TargetCols[colHeads[1]].Count ; i++)
                         {
                             foreach (string col in SourceCols )
                             {
                                 string work = col.Trim();
                                 if (work == "DateTime")
                                 {
                                     sw.Write(thisDate[i].ToString());
                                 }
                                 else
                                 {   
                                     if (!TargetCols.ContainsKey(work))
                                     {
                                         sw.Write("colNotFound");
                                     }
                                     else
                                     {
                                         if (!Convert.IsDBNull(TargetCols[work][i]) & TargetCols[work][i].ToString().Length > 0)
                                         {
                                             sw.Write(TargetCols[work][i].ToString());
                                         }
                                         else
                                         {
                                             sw.Write("-9999.99");
                                         }
                                     }
                                 }
                                 if (colHeads .IndexOf (work)!=(colHeads .Count-1))
                                 {
                                     sw.Write(",");
                                 }
                             } 
                             sw.Write(sw.NewLine);
                         }
                         
                     
                     sw.Close();

                     TargetCols = null;
                     
                 }
        private string GetFolder()
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();

            dialog.Description = "Select Folder";
            dialog.ShowNewFolderButton = false;
            //dialog.SelectedPath = @"C:\My Documents\Temporary Documents";
            //@"C:\EHarvest\Management\ObserverExportFiles\Text Files";

            if (dialog.ShowDialog() == DialogResult.OK)
            { }

            return dialog.SelectedPath;
        }
        private string GetFile()
        {
            FileDialog fd = new OpenFileDialog();
            

            fd.Title  = "Select File";
            
            //dialog.SelectedPath = @"C:\My Documents\Temporary Documents";
            //@"C:\EHarvest\Management\ObserverExportFiles\Text Files";

            if (fd.ShowDialog() == DialogResult.OK)
            { }

            return fd.FileName ;
        }
        
       private static OleDbConnection  Connect()
        {
            
            OleDbConnection Cnn = new OleDbConnection(
           @"Provider=SQLNCLI;Data Source=10.128.10.8;Initial Catalog=WindART_SMI;User Id=Windart_Jesserichards; password=metteam555");
            Cnn.Open();
            return Cnn;
            
           
        }
        private bool IsDistinctSite(string siteid)
        {
            string sqlstring=@"select count(*) from tblsite where oldid='" + siteid + "'";

            using (OleDbCommand cmd = new OleDbCommand(sqlstring ))
            {


                cmd.CommandType = CommandType.Text;
                cmd.Connection = Connect();
                int idcount = (int)cmd.ExecuteScalar();
                cmd.Connection.Close();

                if (idcount > 1)
                    return false;
                else
                    return true;


            }
        }
        private void InsertSiteInventory(string siteID, string filename, 
            DateTime StartDate, string StartTime, DateTime EndDate, string EndTime, int Rowcount)
    {
            //check to see if record is there first 
        try
        {
            
            string insert = @"insert into tblsitedatafileinventory (oldsiteid,filename,StartDate,StartTime,EndDate,EndTime,DataRowcount) 
            values('" + siteID + "','" + filename + "','" + StartDate + "','" + StartTime + "', '" + EndDate + "','" + EndTime + "'," + Rowcount + ")";
            Debug.Print(filename);
            using (OleDbCommand cmd = new OleDbCommand(insert))
            {


                cmd.CommandType = CommandType.Text;
                cmd.Connection = Utils.Connect();
                cmd.ExecuteNonQuery();
                cmd.Connection.Close();


            }
        }
        catch(Exception e)
        {
            MessageBox.Show(e.Message);
        }
            }   
        private string  FindDateTimeCells(Excel.Worksheet ws)
        {
            Excel.Range rgFound;
             string sFirstFoundAddress;
             string[] searchVals = new string[] { @"TIMESTAMP", @"Date & Timestamp", @"DTTM",@"Date", @"TimeStamp", @"Year", @"Date & Time", @"Date/Time" };
                        
                // Find's parameters are "sticky". If you don't specify them
                // they'll default to the last used values - including parameters
                // set via Excel's user interface

            sFirstFoundAddress = @"not assigned";
            foreach (string val in searchVals)
            {
                rgFound = ws.Cells.Find(val ,
                                        ws.Cells[1, 1],
                                        Excel.XlFindLookIn.xlValues,
                                        Excel.XlLookAt.xlPart,
                                        Missing.Value,
                                        Excel.XlSearchDirection.xlNext,
                                        false,
                                        Missing.Value,
                                        Missing.Value);
                if (rgFound != null)
                {   
                    bool moveon=false;
                    string address = string.Empty;
                    int i=0;
                    string firstdate = string.Empty;
                    double DecDate=default(double);
                    DateTime res = default(DateTime);
                    //iterate until the first actual date is found
                    //since they have decided to insert all manner of garbage between the header and the actual values 
                    while (!moveon)
                    {
                        

                        try
                        {
                            firstdate = rgFound.get_Offset(i, 0).Value2 .ToString ();
                            if (firstdate.Length > 0)
                            {
                                DecDate = Convert.ToDouble(firstdate);
                                res = DateTime.FromOADate(DecDate);
                                rgFound = rgFound.get_Offset(i, 0);
                                moveon = true;
                            }
                            else
                                i++;
                        }
                        
                        catch
                        {
                            i++;
                        }
                    }


                    sFirstFoundAddress = rgFound.get_Address(
                            true, true, Excel.XlReferenceStyle.xlA1, Missing.Value, Missing.Value);
                    
                                        
                    break;
                    
                    
                }
                else 
                {

                    continue;
                }
                
            }

            
            return sFirstFoundAddress;
            
        }
        private string FindDateTimeHeader(Excel.Worksheet ws)
        {
            Excel.Range rgFound;
            string sFirstFoundAddress;
            string[] searchVals = new string[] {@"TIMESTAMP", @"Date & Timestamp", @"DTTM", @"Date", @"TimeStamp", @"Year", @"Date & Time", @"Date/Time" };

            // Find's parameters are "sticky". If you don't specify them
            // they'll default to the last used values - including parameters
            // set via Excel's user interface

            sFirstFoundAddress = @"not assigned";

            foreach (string val in searchVals)
            {
                rgFound = ws.Cells.Find(val,
                                        ws.Cells[1, 1],
                                        Excel.XlFindLookIn.xlValues,
                                        Excel.XlLookAt.xlPart,
                                        Missing.Value,
                                        Excel.XlSearchDirection.xlNext,
                                        false,
                                        Missing.Value,
                                        Missing.Value);
                

                if (rgFound !=null)
                {
                    sFirstFoundAddress = rgFound.get_Address(
                            true, true, Excel.XlReferenceStyle.xlA1, Missing.Value, Missing.Value);
                    break;
                }
                else
                {
                    continue;
                }

            }

            
            return sFirstFoundAddress;

        }
        public static DateTime JulianToDateTime(string year, string doy, string hoursMinutes)
        {
            DateTime dtOut = new DateTime(int.Parse(year), 1, 1);
            dtOut = dtOut.AddDays(int.Parse(doy) - 1);

            switch (hoursMinutes.Length)
            {
                case 2:
                    dtOut = dtOut.AddMinutes(double.Parse(hoursMinutes));
                    break;
                case 3:
                    dtOut = dtOut.AddHours(double.Parse(hoursMinutes.Substring(0, 1)));
                    dtOut = dtOut.AddMinutes(double.Parse(hoursMinutes.Substring(1, 2)));
                    break;
                case 4:
                    dtOut = dtOut.AddHours(double.Parse(hoursMinutes.Substring(0, 2)));
                    dtOut = dtOut.AddMinutes(double.Parse(hoursMinutes.Substring(2, 2)));
                    break;
            }

            return dtOut;
        }
        public static bool IsDate(string anyString) 
        {
            if (anyString == null) 
            { anyString = ""; } 
            if (anyString.Length > 0) 
            { DateTime dummyDate ; 
                try 
            { dummyDate = DateTime.Parse(anyString); } 
            catch 
            { 
                return false; 
            } 
                return true; 
            } 
            else 
            { 
                return false; 
            }
        }
        private void CatalogueFiles()
        {
            //enter the date information for each excel file in a folder into the proton database 
            Excel._Application oXL;
            Excel._Workbook oWB=null;
            Excel.Range oRng;
            Excel.Range Used;

            //directoryb to get files from 
            string FolderPath = GetFolder();


            //Start Excel and get Application object.
            oXL = new Excel.Application();
            oXL.DisplayAlerts = false;
            oXL.Visible = true;


            string[] fileEntries = Directory.GetFiles(FolderPath);
            string flname = string.Empty;
            foreach (string fileName in fileEntries)
            {
                //site assumes site number is always the first 4 characters in a string 
                string sitestring = FindSiteString(fileName);
                if (!IsDistinctSite(sitestring) )continue;

                try
                {

                    flname = Path.GetFileName(fileName).ToString();
                    if (FileExists(flname))
                        continue;

                    oWB = oXL.Workbooks.Open(fileName,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value,
                Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value, Missing.Value);

                    

                    foreach (Excel.Worksheet displayWorksheet in oWB.Worksheets)
                    {
                        displayWorksheet.Cells.Replace(@"""", @"", Excel.XlLookAt.xlPart, Excel.XlSearchOrder.xlByColumns, false, Missing.Value,
                            Missing.Value, Missing.Value);

                       
                            //convert text to columns
                            this.CommaDelimToCols(displayWorksheet);

                            //Excel.Range DeleteRange = displayWorksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeBlanks, Missing.Value);

                            //if (DeleteRange != null)
                            //{
                            //    DeleteRange.Cells.Delete(Missing.Value);

                            //}
                            //DeleteRange = null;
                        


                        Used=displayWorksheet.UsedRange ;

                        string FlExt = flname.Substring(flname.Length - 3, 3).ToLower();

                        string DateHeader = string.Empty;
                        string DateAddr = string.Empty;
                        
                        if (FlExt == @"asc")
                        { DateAddr  = @"C2"; }
                        else
                        {
                            DateHeader = FindDateTimeHeader(displayWorksheet);
                            DateAddr = FindDateTimeCells(displayWorksheet);
                            //delete garbage rows above header
                            Excel.Range DeleteRowsBetween = Used.get_Range("$A$1",
                               Used.get_Range(DateHeader, Missing.Value).get_Offset(-1, 100));
                            int toprowoffset=Math.Abs(Used.get_Range(DateHeader, Missing.Value).Row - DeleteRowsBetween.Row) ;
                                
                            if (toprowoffset>0)
                                    DeleteRowsBetween.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                            
                            //adjust address of dateheaders
                                DateHeader = Used.get_Range(DateHeader).get_Offset((-1 * toprowoffset),0).get_Address(
                            true, true, Excel.XlReferenceStyle.xlA1, Missing.Value, Missing.Value);
                                DateAddr = Used.get_Range(DateAddr).get_Offset(( -1 * toprowoffset),0).get_Address(
                            true, true, Excel.XlReferenceStyle.xlA1, Missing.Value, Missing.Value);
 
                           //delete rows between header and first actual date field
                             DeleteRowsBetween = Used.get_Range(Used.get_Range(DateHeader , Missing.Value).get_Offset(1, 0),
                               Used.get_Range(DateAddr  , Missing.Value).get_Offset(-1, 100));
                            
                                if (Math.Abs(Used.get_Range(DateHeader, Missing.Value).Row - DeleteRowsBetween.Row) > 0)
                                    DeleteRowsBetween.Delete(Excel.XlDeleteShiftDirection.xlShiftUp);
                            }
                        
                        oRng = displayWorksheet.get_Range(DateAddr , Missing.Value);

                        //get last row 
                        Excel.Range WholeRng = displayWorksheet.get_Range(DateAddr, oRng.get_End(
                            Excel.XlDirection.xlDown).get_Address(
                            true, true, Excel.XlReferenceStyle.xlA1, Missing.Value, Missing.Value));

                        string formatString;
                        DateTime EndDate = Convert.ToDateTime(@"09/09/1909"); 
                        string EndTime = "";
                        string firstdate = "";
                        string lastdate = "";
                        double DecDate;
                        string StartTime = "";
                        DateTime StartDate = Convert.ToDateTime(@"09/09/1909");
                        int cnt = 0;

                        //if year is found we are dealing with julian dates 
                        if (oRng.get_Value(Missing.Value).ToString() == "Year")
                        {

                            if (oRng.get_Offset(0, 1).Value2.ToString() == "Month")
                            {
                                StartDate = Convert.ToDateTime(oRng.get_Offset(1, 1).Value2.ToString() + @"/" +
                                oRng.get_Offset(1, 2).Value2.ToString() + @"/" +
                                oRng.get_Offset(1, 0).Value2.ToString());
                            }
                            else
                            {
                                StartDate = JulianToDateTime(oRng.get_Offset(1, 0).Value2.ToString(),
                                    oRng.get_Offset(1, 1).Value2.ToString(),
                                    oRng.get_Offset(1, 2).Value2.ToString());
                            }

                        }

                        else if (FlExt == "asc" || FlExt=="prn")
                        {
                            StartDate = JulianToDateTime(oRng.get_Offset(0, 0).Value2.ToString(),
                            oRng.get_Offset(0, 1).Value2.ToString(),
                            oRng.get_Offset(0, 2).Value2.ToString());
                        }
                        else
                        //find the start date of a normal date 
                        {
                            foreach (Excel.Range cell in WholeRng)
                            {
                                if (null == cell.Value2)
                                {
                                    continue;
                                }
                                else
                                {


                                    formatString = cell.Value2.GetType().ToString();
                                    if (formatString == "System.Double")
                                    {
                                        firstdate = cell.Value2.ToString();
                                        DecDate = Convert.ToDouble(firstdate);
                                        if (DecDate != 0)//make sure it is a valid date
                                        {
                                            try
                                            {
                                                StartDate = DateTime.FromOADate(DecDate);

                                                break;
                                            }
                                            catch
                                            {
                                                continue;
                                            }
                                        }
                                    }




                                }
                            }

                        }

                        StartTime = StartDate.ToShortTimeString();
                        cnt = oRng.get_End(Excel.XlDirection.xlDown).Row - oRng.Row;
                        if (oRng.get_Value(Missing.Value).ToString() == "Year" || FlExt == "asc")
                        {
                            EndDate = JulianToDateTime(oRng.get_Offset(cnt, 0).Value2.ToString(),
                                                       oRng.get_Offset(cnt, 1).Value2.ToString(),
                                                       oRng.get_Offset(cnt, 2).Value2.ToString());
                            EndTime = EndDate.ToShortTimeString();

                        }
                        else
                        {
                            lastdate = oRng.get_End(Excel.XlDirection.xlDown).Value2.ToString();
                            DecDate = Convert.ToDouble(lastdate);
                            EndDate = DateTime.FromOADate(DecDate);
                            EndTime = DateTime.FromOADate(DecDate).ToShortTimeString();
                        }
                        //insert into table
                        //this block for use when not opening excel sheets



                        
                        this.InsertSiteInventory(sitestring , flname, StartDate, StartTime, EndDate, EndTime, cnt);

                        oWB.Close(false, fileName, Missing.Value);

                    }

                }
                
                catch (Exception e)
                {
                    Debug .Print (flname + " not imported. error: " + e.Message);

                    oWB.Close(false, fileName, Missing.Value);
                }
            }
            
        }
        private void MoveUncataloguedFiles()
        {
            //Files taht can't be catalogued can be moved with this. it uses the first 4 characters in the filename to identify the site and move the file to the appropriate folder 

            string cnnString = @"Provider=SQLNCLI; Data Source=10.128.10.8;Initial catalog=WindART_SMI;User Id=WindART_JesseRichards;password=metteam555";
            OleDbConnection cnn = new OleDbConnection(cnnString);
           
            cnn.Open();


            string sourcepath = "E:\\Work2\\";
            string targetpath = "";
            string sitename="";
            
            List<string> keys = new List<string>();
            List<string> Uniquekeys = new List<string>();
            Dictionary<string, List<string>> sites = new Dictionary<string, List<string>>();
            
            
                               
                //get files from work
               keys=  Directory.GetFiles(sourcepath).ToList();
               Uniquekeys = keys.Select (c => Path.GetFileName (c).Substring(0,4)).Distinct ().ToList();
               foreach(string s in Uniquekeys)
               {
                   List<string> thisSiteList=Directory.GetFiles(sourcepath,s + "*").ToList ();
                   sites.Add(s,thisSiteList );
               }
               //get distinct sites
               
                foreach(KeyValuePair <string,List<string>> kv in sites)
                {
                
                        //figure out what the siteid is 
                    string thissite = kv.Key;

                        string sql = @"select top 1 BPID from tblsite where oldid like '" + thissite + "' and (select count(*) from tblsite where oldid like '" + thissite + "') =1";
                        OleDbCommand  myCommand = new OleDbCommand  (sql,cnn);
                       
                        OleDbDataReader reader = myCommand.ExecuteReader();
                        while (reader.Read())
                        {
                            sitename = reader[0].ToString();
                        }
                        if (sitename.Length == 0) continue;

                        targetpath = @"E:\BPWindData\" + sitename + @"\";

                        if (Directory.Exists(targetpath))
                        {
                        }
                        else
                        {
                            Directory.CreateDirectory(targetpath);
                        }
                        foreach (string file in kv.Value.ToList())
                        {
                            Debug.Print(Path.GetFileName(Path.GetFileName (file)));
                            try
                            {
                                File.Copy(file, targetpath + Path.GetFileName(file),true);
                                File.Delete(file);
                            }
                            catch (Exception e)
                            {
                                Debug.Print(e.Message + "   " + Path.GetFileName(file));
                            }
                        }
                                     

                }
                    
           
            cnn.Close();
   
        }
        private void MoveFiles()
        {


            string insert = @" Select BPID,filename,idcount from tblsitedatafileinventory sdi join vwProton p on 
                            p.oldid=sdi.oldsiteid where dateinserted > '12/14/2010'";
            string cnnString = @"Provider=SQLNCLI; Data Source=10.128.10.8;Initial catalog=WindART_SMI;User Id=WindART_JesseRichards;password=metteam555";
            OleDbConnection cnn = new OleDbConnection(cnnString);
            OleDbCommand myCommand = new OleDbCommand(insert, cnn);
            cnn.Open();
            OleDbDataReader myReader;
            myReader = myCommand.ExecuteReader();
            // Always call Read before accessing data.
            string path = "E:\\Work\\";
            string newpath = "";
            string foldername = "";
            string filename = "";
            string[] files;
            while (myReader.Read())
            {
                if (myReader["idcount"].ToString() == "2")
                    continue;
                filename = myReader.GetString(1);
                foldername = myReader.GetString(0);
                newpath = @"E:\BPWindData\" + foldername + @"\";

                // string[] dir = Directory.GetDirectories(path);
                //foreach (string d in dir)
                //{
                string d = path;

                files = Directory.GetFiles(d, filename, SearchOption.AllDirectories);
                if (files.Length > 0)
                {
                    for (int i = 0; i <= files.Length - 1; i++)
                    {
                        if (Directory.Exists(newpath))
                        {
                        }
                        else
                        {
                            Directory.CreateDirectory(newpath);
                        }
                        Debug.Print(Path.GetFileName(files[i].ToString()));
                        try
                        {
                            File.Move(files[i].ToString(), newpath + Path.GetFileName(files[i].ToString()));
                            File.Delete(Path.GetFileName(files[i].ToString()));
                        }
                        catch (Exception e)
                        {
                            Debug.Print(e.Message + "   " + Path.GetFileName(files[i].ToString()));
                        }


                    }

                }

                //}

            }
            // always call Close when done reading.
            myReader.Close();
            // Close the connection when done with it.
            cnn.Close();

        }
        private void ReprocessPipelineFiles()
        {
            //get the list of files from loaded folder
            //find the files in data catalog
            //move files to test folder

            string insert = @" Select distinct  originalname from filelist2 order by originalname";
            string cnnString = @"Provider=SQLNCLI; Data Source=10.128.10.8;Initial catalog=WindART_SMI;User Id=WindART_JesseRichards;password=metteam555";
            OleDbConnection cnn = new OleDbConnection(cnnString);
            OleDbCommand myCommand = new OleDbCommand(insert, cnn);
            cnn.Open();
            OleDbDataReader myReader;
            myReader = myCommand.ExecuteReader();
            // Always call Read before accessing data.
            string path = @"E:\";
            string[] directories = Directory.GetDirectories(path);
            string newpath = "";
            string filename = "";
            string[] files;
            bool found;
            List<string> fileList=new List<string>();
            System.IO.StreamWriter  fs = new StreamWriter (newpath + "FilesNotFound.txt");
            while (myReader.Read())
            {
                if (myReader.GetString(0).Split('_').Length > 1)
                {
                    filename = myReader.GetString(0).Split('_')[1].Replace(".txt", ".rwd");
                }
                else
                {
                    filename = myReader.GetString(0);
                }

                Console.WriteLine("adding " + filename);
                fileList.Add(filename);
            }
            myReader.Close();
            cnn.Close();
                newpath = @"E:\Test";

                
            foreach(string f in fileList)
            {

                found = false;

                if (!File.Exists(newpath + "\\" + f))
                {
                    foreach (string dir in directories)
                    {
                        
                        try
                        {
                            if (dir == newpath) continue;

                            files = Directory.GetFiles(dir, f, SearchOption.AllDirectories);
                            if (files.Length > 0)
                            {
                                for (int i = 0; i <= files.Length - 1; i++)
                                {
                                    Console.WriteLine(f);

                                    File.Copy(files[i].ToString(), newpath + "\\" + f, false);
                                    found = true;
                                    break;

                                }

                            }
                           
                        }
                        catch (UnauthorizedAccessException )
                        {
                            continue;
                        }
                    }
                    if (!found)
                    {
                        Console.WriteLine(f + "not found");
                        fs.WriteLine(f);
                    }
                }

            }
            // always call Close when done reading.
            
            MessageBox.Show("done");
            fs.Close();
        }
        private void ReloadPipelineFiles()
        {
            string dir = @"\\10.128.10.8\0.EmailFolder\";
            string localArchive=@"E:\Files From Server\FilesReloadedForEkho\";
           

            string[] filestoReload = Directory.GetFiles(@"E:\Test");

            int filecount= filestoReload.Length;

            

            //as long as there are files to reload keep going
           while (filecount > 0)
            {
                string[] files = Directory.GetFiles(dir, "*", SearchOption.AllDirectories);
                
                if (files.Length == 0)
                {
                    System.IO.StreamWriter fs = new StreamWriter(@"E:Files From Server\" + "FilesCopiedToServer.txt", true);
                    for (int i = 0; i < 200; i++)
                    {
                        //copy files from test to email folder and log
                        try
                        {
                            File.Copy(filestoReload[i], dir + Path.GetFileName(filestoReload[i]), true);
                            File.Move(filestoReload[i], localArchive + Path.GetFileName(filestoReload[i]));
                            fs.WriteLine();
                            fs.Write(filestoReload[i] + ", loaded to " + dir + " at " + DateTime.Now);
                        }
                        catch 
                        {
                            i = i - 1;
                        }
                    }
                    
                    //see where we are after doing the last bit of reloading 
                    filestoReload = Directory.GetFiles(@"E:\Test");
                    filecount = filestoReload.Length;
                    
                    fs.Close();
                }
                else
                {
                    Thread.Sleep(2000);
                }
            }

            
        }
        private string FindSiteString(string filename)
        {
            //parse the filename and find the 4 numbers after the first number found 
            
            int outresult=0;
            string strResult = "9999";
            for(int i=0;i<filename.Length;i++)
            {
                if (int.TryParse (filename[i].ToString (),out outresult))
                {
                    strResult = filename.Substring(i, 4);
                    break;
                }

                
            }
            return strResult;
        }
        private bool FileExists(string filename)
        {
            string checkinsert = @"Select oldsiteid from tblsitedataFileInventory where filename='" + filename + "'" ;
            object result = default(object);
            using (OleDbCommand cmd = new OleDbCommand(checkinsert))
            {

                cmd.CommandType = CommandType.Text;
                cmd.Connection = Connect();
                result = cmd.ExecuteScalar();
                cmd.Connection.Close();
                if (result != null)
                {
                    Debug.Print(filename + " already in siteinventory");
                    return true;
                }
                return false;

            }
        }
        private void CatalogueFilesNoData()
        {

            
            DateTime EndDate = Convert.ToDateTime(@"09/09/1909"); 
            string EndTime = "";
            
            string StartTime = "";
            DateTime StartDate = Convert.ToDateTime(@"09/09/1909");
            int cnt = 0;

            string FolderPath = GetFolder();

            string[] fileEntries = Directory.GetFiles(FolderPath);
            foreach (string fileName in fileEntries)
            {

                string flname = Path.GetFileName(fileName).ToString();
                this.InsertSiteInventory(flname.Substring(0, 11), flname, StartDate, StartTime, EndDate, EndTime, cnt);
                
            }

        }
        private void button2_Click(object sender, EventArgs e)
        {
            ExportWorksheets();
        }
        private void button1_Click(object sender, System.EventArgs e)
        {
                      
            
            try
            {

                CatalogueFiles();
            }
            catch (Exception theException)
            {
                String errorMessage;
                errorMessage = "Error: ";
                errorMessage = String.Concat(errorMessage, theException.Message);
                errorMessage = String.Concat(errorMessage, " Line: ");
                errorMessage = String.Concat(errorMessage, theException.Source);

                MessageBox.Show(errorMessage, "Error");
                
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            CatalogueFilesNoData();
        }
        private void button4_Click(object sender, EventArgs e)
        {
            RestoreRawTowerShadowVals();
        }
        private void button5_Click(object sender, EventArgs e)
        {
            StackExcelFiles();
        }
        private void button6_Click(object sender, EventArgs e)
        {
            TestExcelOLEDB();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            MoveFiles();
        }

        private void button8_Click(object sender, EventArgs e)
        {
            ReorderExcelColumns();
            MessageBox.Show("done!");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            ReprocessPipelineFiles(); 
        }

        private void button10_Click(object sender, EventArgs e)
        {
            ReloadPipelineFiles();
        }

        private void button11_Click(object sender, EventArgs e)
        {
            MoveUncataloguedFiles();
        }

        private void button12_Click(object sender, EventArgs e)
        {
            string folder = Utils.GetFolder();
            HeaderAppender appender = new HeaderAppender();
            appender.AddHeaders(folder);
           
        }

        private void button13_Click(object sender, EventArgs e)
        {
            GridUploader gu=new GridUploader ();
           List<GridRow > rows= gu.getUploadList();
           rows.ForEach(c => gu.InsertGridData(c.grid,c.xaxistype,c.yaxistype ,c.filename,c.site, c.config, c.exclusions, c.alphamethod, c.xaxis, c.yaxis, c.value));
        }
    }
}