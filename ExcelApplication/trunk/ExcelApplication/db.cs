using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;


namespace ExcelApplication
{
    public class db
    {
        private SqlConnection _connection = null;
        private SqlConnection _ekhoconnection = null;

        public SqlConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }

        public SqlConnection EkhoConnection
        {
            get { return _ekhoconnection; }
            set { _ekhoconnection = value; }
        }
        public void Connect()
        {
            string connectionString = @"Data Source = 10.254.13.3; Initial Catalog = Windart_SMI; user=ekho; password=ekho; Integrated Security=false";

            try
            {
                _connection = new SqlConnection(connectionString);
            }
            catch (Exception e)
            {
                string dummy = e.Message;
            }

            try
            {
                _connection.Open();
            }
            catch (Exception e)
            {
                string dummy = e.Message;
                Error frmError = new Error();

                frmError.ErrorMsg = "Cannot connect to the database server.\n\nPlease check you are connected to the Telvent VPN.";

                frmError.ShowDialog();
            }
        }

        public void ConnectEkho()
        {
            string connectionString = @"Data Source = 10.254.13.3; Initial Catalog =EkhoWind; User=ekho; password=ekho; Integrated Security = false";

            try
            {
                _ekhoconnection = new SqlConnection(connectionString);
            }
            catch (Exception e)
            {
                string dummy = e.Message;
            }

            try
            {
                _ekhoconnection.Open();
            }
            catch (Exception e)
            {
                string dummy = e.Message;
                Error frmError = new Error();

                frmError.ErrorMsg = "Cannot connect to the database server.\n\nPlease check you are connected to the Telvent VPN.";

                frmError.ShowDialog();
            }
        }

        public bool verifyCanConnectToDb()
        {
            try
            {
                Connect();
                if (_connection.State.ToString() == "Closed")
                {
                    return false;
                }
                else
                {
                    _connection.Close();
                }

                ConnectEkho();
                if (_ekhoconnection.State.ToString() == "Closed")
                {
                    return false;
                }
                else
                {
                    _ekhoconnection.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        public void getSites(List<string> metTowers)
        {
            ConnectEkho();

            Object site = new object();
            SqlCommand command = new SqlCommand("[WA_EXCEL_SpExcelGetSite]", _ekhoconnection);
            command.CommandType = CommandType.StoredProcedure;
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                site = reader["WA_Site_BPID"];

                // Look out for nasty nulls
                if (site.GetType().ToString() != "System.DBNull")
                {
                    try
                    {

                        metTowers.Add((string)site);
                    }
                    catch (Exception e)
                    {
                        string dummy = e.Message;
                    }
                }
            }

            reader.Close();
            _ekhoconnection.Close();
        }

        public void getLocations(List<string> locations)
        {
            Connect();

            Object folder = null;
            SqlCommand command = new SqlCommand("SELECT Distinct Folder from FileLocation", _connection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                folder = reader["Folder"];

                // Look out for nasty nulls
                if (folder.GetType().ToString() != "System.DBNull")
                {
                    try
                    {

                        locations.Add((string)folder);
                    }
                    catch (Exception e)
                    {
                        string dummy = e.Message;
                    }
                }
            }

            reader.Close();
            _connection.Close();
        }


        public void getSitePITags(string PiBPId, List<string> sitePITags)
        {
            SqlDataReader reader = null;
            Object tag = new object();
            SqlParameter siteParam = new SqlParameter("@SitePiBPId", SqlDbType.VarChar, 50);
            siteParam.Value = PiBPId;

            ConnectEkho();

            SqlCommand command = new SqlCommand("[WA_Excel_SpExcelGetPiTagsForSite]", _ekhoconnection);
            command.CommandType = CommandType.StoredProcedure;

            command.Parameters.Add(siteParam);



            try
            {
                reader = command.ExecuteReader();
            }
            catch (Exception e)
            {
                string dummy = e.Message;
            }


            while (reader.Read())
            {
                tag = reader["TagName"].ToString();

                // Look out for nasty nulls
                if (tag.GetType().ToString() != "System.DBNull")
                {
                    try
                    {
                        // Don't store the full string, remove the header
                        string strTag = (string)tag;
                        sitePITags.Add(strTag);
                    }
                    catch (Exception e)
                    {
                        string dummy = e.Message;
                    }
                }
            }

            reader.Close();
            _ekhoconnection.Close();

        }


        public List<AddValue> loadSavedMetTower()
        {
            List<AddValue> metTowers = new List<AddValue>();

            Connect();

            Object tower = null;
            Object id = null;
            Object folder = null;
            Object oldId = null;

            if (_connection.State.ToString() == "Closed")
            {
                return null;
            }

            SqlCommand command = new SqlCommand("SELECT * FROM vwUFLDetailLoad ORDER BY PiTowerName", _connection);

            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tower = reader["PiTowerName"];
                id = reader["UFLDetailId"];
                folder = reader["Folder"];
                oldId = reader["OldId"];

                // Look out for nasty nulls
                if ((String)folder == string.Empty)
                {
                    folder = "unknown folder";
                }

                if ((String)tower == string.Empty)
                {
                    tower = "unknown tower";
                }

                if (oldId.GetType().ToString() == "System.DBNull")
                {
                    oldId = "";
                }


                try
                {
                    metTowers.Add(new AddValue(String.Format("{0} [{1}] Id:{2} : {3}", (string)tower, (string)oldId, (int)id, (string)folder), (int)id));
                }
                catch (Exception e)
                {
                    string dummy = e.Message;
                }
            }

            reader.Close();
            _connection.Close();

            return metTowers;
        }


    }
}
