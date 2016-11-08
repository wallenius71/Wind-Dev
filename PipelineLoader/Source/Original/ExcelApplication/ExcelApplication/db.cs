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

        public SqlConnection Connection
        {
            get { return _connection; }
            set { _connection = value; }
        }
	
        
        public void Connect()
        {
            string connectionString = @"Data Source = 10.128.10.8; Initial Catalog = WindART_SMI; Integrated Security = SSPI";

            try
            {
                _connection = new SqlConnection(connectionString);
            }
            catch(Exception e)
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

                frmError.ErrorMsg = "Cannot connect to the database server.\n\nPlease check you are connected to the Fujitsu VPN.";

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
            }
            catch
            {
                return false;
            }

            return true;
        }


        public void getSites(List<string> metTowers)
        {
            Connect();

            Object site = null;
            SqlCommand command = new SqlCommand("SELECT PiBPID from vwSiteWithPISiteId ORDER BY PiBPID", _connection);
         
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                site = reader["PiBPID"];

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
            _connection.Close();
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
            Object tag = null;
            SqlParameter siteParam = new SqlParameter("@SitePiBPId", SqlDbType.VarChar, 50);
            siteParam.Value = PiBPId;

            Connect();

            SqlCommand command = new SqlCommand("SpExcelGetPiTagsForSite", _connection);
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
                tag = reader["PITag"];

                // Look out for nasty nulls
                if (tag.GetType().ToString() != "System.DBNull")
                {
                    try
                    {
                        // Don't store the full string, remove the header
                        string strTag = (string)tag;
                        sitePITags.Add(strTag.Substring(strTag.IndexOf(".DCL.")+5));
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
