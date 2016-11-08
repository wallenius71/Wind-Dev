using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.OleDb;
using System.Data;
using System.Windows.Forms;

namespace WindowsApplication1
{
    public class GridUploader
    {
        string site;
        string config;
        string exclusions;
        string alphamethod;
        string xaxistype;
        string yaxistype;
        string xaxisval;
        string yaxisval;
        string gridvalue;
        string originalname;
        string grid;





        //allows user to upload count, average and alpha grids to windart_smi for comparison with ekho data
        // ,1,2,3,4,5
        //1,a,a,a,a,a
        //2,a,a,a,a,a    -----> site, config, exclusion, alpha-method,xaxis, yaxis, value 
        //3,a,a,a,a,a
        //4,a,a,a,a,a
        //5,a,a,a,a,a

        public List<GridRow> getUploadList()
        {
            List<GridRow> results = new List<GridRow>();
            string folder = Utils.GetFolder();

            //return list of data for upload
            foreach (string s in Directory.EnumerateFiles(folder, "*.csv"))
            {
                site = "AZ00002";
                config = s.Substring(s.IndexOf("CF"), 3);
                originalname  = Path.GetFileNameWithoutExtension(s);
                exclusions = s.Substring(s.IndexOf("CDNT") + 5, 3).Trim().Replace("\\\\","");
                alphamethod = s.Substring(s.IndexOf("CDNT") -1, 5).TrimStart();
                grid = s.Substring(s.IndexOf("Grid"), 6);
                string[] thisaxistype=originalname.Split('_');
                xaxistype = thisaxistype[thisaxistype.Length - 3].Split('x')[0];
                yaxistype = thisaxistype[thisaxistype.Length - 3].Split('x')[1];
                StreamReader readerCSV = new StreamReader(s);

                string filename = Path.GetFileName(s);


                List<List<string>> griddata = new List<List<string>>();

                while (!readerCSV.EndOfStream)
                {
                    griddata.Add(readerCSV.ReadLine().Split(',').ToList());
                }
                readerCSV.Close();


                foreach (List<string> ls in griddata)
                {
                    int i = 0;
                    
                    foreach (string value in ls)
                    {
                        
                        
                        int y = griddata.IndexOf(ls);
                        if (y == 0) break;
                        //if we are not on a column head or row head
                        if (i != 0 & i<=21)
                        {
                            xaxisval = griddata[0][i];
                            yaxisval = griddata[y][0];
                            gridvalue = value;

                            GridRow newrow = new GridRow()
                            {
                                site = this.site,
                                config = this.config,
                                exclusions = this.exclusions,
                                alphamethod = this.alphamethod,
                                xaxis = xaxisval,
                                yaxis = yaxisval,
                                value = gridvalue,
                                grid = this.grid,
                                xaxistype = xaxistype,
                                yaxistype = yaxistype,
                                filename = originalname
                            };
                            if (!results.Contains (newrow))
                            results.Add(newrow);
                            
                            

                        }
                        i++;
                    }
                }


            }
            return results;
        }



       public void InsertGridData(string grid, string xaxistype, string yaxistype, string filename, string site, string config, string exclusions, string alphamethod, string xaxis, string yaxis, string value)
        {
            //check to see if record is there first 
            try
            {

                string insert = @"insert into Ekho_ShearGridTestData (grid, xaxistype, yaxistype, filename, site,config,exclusions,alphamethod,x,y,value) values('" + grid + "','" + xaxistype + "','" + yaxistype + "','" + filename + "','" + site + "','" + config + "','" + exclusions + "','" + alphamethod + "', '" + xaxis + "','" + yaxis + "'," + value + ")";
                Console.WriteLine(xaxis + ", " + yaxis + " , " + value + ", " + xaxistype + ", " + yaxistype + ", " + grid);
                using (OleDbCommand cmd = new OleDbCommand(insert))
                {


                    cmd.CommandType = CommandType.Text;
                    cmd.Connection = Utils.Connect();
                    cmd.ExecuteNonQuery();
                    cmd.Connection.Close();


                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }
    }
}
