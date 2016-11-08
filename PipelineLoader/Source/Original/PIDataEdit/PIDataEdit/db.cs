using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Collections;
using System.Windows.Forms;

namespace PIDataEdit
{

    public class db
    {
        private static bool debugMode = false;


        private SqlConnection _sqlConnection = null;
        private OleDbConnection _oleConnection = null;

        public OleDbConnection OLEConnection
        {
            get { return _oleConnection; }
            set { _oleConnection = value; }
        }


        public bool OLEConnect()
        {
            string connectionString = @"Provider = PIOLEDB; Data Source = 194.247.115.6 ; User ID=PIAdmin; Password=Hannah123; Command Timeout=0; Session Pipelines = 4; Time Zone = Server; Defer Execution = True;";
            //string connectionString = @"Provider = PIOLEDB; Data Source = 10.128.10.7 ; User ID=PIAdmin; Password=Hannah123; Log File = c:\temp\PIDataEdit.log";

            try
            {
                _oleConnection = new OleDbConnection(connectionString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                return false;
            }

            try
            {
                _oleConnection.Open();
            }
            catch (Exception e)
            {
                MessageBox.Show("Cannot connect to the database server.\n\n" + e.Message);
                return false;
            }

            return true;
        }

        public void SQLConnect()
        {
            string connectionString = @"Data Source = 10.128.10.8; Initial Catalog = WindART_SMI; Integrated Security = SSPI; Connect Timeout=600";

            try
            {
                _sqlConnection = new SqlConnection(connectionString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            try
            {
                _sqlConnection.Open();
            }
            catch (Exception e)
            {
                string dummy = e.Message;
                MessageBox.Show("Cannot connect to the database server.\n\nPlease check you are connected to the Fujitsu VPN.\n\n" + e.Message);

            }
        }


        public bool verifyCanConnectToDb()
        {
            try
            {
                if (!OLEConnect())
                {
                    return false;
                }

                if (_oleConnection.State.ToString() == "Closed")
                {
                    return false;
                }
                else
                {
                    _oleConnection.Close();
                }
            }
            catch
            {
                return false;
            }

            return true;
        }


        public bool updateData(string tag, string dateAndTime, string val, bool keepConnection)
        {
            int res = 0;

            if (!keepConnection)
            {
                if (!OLEConnect())
                {
                    return false;
                };
            }


            string cmd = string.Empty;

            if(val.ToLower()!="delete")
            {
                cmd = String.Format("update piarchive..picomp2 set value = {0} where tag = '{1}' and time = '{2}'", val, tag, dateAndTime);
            } else {
                cmd = String.Format("delete from piserver.piarchive..picomp2 where tag = '{0}' and time = '{1}'", tag, dateAndTime);
            }

            //SqlCommand command = new SqlCommand(cmd, _connection);
            OleDbCommand command = new OleDbCommand(cmd, _oleConnection);
            
            try
            {
                res = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }


            if (!keepConnection)
            {
                _oleConnection.Close();
            }

            return true;
        }

        public string CreateSQL(string tag, string dateAndTime, string val)
        {
            string cmd = string.Empty;

            if (val.ToLower() != "delete")
            {
                cmd = String.Format("update piarchive..picomp2 set value = {0} where tag = '{1}' and time = '{2}';", val, tag, dateAndTime);
            }
            else
            {
                cmd = String.Format("delete from piserver.piarchive..picomp2 where tag = '{0}' and time = '{1}';", tag, dateAndTime);
            }

            return cmd;
        }


        public object[] getSites()
        {
            String tower = String.Empty;
            Object tag = null;

            List<String> metTowers = new List<String>();

            if (!OLEConnect())
            {
                return null;
            }


            if (_oleConnection.State.ToString() == "Closed")
            {
                return null;
            }

            OleDbCommand command = new OleDbCommand("SELECT DISTINCT tag FROM [pipoint]..[classic] where tag LIKE ('BP%DCL%')", _oleConnection);
         
            //SqlDataReader reader = command.ExecuteReader();
            OleDbDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                tag = reader["tag"];


                // Look out for nasty nulls
                if (tower.GetType().ToString() != "System.DBNull")
                {
                    try
                    {
                        // get tower part of tag
                        if (tag.ToString().IndexOf(".DCL")>0)
                        {
                            tower = tag.ToString().Substring(0, tag.ToString().IndexOf(".DCL"));
                        }

                        if (!metTowers.Contains(tower))
                        {
                            metTowers.Add(tower);
                        }
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message);
                    }
                }
            }

            reader.Close();
            _oleConnection.Close();
            _oleConnection.Dispose();

            return metTowers.ToArray();
        }

        public List<PITagData> GetDataForSite(string site, DateTime dtFrom, DateTime dtTo, bool keepConnectionAlive, bool showUI)
        {
            //SQLConnect();
            if (!OLEConnect())
            {
                return null;
            }

            Object tag = null;
            Object time = null;
            Object val = null;

            DateTime startTime = DateTime.Now;

            List<PITagData> metData = new List<PITagData>();

            // If you want to use SQL Server
            // SqlCommand command = new SqlCommand();
            // SqlDataReader reader = null;

           OleDbCommand command = new OleDbCommand();
            OleDbDataReader reader = null;


            if (_oleConnection.State.ToString() == "Closed")
            {
                return null;
            }

           foreach (string tagType in new string[] { "DCL", "QCW" })
           {
               // Grab the data between the datetime stamps
               command = new OleDbCommand(String.Format("SELECT tag, [time], [value] FROM piarchive..picomp2 WHERE tag like('{0}%{1}%') AND [time] BETWEEN '{2}-{3}-{4} {5}' AND '{6}-{7}-{8} {9}' ORDER BY tag, [time]", site, tagType, dtFrom.Year, dtFrom.Month, dtFrom.Day, dtFrom.ToLongTimeString(), dtTo.Year, dtTo.Month, dtTo.Day, dtTo.ToLongTimeString()), _oleConnection);

               if (debugMode)
               {
                   Debug frmDebug = new Debug();
                   frmDebug.Input = command.CommandText;
                   frmDebug.Show();
               }

               try
               {
                   reader = command.ExecuteReader();
               }
               catch (Exception e)
               {
                   MessageBox.Show(e.Message);

                   //reader.Close();
                   _oleConnection.Close();

                   return metData;
               }

               while (reader.Read())
               {
                   tag = reader["tag"];
                   time = reader["time"];
                   val = reader["value"];

                   // Look out for nasty nulls

                   if (tag.GetType().ToString() == "System.DBNull")
                   {
                       tag = "";
                   }

                   if (time.GetType().ToString() == "System.DBNull")
                   {
                       time = "";
                   }

                   if (val.GetType().ToString() == "System.DBNull")
                   {
                       val = "";
                   }



                   try
                   {
                       metData.Add(new PITagData(tag.ToString().Replace(site + ".", ""), (DateTime)time, val.ToString()));
                   }
                   catch (Exception e)
                   {
                       MessageBox.Show(e.Message);
                   }

               }

               reader.Close();
           }

           if (!keepConnectionAlive)
           {
               _oleConnection.Close();
           }

           if (showUI)
           {
               TimeSpan duration = DateTime.Now - startTime;
               MessageBox.Show(string.Format("PI data download: {0} minutes {1} seconds", duration.Minutes, duration.Seconds));
           }

           return metData;
        }

        public bool updateDataInBlock(StringBuilder megaSQL)
        {
            int res = 0;

            if (!OLEConnect())
            {
                return false;
            };

            OleDbCommand command = new OleDbCommand(megaSQL.ToString(), _oleConnection);
            
            try
            {
                res = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }

            _oleConnection.Close();

            return true;
        }

        internal void SetFlushOff()
        {
            int res = 0;

            OleDbCommand command = new OleDbCommand("SET FLUSH OFF", _oleConnection);

            try
            {
                res = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        internal void SetFlushOn()
        {
            int res = 0;

            OleDbCommand command = new OleDbCommand("FLUSH", _oleConnection);

            try
            {
                res = command.ExecuteNonQuery();

                command = new OleDbCommand("SET FLUSH ON", _oleConnection);
                res = command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
