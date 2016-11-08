using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using WindART.DAL;
using System.IO;

namespace OpsDataDownloader
{
    public class ProgramConsole
    {
        DataTable _data = new DataTable();
        DateTime start = DateTime.Today.AddDays(-40);
        DateTime end = DateTime.Now;
        string filename = @"\\10.92.83.205\public\JesseRichards\Operations\Operations Raw Data\Download Application Output\MET";


        public ProgramConsole(int days)
        {
            start = end.AddDays(-1 * days);

        }

        public ProgramConsole()
        { }

        public void getData(string startParam, string endParam)
        {
            start = DateTime.Parse(startParam);
            end =   DateTime.Parse(endParam);

            DataRepository rep = new DataRepository(DataSourceType.WindART_ekho);

            string sql = @"select eg.entity_id,
						(select replace(p.WA_Project_Name ,' ','_') + '_' + replace(wa_site_oldid,' ','_') 
                        from wa_site s
                        join EntityGenealogy eg  on eg.Entity_ID =s.Entity_ID
                        join WA_PROJECT p on p.Entity_ID =eg.ParentEntity_ID 
                        where s.entity_id=configs.ParentEntity_ID
                        ) [site]
                        from EntityValue ev join EntityGenealogy eg on eg.Entity_ID =ev.ID
                        join 
						
						 (select b.ParentEntity_ID , MAX(ev.starttime) start  from EntityValue ev
							join 
							(select eg.ParentEntity_ID, eg.entity_id from entitygenealogy eg join 
							(select entity_id  from wa_site where wa_site_purpose='Permanent Met') a 
							 on a.Entity_ID =eg.ParentEntity_ID) b 
						 on ev.id= b.Entity_ID where Name='Config' group by ParentEntity_ID ) configs
						on configs.ParentEntity_ID =eg.ParentEntity_ID and ev.StartTime =configs.start
                        and ev.Name='Config'";

            DataTable siteToDownload = rep.GetData(sql);
            int id = default(int);
            string name = string.Empty;
            string loopSQL = string.Empty;
            string thisfilename = string.Empty;
            foreach (DataRow row in siteToDownload.Rows)
            {
                id = int.Parse(row[0].ToString());
                name = row[1].ToString();
                loopSQL = @"exec windog_ReadAllDataColumns_with_TS_Exclusion " + id + ",'" + start + "','" + end + "', 370";
                DataTable siteData = rep.GetData(loopSQL);
                thisfilename = filename + @"\" + name + ".csv";
                OutputFile(siteData, thisfilename);
                Console.WriteLine("Outputting " + name + "    " + id + "   " + start + " " + end );


            }
        }
        public void getData()
        {
            DataRepository rep = new DataRepository(DataSourceType.WindART_ekho );

            string sql = @"select eg.entity_id,
						(select replace(p.WA_Project_Name ,' ','_') + '_' + replace(wa_site_oldid,' ','_') 
                        from wa_site s
                        join EntityGenealogy eg  on eg.Entity_ID =s.Entity_ID
                        join WA_PROJECT p on p.Entity_ID =eg.ParentEntity_ID 
                        where s.entity_id=configs.ParentEntity_ID
                        ) [site]
                        from EntityValue ev join EntityGenealogy eg on eg.Entity_ID =ev.ID
                        join 
						
						 (select b.ParentEntity_ID , MAX(ev.starttime) start  from EntityValue ev
							join 
							(select eg.ParentEntity_ID, eg.entity_id from entitygenealogy eg join 
							(select entity_id  from wa_site where wa_site_purpose='Permanent Met') a 
							 on a.Entity_ID =eg.ParentEntity_ID) b 
						 on ev.id= b.Entity_ID where Name='Config' group by ParentEntity_ID ) configs
						on configs.ParentEntity_ID =eg.ParentEntity_ID and ev.StartTime =configs.start
                        and ev.Name='Config'";

            DataTable siteToDownload = rep.GetData(sql);
            int id = default(int);
            string name = string.Empty;
            string loopSQL=string.Empty;
            string thisfilename=string.Empty;
            foreach (DataRow row  in siteToDownload.Rows)
            {
                id=int.Parse(row[0].ToString ());
                name = row[1].ToString();
                loopSQL=@"exec windog_ReadAllDataColumns_with_TS_Exclusion " + id + ",'"  + start +"','" + end + "', 370";
                DataTable siteData = rep.GetData(loopSQL);
                thisfilename = filename + @"\" + name + ".csv";
                OutputFile(siteData, thisfilename);
                Console.WriteLine("Outputting " + name);


            }
        }

        public static void OutputFile(DataTable data,string filename)
        {
            
            
                
                   // filename = filename + @"\TestOutput_" +   DateTime.Now.ToLongTimeString ().Replace(@"/", "") + ".csv";
                    
                    // Create the CSV file to which grid data will be exported.
                    StreamWriter sw = new StreamWriter(filename, false);
                    List<int> exclude = new List<int>();
                    //column heads
                    try
                    {
                        foreach (DataColumn col in data.Columns)
                        {
                            double avg = 0.0;
                            if (col.ColumnName != "datetime") avg = data.AsEnumerable().Average(c => Convert.ToDouble(c[col.Ordinal]));

                            if (avg < -8888 )
                            {
                                exclude.Add(col.Ordinal);
                                continue;
                            }
                            else
                            {
                                sw.Write(col.ColumnName.Replace("#", "_"));
                                // Console.WriteLine(col.ColumnName.Replace("#", "_"));
                                if (data.Columns.IndexOf(col) < data.Columns.Count - 1)

                                    sw.Write(",");
                            }
                        }
                    }
                 catch(InvalidOperationException e)
                    {}




                    sw.Write(sw.NewLine);


                    //Now write all the rows.
                    lock (data)
                    {
                        DataView view = data.AsDataView();
                        //view.Sort = "DateTime";

                        foreach (DataRowView dr in view)
                        {
                            for (int i = 0; i < data.Columns.Count; i++)
                            {
                                if (exclude.Contains(i)) continue;
                                if (!Convert.IsDBNull(dr[data.Columns[i].Ordinal]) & dr[data.Columns[i].Ordinal].ToString().Length > 0)
                                {
                                    sw.Write(dr[data.Columns[i].Ordinal].ToString());
                                }
                                else
                                {
                                    sw.Write("-9999.99");
                                }
                                if (i < data.Columns.Count - 1)
                                {
                                    sw.Write(",");
                                }
                            }
                            sw.Write(sw.NewLine);
                        }
                    }
                    sw.Close();

               
            
        }

           
        }
    }


