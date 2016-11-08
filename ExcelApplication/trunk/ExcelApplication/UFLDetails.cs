using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using Microsoft.VisualStudio.Tools.Applications.Runtime;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Core;
using System.IO;

namespace ExcelApplication
{
    public class UFLDetails
    {
        public static readonly int UFL_DETAIL_HEADER_SIZE = 15;

        private System.Object missing = System.Type.Missing;

        private int _UFLDetailId = -1;

        private string _delimiter = string.Empty;
        private string _dateTimeFormat = string.Empty;
        private string _dateFormat = string.Empty;
        private string _timeFormat = string.Empty;
        private string _piTowerName = string.Empty;
        private string _pitagduplicates = string.Empty;

        private int _headerMarkerRow = 0;

        // storing numbers as strings
        // this makes it easier to serialise the list to db
        private List<string> _headerAdditionalDetailRows = new List<string>();

        private int _piTagRow = 0;
        private List<String> _piTagList = new List<string>();
        private List<String> _headerAdditionalDetailRowsContents = new List<string>();
        private List<String> _headerMarkerRowContents = new List<string>();

        private string _fileMask;

        public string FileMask
        {
            get { return _fileMask; }
            set { _fileMask = value; }
        }

        private string _folder;

        public string Folder
        {
            get { return _folder; }
            set
            {
                if (value.EndsWith("\\"))
                {
                    _folder = value;
                }
                else
                {
                    _folder = value + '\\';
                }

            }
        }

        private bool _recursive;

        public bool Recursive
        {
            get { return _recursive; }
            set { _recursive = value; }
        }

        private bool _encrypted;

        public bool Encrypted
        {
            get { return _encrypted; }
            set { _encrypted = value; }
        }

        private string _dataType;

        public string DataType
        {
            get { return _dataType; }
            set { _dataType = value; }
        }

        public int UFLDetailId
        {
            get { return _UFLDetailId; }
            set { _UFLDetailId = value; }
        }

        public string Delimiter
        {
            get { return _delimiter; }
            set { _delimiter = value; }
        }

        public string DelimiterCode
        {
            get
            {
                if (_delimiter == "," || _delimiter == ";")
                {
                    return _delimiter;
                }
                else if (_delimiter == "TAB")
                {
                    return "\t";
                }
                else if (_delimiter == "SPACE")
                {
                    return " ";
                }
                else
                {
                    return "<unknown delimitier>";
                }
            }
        }


        public string DateTimeFormat
        {
            get { return _dateTimeFormat; }
            set { _dateTimeFormat = value; }
        }
        public string DateFormat
        {
            get { return _dateFormat; }
            set { _dateFormat = value; }
        }
        public string TimeFormat
        {
            get { return _timeFormat; }
            set { _timeFormat = value; }
        }
        public string PiTowerName
        {
            get { return _piTowerName; }
            set { _piTowerName = value; }
        }

        // HeaderMarkerRow aka "Header Match" row
        public int HeaderMarkerRow
        {
            get { return _headerMarkerRow; }
            set { _headerMarkerRow = value; }
        }

        // HeaderAdditionalDetail aka "Additional Detail" rows
        public List<string> HeaderAdditionalDetailRows
        {
            get { return _headerAdditionalDetailRows; }
            set { _headerAdditionalDetailRows = value; }
        }
        public int PiTagRow
        {
            get { return _piTagRow; }
            set { _piTagRow = value; }
        }
        public List<String> PiTagList
        {
            get { return _piTagList; }
            set { _piTagList = value; }
        }

        // list of cell contensts for "Header Match"
        public List<String> HeaderMarkerRowContents
        {
            get { return _headerMarkerRowContents; }
            set { _headerMarkerRowContents = value; }
        }

        // list of cell contensts for "Additional Detail"
        public List<String> HeaderAdditionalDetailRowsContents
        {
            get { return _headerAdditionalDetailRowsContents; }
            set { _headerAdditionalDetailRowsContents = value; }
        }



        public void SaveUFL()
        {
            SqlDataReader reader = null;

            SqlParameter uflDetailIdParam = new SqlParameter("@UFLDetailId", SqlDbType.Int);
            uflDetailIdParam.Value = UFLDetailId;

            SqlParameter piTowerNameParam = new SqlParameter("@PiTowerName", SqlDbType.VarChar, 50);
            piTowerNameParam.Value = PiTowerName;

            SqlParameter fileMaskParam = new SqlParameter("@FileMask", SqlDbType.VarChar, 50);
            fileMaskParam.Value = FileMask;

            SqlParameter folderParam = new SqlParameter("@Folder", SqlDbType.VarChar, 255);
            folderParam.Value = Folder;

            SqlParameter recursiveParam = new SqlParameter("@Recursive", SqlDbType.Bit);
            recursiveParam.Value = Recursive;

            SqlParameter encryptedParam = new SqlParameter("@Encrypted", SqlDbType.Bit);
            encryptedParam.Value = Encrypted;

            SqlParameter dataTypeParam = new SqlParameter("@DataType", SqlDbType.Char, 3);
            dataTypeParam.Value = DataType;

            SqlParameter delimiterParam = new SqlParameter("@Delimitor", SqlDbType.VarChar, 10);
            delimiterParam.Value = Delimiter;

            SqlParameter dateTimeFormatParam = new SqlParameter("@DateTimeFormat", SqlDbType.VarChar, 30);
            dateTimeFormatParam.Value = DateTimeFormat;

            SqlParameter dateFormatParam = new SqlParameter("@DateFormat", SqlDbType.VarChar, 20);
            dateFormatParam.Value = DateFormat;

            SqlParameter timeFormatParam = new SqlParameter("@TimeFormat", SqlDbType.VarChar, 20);
            timeFormatParam.Value = TimeFormat;

            SqlParameter headerMarkerRowParam = new SqlParameter("@HeaderMarkerRow", SqlDbType.Int);
            headerMarkerRowParam.Value = HeaderMarkerRow;

            SqlParameter headerAdditionalDetailParam = new SqlParameter("@HeaderAdditionalDetail", SqlDbType.VarChar, 2000);
            headerAdditionalDetailParam.Value = String.Join("|", HeaderAdditionalDetailRows.ToArray());

            SqlParameter piTagRowParam = new SqlParameter("@PiTagRow", SqlDbType.Int);
            piTagRowParam.Value = PiTagRow;

            SqlParameter piTagListParam = new SqlParameter("@PiTagList", SqlDbType.VarChar, 3000);
            piTagListParam.Value = String.Join("|", PiTagList.ToArray());

            db myDb = new db();
            myDb.Connect();

            SqlCommand command = new SqlCommand("SpExcelSaveUFLDetail", myDb.Connection);
            command.CommandType = CommandType.StoredProcedure;




            try
            {
                command.Parameters.Add(uflDetailIdParam);
                command.Parameters.Add(piTowerNameParam);
                command.Parameters.Add(fileMaskParam);
                command.Parameters.Add(folderParam);
                command.Parameters.Add(recursiveParam);
                command.Parameters.Add(encryptedParam);
                command.Parameters.Add(dataTypeParam);
                command.Parameters.Add(delimiterParam);
                command.Parameters.Add(dateTimeFormatParam);
                command.Parameters.Add(dateFormatParam);
                command.Parameters.Add(timeFormatParam);
                command.Parameters.Add(headerMarkerRowParam);
                command.Parameters.Add(headerAdditionalDetailParam);
                command.Parameters.Add(piTagRowParam);
                command.Parameters.Add(piTagListParam);
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                string dummy = e.Message;
            }


            while (reader.Read())
            {
                if (reader["UFLDetailId"].ToString() != "")
                {
                    UFLDetailId = int.Parse(reader["UFLDetailId"].ToString());
                }
            }

            reader.Close();
            myDb.Connection.Close();

        }

        public void LoadUFL()
        {
            db myDb = new db();
            SqlDataReader reader = null;

            SqlParameter uflDetailIdParam = new SqlParameter("@UFLDetailId", SqlDbType.Int);
            uflDetailIdParam.Value = UFLDetailId;

            myDb.Connect();

            SqlCommand command = new SqlCommand("SpExcelLoadUFLDetail", myDb.Connection);
            command.CommandType = CommandType.StoredProcedure;




            try
            {
                command.Parameters.Add(uflDetailIdParam);
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                string dummy = e.Message;
            }


            while (reader.Read())
            {
                if (reader["UFLDetailId"].ToString() != "")
                {
                    UFLDetailId = int.Parse(reader["UFLDetailId"].ToString());
                }

                PiTowerName = reader["PiTowerName"].ToString();
                FileMask = reader["FileMask"].ToString();
                Folder = reader["Folder"].ToString();
                if (reader["Recursive"].ToString() != String.Empty)
                {
                    Recursive = (bool)reader["Recursive"];
                }
                else
                {
                    Recursive = false;
                }
                if (reader["Encrypted"].ToString() != String.Empty)
                {
                    Encrypted = (bool)reader["Encrypted"];
                }
                else
                {
                    Encrypted = false;
                }

                DataType = reader["DataType"].ToString();
                Delimiter = reader["Delimitor"].ToString();
                DateTimeFormat = reader["DateTimeFormat"].ToString();
                DateFormat = reader["DateFormat"].ToString();
                TimeFormat = reader["TimeFormat"].ToString();

                if (reader["HeaderMarkerRow"].ToString() != "")
                {
                    HeaderMarkerRow = int.Parse(reader["HeaderMarkerRow"].ToString());
                }

                HeaderAdditionalDetailRows.Clear();



                HeaderAdditionalDetailRows.AddRange((reader["HeaderAdditionalDetail"].ToString()).Split('|'));

                if (reader["PiTagRow"].ToString() != "")
                {
                    PiTagRow = int.Parse(reader["PiTagRow"].ToString());
                }

                PiTagList.Clear();
                PiTagList.AddRange((reader["PiTagList"].ToString()).Split('|'));
            }

            reader.Close();
            myDb.Connection.Close();

        }


        public void GetHeaderValues(Excel.Worksheet ws)
        {
            _delimiter = (String)((Excel.Range)ws.Cells[2, 3]).Text;
            _dateTimeFormat = (String)((Excel.Range)ws.Cells[3, 3]).Text;
            _dateFormat = (String)((Excel.Range)ws.Cells[4, 3]).Text;
            _timeFormat = (String)((Excel.Range)ws.Cells[5, 3]).Text;
            _piTowerName = (String)((Excel.Range)ws.Cells[7, 3]).Text;
            if (((Excel.Range)ws.Cells[8, 3]).Text.ToString() != "")
            {
                _UFLDetailId = int.Parse(((Excel.Range)ws.Cells[8, 3]).Text.ToString());
            }
            _fileMask = (String)((Excel.Range)ws.Cells[10, 3]).Text;
            _folder = (String)((Excel.Range)ws.Cells[11, 3]).Text;
            _recursive = (((Excel.Range)ws.Cells[12, 3]).Text).ToString().ToUpper() == "YES";
            _encrypted = (((Excel.Range)ws.Cells[13, 3]).Text).ToString().ToUpper() == "YES";
            _dataType = (String)((Excel.Range)ws.Cells[14, 3]).Text;
        }

        public void SetHeaderValues(ref Excel.Worksheet ws)
        {
            ws.Cells[2, 3] = _delimiter;
            ws.Cells[3, 3] = _dateTimeFormat;
            ws.Cells[4, 3] = _dateFormat;
            ws.Cells[5, 3] = _timeFormat;
            ws.Cells[7, 3] = _piTowerName;
            ws.Cells[8, 3] = _UFLDetailId;
            ws.Cells[10, 3] = _fileMask;
            ws.Cells[11, 3] = _folder;
            ws.Cells[12, 3] = _recursive ? "Yes" : "No";
            ws.Cells[13, 3] = _encrypted ? "Yes" : "No";
            ws.Cells[14, 3] = _dataType;
        }



        public void GetHeaderRowContents(Excel.Worksheet ws)
        {
            int col = 2;
            int blankCount = 0;
            Utils u = new Utils();

            Excel.Range cell = null;
            _headerMarkerRowContents.Clear();

            while (_headerMarkerRow > 0 && blankCount < 10)
            {
                cell = (Excel.Range)ws.Cells[_headerMarkerRow, col++];
                if ((String)cell.Text != "")
                {
                    _headerMarkerRowContents.Add((String)cell.Text);
                    blankCount = 0;
                }
                else
                {
                    _headerMarkerRowContents.Add("");
                    blankCount++;
                }
            }

            if (blankCount == 10)
            {
                // remove the final 10 blanks
                for (int i = 0; i < 10; i++)
                {
                    _headerMarkerRowContents.RemoveAt(_headerMarkerRowContents.Count - 1);
                }
            }
        }

        public void GetHeaderAdditionalDetailsRowsContents(Excel.Worksheet ws)
        {
            int col = 2;
            int blankCount = 0;
            Utils u = new Utils();
            List<String> buildRow = new List<string>();

            Excel.Range cell = null;
            _headerAdditionalDetailRowsContents.Clear();

            foreach (string row in _headerAdditionalDetailRows)
            {
                buildRow.Clear();
                blankCount = 0;
                col = 2;

                while (int.Parse(row) > 0 && blankCount < 10)
                {
                    cell = (Excel.Range)ws.Cells[row, col++];
                    if ((String)cell.Text != "")
                    {
                        buildRow.Add((String)cell.Text);
                        blankCount = 0;
                    }
                    else
                    {
                        buildRow.Add("");
                        blankCount++;
                    }
                }

                if (blankCount == 10)
                {
                    // remove the final 10 blanks
                    for (int i = 0; i < 10; i++)
                    {
                        buildRow.RemoveAt(buildRow.Count - 1);
                    }
                }

                _headerAdditionalDetailRowsContents.Add(string.Join(DelimiterCode, buildRow.ToArray()));

            }

        }

        public void GetPiTags(Excel.Worksheet ws)
        {
            int col = 2;
            int blankCount = 0;
            Utils u = new Utils();
            Error error = new Error();

            Excel.Range cell = null;
            _piTagList.Clear();

            _piTagRow = u.FindStringInSpreadsheet(ws, "PITags");

            while (_piTagRow > 0 && blankCount < 3)
            {
                cell = (Excel.Range)ws.Cells[_piTagRow, col++];
                if ((String)cell.Text != "")
                {
                    if (_piTagList.Contains(cell.Text.ToString()) && cell.Text.ToString() != "N/A")
                    {
                        _pitagduplicates = _pitagduplicates + System.Environment.NewLine + cell.Text.ToString();

                    }
                    _piTagList.Add((String)cell.Text);
                    blankCount = 0;
                }
                else
                {
                    blankCount++;
                }
            }

            if (_pitagduplicates != string.Empty)
            {
                error.ErrorMsg = " Mapped to more than one column: " + System.Environment.NewLine + _pitagduplicates;
                error.ShowDialog();
            }
        }

        public void SetPiTags(ref Excel.Worksheet ws)
        {
            int col = 2;
            ws.Cells[_piTagRow, 1] = "PITags";

            foreach (string tag in _piTagList)
            {
                ws.Cells[_piTagRow, col++] = tag;
            }
        }

        public void UpdateFileFormatDetails()
        {
            int affected = 0;
            db myDb = new db();

            SqlParameter fileFormatIdParam = new SqlParameter("@FileFormatId", SqlDbType.Int);
            fileFormatIdParam.Value = UFLDetailId; // Use the same id for FileFormatId as the UFLDetailId

            SqlParameter delimiterParam = new SqlParameter("@Delimitor", SqlDbType.VarChar, 5);
            delimiterParam.Value = Delimiter;

            SqlParameter headerRowNumberParam = new SqlParameter("@HeaderRowNumber", SqlDbType.Int);
            headerRowNumberParam.Value = HeaderMarkerRow - UFL_DETAIL_HEADER_SIZE; // We use 15 extra lines for our UFL Details header

            SqlParameter headerRowContentsParam = new SqlParameter("@HeaderRowContents", SqlDbType.VarChar, 3000);
            headerRowContentsParam.Value = String.Join(DelimiterCode, HeaderMarkerRowContents.ToArray());

            SqlParameter piTowerNameParam = new SqlParameter("@PiTowerName", SqlDbType.VarChar, 50);
            piTowerNameParam.Value = PiTowerName;

            SqlParameter folderParam = new SqlParameter("@Folder", SqlDbType.VarChar, 1000);
            folderParam.Value = Folder;

            SqlParameter fileMaskParam = new SqlParameter("@FileMask", SqlDbType.VarChar, 100);
            fileMaskParam.Value = FileMask;

            SqlParameter recurseParam = new SqlParameter("@Recurse", SqlDbType.Bit);
            recurseParam.Value = Recursive;

            SqlParameter encryptedParam = new SqlParameter("@Encrypted", SqlDbType.Bit);
            encryptedParam.Value = Encrypted;

            myDb.Connect();

            SqlCommand command = new SqlCommand("SpExcelUpdateFileFormatDetails", myDb.Connection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(fileFormatIdParam);

            command.Parameters.Add(delimiterParam);

            command.Parameters.Add(headerRowNumberParam);

            command.Parameters.Add(headerRowContentsParam);

            command.Parameters.Add(piTowerNameParam);

            command.Parameters.Add(folderParam);

            command.Parameters.Add(fileMaskParam);

            command.Parameters.Add(recurseParam);

            command.Parameters.Add(encryptedParam);



            try
            {
                affected = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string dummy = e.Message;

                Error frmError = new Error();

                frmError.ErrorMsg = String.Format("An error occoured: {0}", e.Message);

                frmError.ShowDialog();
            }


            // now store additional details content
            SqlParameter fileFormatIdMLParam = new SqlParameter("@FileFormatId", SqlDbType.Int);
            SqlParameter headerAdditioanlDetailRowNumberParam = new SqlParameter("@HeaderRowNumber", SqlDbType.Int);
            SqlParameter headerAdditioanlDetailRowContentsParam = new SqlParameter("@HeaderRowContents", SqlDbType.VarChar, 1000);
            SqlParameter deleteExistingParam = new SqlParameter("@DeleteExisting", SqlDbType.Bit);


            SqlCommand commandML = new SqlCommand("SpExcelUpdateMultiLineHeaderRow", myDb.Connection);
            commandML.CommandType = CommandType.StoredProcedure;

            fileFormatIdMLParam.Value = UFLDetailId;

            commandML.Parameters.Add(fileFormatIdMLParam);
            commandML.Parameters.Add(headerAdditioanlDetailRowNumberParam);
            commandML.Parameters.Add(headerAdditioanlDetailRowContentsParam);
            commandML.Parameters.Add(deleteExistingParam);


            for (int i = 0; i < HeaderAdditionalDetailRows.Count; i++)
            {
                // insert 
                headerAdditioanlDetailRowNumberParam.Value = int.Parse(HeaderAdditionalDetailRows[i]) - UFL_DETAIL_HEADER_SIZE; // We use 15 extra lines for our UFL Details header
                headerAdditioanlDetailRowContentsParam.Value = HeaderAdditionalDetailRowsContents[i];
                deleteExistingParam.Value = (i == 0); // Only delete existing records when we insert the first line

                try
                {
                    affected = commandML.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    string dummy = e.Message;

                    Error frmError = new Error();

                    frmError.ErrorMsg = String.Format("An error occoured: {0}", e.Message);

                    frmError.ShowDialog();
                }

            }

        }

        public string CheckMappedSensors(int formatid, DateTime filedate)
        {



            db myDb = new db();



            SqlParameter fileFormatIdParam = new SqlParameter("@FileFormatId", SqlDbType.Int);
            fileFormatIdParam.Value = formatid;

            SqlParameter dateParam = new SqlParameter("@MinFileDate", SqlDbType.DateTime);
            dateParam.Value = filedate;

            SqlParameter retmsg = new SqlParameter("@retmessage", SqlDbType.NVarChar, 300);
            retmsg.Direction = ParameterDirection.Output;

            myDb.ConnectEkho();

            SqlCommand command = new SqlCommand("WA_CheckMappedSensors", myDb.EkhoConnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(fileFormatIdParam);
            command.Parameters.Add(dateParam);
            command.Parameters.Add(retmsg);


            try
            {
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                string dummy = e.Message;

                Error frmError = new Error();

                frmError.ErrorMsg = String.Format("An error in checkmappedsensors occoured: {0}", e.Message);

                frmError.ShowDialog();
                return " ";
            }

            return retmsg.Value.ToString().Replace(@"\n", Environment.NewLine);
        }
    }
}
