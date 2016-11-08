using System.Collections.Generic;
using NUnit.Framework;
using WindART.DAL;
using WindART;
using System.Data;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Linq.Expressions;
using System.Linq.Dynamic;
using System.Reflection;
using TurbineDataUtility.Model;




namespace MultiConfigTool_UI
{
    [TestFixture ]
   public class TestUI
    {
        BaseDataSourceView bdsv;
        List<SingleConfigViewModel> multiconfigs = new List<SingleConfigViewModel>();
        
        List<SessionColumnCollection> collections;
        AllConfigsViewModel allconfigs;
        DateTime testDate;
        List<DataRow> coincidentlist = new List<DataRow>();
        public static Dictionary<string, DataTable> shearTestGrids = new Dictionary<string, DataTable>();
        public List<DataTable> MultiConfigData = new List<DataTable>();

        [Test]
        public  void getFileData()
        {

            MainWindowViewModel mwvm = new MainWindowViewModel();
            
            
            mwvm.Filename = @"C:\Users\Oakland\Desktop\LocalWorkFolder\LocalP\REAL_DATA\MultiConfig\White Hills\0302Config1.csv";
            mwvm.LoadFileCommand.Execute(null);
            int i=0;
            SingleConfigViewModel scvm = mwvm.AllConfigsVM.Configs[0];
            while (scvm.DownloadedData .Count==0)
            {
                Thread.Sleep(1000);
                Console.WriteLine(i++);
            }
            Thread.Sleep(5000);
            scvm.PersistFilename = @"C:\Users\Oakland\Desktop\LocalWorkFolder\LocalP\REAL_DATA\MultiConfig\White Hills\0302Config1.txt";
            scvm.LoadPersistedConfigCommand.Execute(null);
            testDate = scvm.ColumnCollection.DataSetStart;
            Console.WriteLine("File data contains " + scvm.DownloadedData[0].Rows.Count + " rows " 
                + " datestart " + scvm.ColumnCollection .DataSetStart );
            collections = new List<SessionColumnCollection>(){scvm.ColumnCollection };
            //scvm.ColumnCollection.DataSetStart = DateTime.Parse("5/12/2004 14:30");
           
            multiconfigs .Add( scvm);

        }

        [Test]
        public void getMultiConfigsData()
        {
            string site = "0302";
            //number of data files 
            int count =1;
            for (int i = 1; i <=count; i++)
            {

                MainWindowViewModel mwvm = new MainWindowViewModel();

                Assert.IsInstanceOf(typeof(MainWindowViewModel ),mwvm);

                mwvm.Filename = @"\\BP1\public\JesseRichards\REAL_DATA\MultiConfig\White Hills\" + site + "Config" + i + ".csv";
                mwvm.LoadFileCommand.Execute(null);
                int j = 0;
                while (mwvm.AllConfigsVM ==null)
                {
                    Thread.Sleep(1000);
                    Console.WriteLine(j++);
                }
                Thread.Sleep(5000);
                SingleConfigViewModel scvm = mwvm.AllConfigsVM.Configs[i-1];
                scvm.PersistFilename = @"\\BP1\public\JesseRichards\REAL_DATA\MultiConfig\White Hills\" + site + "Config" + i + ".txt";
                scvm.LoadPersistedConfigCommand.Execute(null);
                
                Console.WriteLine("Config  "  + i   +  "  " + scvm.DownloadedData[0].Rows.Count + " rows " +
                  " datestart " + scvm.ColumnCollection.DataSetStart + " dateend " + scvm.ColumnCollection.DataSetEnd );

               
                multiconfigs.Add(mwvm.AllConfigsVM.Configs[i-1]);
            }

            Assert.AreEqual(count, multiconfigs.Count);


        }

        private void getTestData()
        {
            //DataRepository repository= new DataRepository(, DataSourceType.CSV);

            List<string> testfiles = new List<string>
            {
                "0302Answers.csv",
                "0302MultiConfigResults.csv"
                
            };
            Console.WriteLine("Loading shear test grids...");
            foreach (string file in testfiles)
            {
                
                string getfile= @"\\BP1\public\JesseRichards\REAL_DATA\MultiConfig\White Hills\" + file;
                DataRepository repository
                      = new DataRepository(getfile, DataSourceType.CSV);

                shearTestGrids.Add(file, repository.GetAllData());

               
            }
            Console.WriteLine("Done loading shear test grids");
        }

        
        private void getMultConfigTestData()
        {
            //DataRepository repository= new DataRepository(, DataSourceType.CSV);

            List<string> testfiles = new List<string>
            {
                "0302Answers.csv"
                
            };
            Console.WriteLine("Loading shear test grids...");
            foreach (string file in testfiles)
            {

                string getfile = @"P:\JesseRichards\REAL_DATA\MultiConfig\White Hills\" + file;
                DataRepository repository
                      = new DataRepository(getfile, DataSourceType.CSV);

                MultiConfigData.Add(repository.GetAllData());


            }
            Console.WriteLine("Done loading multiconfig answer data");
        }

       
        private DataTable GetTable()
        {
            //
            // Here we create a DataTable with four columns.
            //
            DataTable table = new DataTable();
            table.Columns.Add("DateTime", typeof(DateTime));
            table.Columns.Add("WD", typeof(double));
            table.Columns.Add("LowerWS", typeof(double));
            table.Columns.Add("UpperWS", typeof(double));
            
            

            //
            // Here we add 9 DataRows.
            //
            table.Rows.Add(DateTime.Parse("1/1/2010 01:00"), 1, 2.5,-9999);
            table.Rows.Add(DateTime.Parse("3/2/2010 00:10"), 359,4.5,6);
            table.Rows.Add(DateTime.Parse("3/3/2010 00:20"), 5, 6.0,7);
            table.Rows.Add(DateTime.Parse("3/1/2010 00:30"), 355.1, 4.1,8);
            table.Rows.Add(DateTime.Parse("2/4/2010 00:40"), 25, 10.2 ,11);
            table.Rows.Add(DateTime.Parse("2/4/2010 00:50"), 90, 8,11);
            table.Rows.Add(DateTime.Parse("4/4/2010 00:00"), 26, 9,11);
            table.Rows.Add(DateTime.Parse("4/4/2010 00:10"),2, 11,13);
            table.Rows.Add(DateTime.Parse("4/4/2010 00:20"), 8, .5,14);
            table.Rows.Add(DateTime.Parse("4/5/2010 00:20"), 230, -9999,-9999);

            
            return table;
        }
        public void Comps()
        {
           
           foreach(SingleConfigViewModel scvm in multiconfigs )
            {
                WindDirectionComposite wdcomp = new WindDirectionComposite(scvm.ColumnCollection ,scvm.DownloadedData[0]);
                wdcomp.CalculateComposites();

                WindSpeedComposite wscomp = new WindSpeedComposite(scvm.ColumnCollection, scvm.DownloadedData[0]);
                wscomp.CalculateComposites();
            }

                                 
        }
        
        static List<Tag> _tags = new List<Tag>();
        

        [TestFixtureSetUp]
        public static void setup()
        {

            //repository = new DataRepository(_tagfile, DataSourceType.XL2007);
            //data = repository.GetAllData();
            
        }
        
        [Test]
        public void SelectServer()
        {
            //ensure that connection is made to the correct server based on slected type from UI
            MainWindowViewModel mwvm = new MainWindowViewModel();

            mwvm.Datasource = DataSourceType.WindART_PI;


        }
        [Test]
       public void TestDivideDateRange()
       {
           foreach(DateTime[] dt in TurbineDataUtility.Model.Utils.DividedDateRange (DateTime.Parse("1/1/2010"), DateTime.Parse("6/30/2010")))
           {
               Console.WriteLine("start " + dt[0] + " end  " + dt[1]);


           }

       }        
        

       
       

       [Test]
       public void TestReturnAxisValues()
       {   
           //TestAutoConfigureFromDB();

           
           WindDirectionAxis WdAxis = new WindDirectionAxis(20);
           Assert.AreEqual(18, WdAxis.AxisValues.Length);
           WdAxis.BinWidth = 30;

           Assert.AreEqual(12, WdAxis.AxisValues.Length);

           foreach (int wdbin in WdAxis.AxisValues)
           {
               Console.WriteLine(wdbin);
           }
                      

           MonthAxis dateAxis = new MonthAxis();
           Assert.AreEqual(12, dateAxis.AxisValues.Length);
           Assert.AreEqual(10, dateAxis.GetAxisValue(DateTime.Parse("11/01/09")),"month axis");

           HourAxis hourAxis = new HourAxis();
           Assert.AreEqual(24, hourAxis.AxisValues.Length);
           Assert.AreEqual(0, hourAxis.GetAxisValue(DateTime.Parse("11/01/09 00:00:00")));

           //WindDirectionComposite wdcomp = new WindDirectionComposite(InitSettings.collection, InitSettings.data);
           //wdcomp.CalculateComposites();
           //WindSpeedComposite comp = new WindSpeedComposite(InitSettings.collection, InitSettings.data);
           //comp.CalculateComposites();

           //SortedDictionary<double, ISessionColumn> wscomps
           //    = InitSettings.collection.GetColumnsByType(SessionColumnType.WSAvg,
           // InitSettings.start, false);
          // int uppercomp = wscomps.Values.ToList()[1].ColIndex;
           //int upperws = collections[0].UpperWSAvgCols(collections[0].DataSetStart)[0].ColIndex;
           //int lowerws = collections[0].SecondWSAvgCols(collections[0].DataSetStart)[0].ColIndex;

           WindSpeedAxis wsaxis = new WindSpeedAxis(2, GetTable().AsDataView (),2);
           int i=0;
           foreach (int wsbin in wsaxis.AxisValues)
           {   
               
               Console.WriteLine(" ws " + wsbin + "  " +  wsaxis.GetRange(i,Range.start) + " to " + wsaxis.GetRange(i,Range.end));
                i++;
           }
           Assert.AreEqual(0, wsaxis.GetRange(0, Range.start));
           Assert.AreEqual(2, wsaxis.GetRange(0, Range.end));
           Assert.AreEqual(2, wsaxis.Incrementor);
           Assert.AreEqual(4, wsaxis.GetRange(2, Range.start));
           Assert.AreEqual(6, wsaxis.GetRange(2, Range.end));


       }
       [Test]
       public void TestWDAxisGetSingleElements()
       {
           WindDirectionAxis WdAxis = new WindDirectionAxis(10);
           foreach (int wd in WdAxis.AxisValues)
           {
               Console.WriteLine(wd + " " + WdAxis.GetRange (wd/10,Range.start) + " to " + WdAxis.GetRange(wd/10,Range.end ));
               Console.WriteLine(wd + " " + WdAxis.GetAxisValue(wd));

           }

           //Assert.AreEqual(0, WdAxis.GetAxisValue(0));
           //Assert.AreEqual(10, WdAxis.GetAxisValue(1));
           //Assert.AreEqual(20, WdAxis.GetAxisValue(2));
           //Assert.AreEqual(30, WdAxis.GetAxisValue(3));
           //Assert.AreEqual(40, WdAxis.GetAxisValue(4));
           //Assert.AreEqual(50, WdAxis.GetAxisValue(5));


       }
       [Test]
       public void TestWSAxisGetSingleElements()
       {
           WindSpeedAxis WSAxis = new WindSpeedAxis(2.0,GetTable().AsDataView (),2);
           foreach (int ws in WSAxis.AxisValues)
           {
               Console.WriteLine(ws );

           }

           Console.WriteLine(WSAxis.GetRange (0,Range.start)  + " to " + WSAxis.GetRange (0,Range.end));
           Console.WriteLine(WSAxis.GetRange(1, Range.start) + " to " + WSAxis.GetRange(1, Range.end));
           Console.WriteLine(WSAxis.GetRange(2, Range.start) + " to " + WSAxis.GetRange(2, Range.end));

                
          


       }
       [Test]
       public void TestRowfilterTestDataSet()
       {
           DataTable test = GetTable();
            List<DataRow> rows=new List<DataRow >();
           foreach(DataRow dr in test.Rows)
           {
               rows.Add(dr);
           }
           RowFilterFactory filterfactory = new RowFilterFactory();
           AbstractRowFilter filter = filterfactory.CreateRowFilter(rows);

           //date only
           List<double> result = filter.Filter(DateTime.Parse("1/1/2010"), DateTime.Parse("4/1/2010"), 0, 2);
           Assert.AreEqual(6, result.Count);
            
           IAxis wdaxis = new WindDirectionAxis(10);
           IAxis wsaxis = new WindSpeedAxis(2, test.AsDataView(), 2);
           
           AbstractRowFilter filter1 = filterfactory.CreateRowFilter(wdaxis,wsaxis,rows);

           //x and y and date
           var result1 = filter1.Filter(0,2, 1, 2, 2, DateTime.Parse("1/1/2010"),DateTime.Parse("4/1/2010"),0);
           Assert.AreEqual(2, result1.Count);
           
           //one axis and date
           result1 = filter1.Filter(0,1,DateTime.Parse("1/1/2010"), DateTime.Parse("4/1/2010"),0,2);
           Assert.AreEqual(3, result1.Count);
           
       }
       
       [Test]
       public void TestShearCoincident()
       {
           
           //TestAutoConfigureFromDB();
           getFileData();
           Comps();
           //int upperws = collections[0].UpperWSComp(testDate);
           //int lowerws = collections[0].LowerWSComp(testDate);
           //int wd = collections[0].WDComp(testDate);
           //Console.WriteLine("thinks upper comp is at " + upperws);
           List<int> idx = new List<int> {22,23,24};

           ShearCoincidentRows r = new ShearCoincidentRows(idx, bdsv.DownloadedData[0]);
           coincidentlist = r.GetRows();
           
           var answer = bdsv.DownloadedData[0].AsEnumerable().Where(c => c.Field<double>(22) >= 0 & c.Field<double>(23) >= 0 & c.Field<double>(24) >= 0);
           Console.WriteLine(answer.Count());
           Assert.AreEqual (answer.Count(),coincidentlist.Count); 
 
           

           
         
       }
       [Test]
       public void TestImportingShearAnswerGrids()
       {
           getTestData();

           foreach (KeyValuePair<string, DataTable> kv in shearTestGrids)
           {
               Assert.IsInstanceOf(typeof(DataTable), kv.Value);
           }

       }
       
       [Test]
       public void TestBuildWhereClause()
       {
           WhereClauseFactory factory = new WhereClauseFactory();
           var result=factory.CreateWhereClause(new MonthAxis() ,testDate , 0);
           Console.WriteLine(result.ToString());
           Assert.IsInstanceOf(typeof(Expression<Func<DataRow, bool>>),result);

       }
       
       [Test]
       public void TestWdbyHourFilterTestDataSet()
       {
           

           //int upperws = 2;
           //int wd = 1;
           
           //Console.WriteLine(upperws);
           
           DataTable tbl=GetTable();
           
           //accept a list of datarows return a filtered list of doubles
           IAxis xaxis=new MonthAxis ();
           IAxis yaxis=new HourAxis ();
           IAxis wdaxis = new WindDirectionAxis(10);
           IAxis wsaxis = new WindSpeedAxis(2,tbl.AsDataView (), 2);

           //month vs hour 
           //AbstractRowFilter filter=new RowFilterFactory().CreateRowFilter (xaxis ,yaxis,tbl.AsEnumerable ().ToList());

           //List<double> result = filter.Filter(7, testDate.AddHours(4), 0, 0, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average ());
           //Assert.Greater(result.Count, 0);
           
           //hour vs wd

           List<DataRow> rows=new List<DataRow >();
           foreach(DataRow dr in tbl.Rows)
           {
               rows.Add(dr);
           }
           AbstractRowFilter filter0 = new RowFilterFactory().CreateRowFilter(wdaxis, yaxis,rows );

           foreach (DataRow dr in rows)
           {
               
               Console.WriteLine(DateTime.Parse(dr[0].ToString ()).Hour  +", " + dr[1].ToString () + ", " + dr[2].ToString ());
           }

           //Console.WriteLine( rows.Where(c => ((c.Field<double>(1)>355.0 & c.Field<double>(1)<361) |
           //    (c.Field<double>(1)>=0 & c.Field<double>(1)<=5)) ).Count() + " wd found independently");

           List<Double> result = filter0.Filter(0, 1, 1, 0, 2);
           Console.WriteLine("wd by hour " + result.Count());
           //Assert.AreEqual(4, result.Count());

           //month vs wd 
           //AbstractRowFilter filter1 = new RowFilterFactory().CreateRowFilter(xaxis, wdaxis, tbl.AsEnumerable().ToList());
           //result = filter1.Filter(testDate,5, 0, wd, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           // Assert.Greater(result.Count, 0);

           //result = filter1.Filter(testDate,6, 0, wd, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           // Assert.Greater(result.Count, 0);

           //result = filter.Filter(testDate, testDate , 0, 0, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           // Assert.Greater(result.Count, 0);

           //result = filter.Filter(testDate, testDate, 0, 0, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           // Assert.Greater(result.Count, 0);

           //ws vs wd
           // AbstractRowFilter filter2 = new RowFilterFactory().CreateRowFilter(wsaxis, wdaxis, tbl.AsEnumerable().ToList());
           //result = filter2.Filter(2, 4, upperws , wd, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           // Assert.Greater(result.Count, 0);

           //result = filter2.Filter(3, 4, upperws, wd, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           //Assert.Greater(result.Count, 0);

           //result = filter2.Filter(4, 4, upperws, wd, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           //Assert.Greater(result.Count, 0);
       }
       [Test]
       public void TestWdbyMonthFilterTestDataSet()
       {


           //int upperws = 2;
           //int wd = 1;

           //Console.WriteLine(upperws);

           DataTable tbl = GetTable();

           //accept a list of datarows return a filtered list of doubles
           IAxis xaxis = new MonthAxis();
           
           IAxis wdaxis = new WindDirectionAxis(10);
                    

           List<DataRow> rows = new List<DataRow>();
           foreach (DataRow dr in tbl.Rows)
           {
               rows.Add(dr);
           }
           AbstractRowFilter filter0 = new RowFilterFactory().CreateRowFilter(wdaxis, xaxis, rows);

           foreach (DataRow dr in rows)
           {

               Console.WriteLine(DateTime.Parse(dr[0].ToString()).Month + ", " + dr[1].ToString() + ", " + dr[2].ToString());
           }

          
           List<Double> result = filter0.Filter(0, 3, 1, 0, 2);
           Console.WriteLine("wd by hour " + result.Count());
           //Assert.AreEqual(4, result.Count());

           
       }
       [Test]
       public void TestWS_byHourFilterTestDataSet()
       {


           //int upperws = 2;
           //int wd = 1;

           

           DataTable tbl = GetTable();

           //accept a list of datarows return a filtered list of doubles
           IAxis xaxis = new MonthAxis();
           IAxis yaxis = new HourAxis();
           IAxis wdaxis = new WindDirectionAxis(10);
           IAxis wsaxis = new WindSpeedAxis(2.0, tbl.AsDataView(), 2);

          
           //hour vs wd

           List<DataRow> rows = new List<DataRow>();
           foreach (DataRow dr in tbl.Rows)
           {
               rows.Add(dr);
           }
           AbstractRowFilter filter0 = new RowFilterFactory().CreateRowFilter(wsaxis, yaxis, rows);

           foreach (DataRow dr in rows)
           {

               Console.WriteLine(DateTime.Parse(dr[0].ToString()).Hour + ", " + dr[1].ToString() + ", " + dr[2].ToString());
           }

           Console.WriteLine(rows.Where(c => (c.Field<double>(2) >= 0 & c.Field<double>(2) < 1) && c.Field<DateTime>(0).Hour==0).Count());
               

           List<Double> result = filter0.Filter(0,0,2,0,2);
           Console.WriteLine("ws by hour " + result.Count());
           //Assert.AreEqual(4, result.Count());

           //month vs wd 
           //AbstractRowFilter filter1 = new RowFilterFactory().CreateRowFilter(xaxis, wdaxis, tbl.AsEnumerable().ToList());
           //result = filter1.Filter(testDate,5, 0, wd, upperws);
           //Console.WriteLine(result.Count + " Avg " + result.Average());
           // Assert.Greater(result.Count, 0);

          
       }
       [Test]
       public void TestWS_byMonthrFilterTestDataSet()
        {


            //int upperws = 2;
            //int wd = 1;



            DataTable tbl = GetTable();

            //accept a list of datarows return a filtered list of doubles
            IAxis xaxis = new MonthAxis();
            IAxis wsaxis = new WindSpeedAxis(2.0, tbl.AsDataView(), 2);


            //hour vs wd

            List<DataRow> rows = new List<DataRow>();
            foreach (DataRow dr in tbl.Rows)
            {
                rows.Add(dr);
            }
            AbstractRowFilter filter0 = new RowFilterFactory().CreateRowFilter(wsaxis, xaxis, rows);

            foreach (DataRow dr in rows)
            {

                Console.WriteLine(DateTime.Parse(dr[0].ToString()).Hour + ", " + dr[1].ToString() + ", " + dr[2].ToString());
            }

            Console.WriteLine(rows.Where(c => (c.Field<double>(2) >= 0 & c.Field<double>(2) < 1) && c.Field<DateTime>(0).Month == 4).Count());


            List<Double> result = filter0.Filter(0, 3, 2, 0, 2);
            Console.WriteLine("ws by month " + result.Count());
            

        }
       [Test]
       public void TestMultipleShearGrids()
       {          

           //user can create multiple shear grids
           //**1st alpha set 
           //TestAutoConfigureFromDB();
           //getFileData();
           getMultiConfigsData();
           Comps();
           
          Console.WriteLine("testdate=" + testDate);
           SingleConfigViewModel scvm = multiconfigs[0];
           testDate = scvm.ColumnCollection.DataSetStart;
           int wd =  scvm.ColumnCollection.WDComp(testDate);

           int upperwsidx = scvm.ColumnCollection.UpperWSComp(testDate);
           int lowerwsidx = scvm.ColumnCollection.LowerWSComp(testDate);
           int datetimeidx = scvm.ColumnCollection.DateIndex;

           ISessionColumn upperws = scvm.ColumnCollection[upperwsidx];
           ISessionColumn lowerws = scvm.ColumnCollection[lowerwsidx];

           Console.WriteLine(" indexes passed to coincident rows " + upperwsidx.ToString() + "," + lowerwsidx.ToString() + ", " +  wd.ToString());

           List<int> idx = new List<int> { upperwsidx, lowerwsidx, wd };
           ShearCoincidentRows r = new ShearCoincidentRows(idx, scvm.DownloadedData[0]);

           TimeSpan t = new TimeSpan();
           DateTime start = DateTime.Now;

           coincidentlist = r.GetRows();

           t = DateTime.Now - start;
           Console.WriteLine(" time for coincident rows: " + t);

           AxisFactory factory = new AxisFactory();
           IAxis houraxis = factory.CreateAxis(AxisType.Hour);
           houraxis.SessionColIndex = datetimeidx;
           
           IAxis monthaxis = factory.CreateAxis(AxisType.Month);
           monthaxis.SessionColIndex = datetimeidx;
           IAxis wdaxis=factory.CreateAxis (10.0);
           wdaxis.SessionColIndex = wd;
           IAxis wsaxis = factory.CreateAxis(2.0, scvm.DownloadedData[0].AsDataView (), upperws.ColIndex );
           wsaxis.SessionColIndex = upperwsidx;

           Console.WriteLine(" coincident list " + coincidentlist.Count);
           //ISessionColumn uppeWS=collection .
           List<Alpha> alphaCollection = new List<Alpha>()
           {
                
                {new Alpha(coincidentlist, upperws, lowerws, monthaxis, houraxis)},
                //{new Alpha(coincidentlist, upperws, lowerws, houraxis, monthaxis)},
                //{new Alpha(coincidentlist, upperws, lowerws, wdaxis, houraxis)},
               // {new Alpha(coincidentlist, upperws, lowerws, wsaxis, houraxis)},
                {new Alpha(coincidentlist, upperws, lowerws, houraxis, wdaxis)},
                {new Alpha(coincidentlist, upperws, lowerws, monthaxis, wdaxis)},
                //{new Alpha(coincidentlist, upperws, lowerws, wdaxis, monthaxis)},
                {new Alpha(coincidentlist, upperws, lowerws, wdaxis, wsaxis)},
                //{new Alpha(coincidentlist, upperws, lowerws, wsaxis, wdaxis)},
                {new Alpha(coincidentlist, upperws, lowerws, houraxis, wsaxis)},
                {new Alpha(coincidentlist, upperws, lowerws, monthaxis, wsaxis)}
           
           };

           Assert.IsInstanceOf(typeof(Alpha), alphaCollection[0]);
           //Assert.AreEqual(10, alphaCollection.Count);

           TimeSpan ts = new TimeSpan();
           
           foreach (Alpha a in alphaCollection)
           {
               DateTime dt = DateTime.Now;
               a.CalculateAlpha();

               
               ts = DateTime.Now - dt;
               Console.WriteLine("calculated " + a.Xaxis.AxisType + " by  " + a.Yaxis.AxisType + " ---> " + ts);
           
          
           
           }

           //getTestData ();
           foreach (Alpha alpha in alphaCollection)
           {
               int i = alpha.UpperAvg.GetUpperBound(0);
               int j = alpha.UpperAvg.GetUpperBound(1);
               string keyCount =  alpha.Xaxis.AxisType + "By" + alpha.Yaxis.AxisType + "Count.csv";
               string keyUpper = alpha.Xaxis.AxisType + "By" + alpha.Yaxis.AxisType + "Upper.csv";
               string keyLower = alpha.Xaxis.AxisType + "By" + alpha.Yaxis.AxisType + "Lower.csv";
               string keyAlpha = alpha.Xaxis.AxisType + "By" + alpha.Yaxis.AxisType + "Alpha.csv";
               Console.WriteLine(keyCount );

               //test grid ---claculated result

               
               //for (int a = 0; a <= i; a++)
               //{
               //    for (int b = 0; b <= j; b++)
               //    {
               //        int dec=8;
               //        if (shearTestGrids[keyCount].Rows[b][a].ToString() != alpha.UpperAvgCount [a, b].ToString ())
               //        {
               //        Console.WriteLine(b + " " + a + " Count   " + shearTestGrids[keyCount].Rows[b][a].ToString() + "     " + alpha.UpperAvgCount [a, b]);
                       
               //        }

               //        if (Math.Abs (Math.Round(Double.Parse(shearTestGrids[keyUpper].Rows[b][a].ToString()), dec) - Math.Round(alpha.UpperAvg[a, b], dec))>.00001)
               //        {
               //            Console.WriteLine(b + " " + a + " Upper   " +
               //                Math.Round(Double.Parse(shearTestGrids[keyUpper].Rows[b][a].ToString()), dec).ToString() + " " + Math.Round(alpha.UpperAvg[a, b], dec).ToString()
               //                );

               //        }

               //        if (Math.Abs(Math.Round(Double.Parse(shearTestGrids[keyLower].Rows[b][a].ToString()), dec)- Math.Round(alpha.LowerAvg[a, b], dec))>.00001)
               //        {
               //            Console.WriteLine(b + " " + a + " Lower   " +
               //                Math.Round(Double.Parse(shearTestGrids[keyLower].Rows[b][a].ToString()), dec).ToString() + " " +  Math.Round(alpha.LowerAvg[a, b], dec).ToString());

               //        }

               //        if (Math.Abs(Math.Round(Double.Parse(shearTestGrids[keyAlpha].Rows[b][a].ToString()), dec) - Math.Round(alpha.Alpha[a, b], dec)) > .00001)
               //        {
               //            Console.WriteLine(b + " " + a + " Alpha " + 
               //                Math.Round(Double.Parse(shearTestGrids[keyAlpha].Rows[b][a].ToString ()),dec).ToString() + " " +  Math.Round(alpha.Alpha[a, b],dec).ToString()
               //                + " diff  " + (Double.Parse(shearTestGrids[keyAlpha].Rows[b][a].ToString()) - alpha.Alpha[a, b]).ToString());

               //        }
               //    }
               //}

               //write out to a data table and save out
               //make datatable from double. first col has axis values
               DataTable dt1 = new DataTable();
               List<string> leftCol=new List<string> ();
               string file =scvm.DisplayName + "_" + DateTime.Now.ToShortDateString().Replace("/", "") + "_" + alpha.Name;
                WindART.Utils.AddColtoDataTable <double>("---", alpha.Yaxis.AxisValues.ToList(),dt1);
                           
               //alpha
               for (int g = 0; g <= alpha.Alpha.GetUpperBound(0); g++)
               {
                      
                    List<double> thisRow = new List<double>();
                   for (int h = 0; h <= alpha.Alpha.GetUpperBound(1); h++)
                   {
                       thisRow.Add(alpha.Alpha[g,h]);


                   }
                   
                  WindART.Utils.AddColtoDataTable <double>(g.ToString (),thisRow,dt1);
               }
               WindART.Utils.OutputFile(dt1, @"\\BP1\Public\JesseRichards\WindArt\Ekho\Testing\TestData\ToPatrick\" + file + "_Alpha.csv");
               dt1.Clear();
               
               //upperavg
               WindART.Utils.AddColtoDataTable<double>("---", alpha.Yaxis.AxisValues.ToList(), dt1);
               for (int g = 0; g <= alpha.UpperAvg .GetUpperBound(0); g++)
               {

                   List<double> thisRow = new List<double>();
                   for (int h = 0; h <= alpha.UpperAvg .GetUpperBound(1); h++)
                   {
                       
                       thisRow.Add(alpha.UpperAvg [g, h]);


                   }

                   WindART.Utils.AddColtoDataTable<double>(g.ToString(), thisRow, dt1);
               }
               WindART.Utils.OutputFile(dt1, @"\\BP1\Public\JesseRichards\WindArt\Ekho\Testing\TestData\ToPatrick\" + file + "_UpperAvg.csv");


               //loweravg
               WindART.Utils.AddColtoDataTable<double>("---", alpha.Yaxis.AxisValues.ToList(), dt1);
               for (int g = 0; g <= alpha.LowerAvg .GetUpperBound(0); g++)
               {

                   List<double> thisRow = new List<double>();
                   for (int h = 0; h <= alpha.LowerAvg.GetUpperBound(1); h++)
                   {
                       
                       thisRow.Add(alpha.LowerAvg [g, h]);
                       
                   }

                   WindART.Utils.AddColtoDataTable<double>(g.ToString(), thisRow, dt1);
               }
               WindART.Utils.OutputFile(dt1, @"\\BP1\Public\JesseRichards\WindArt\Ekho\Testing\TestData\ToPatrick\" + file +  "_LowerAvg.csv");

               //uppercount
               WindART.Utils.AddColtoDataTable<double>("---", alpha.Yaxis.AxisValues.ToList(), dt1);
               for (int g = 0; g <= alpha.UpperAvgCount.GetUpperBound(0); g++)
               {

                   List<double> thisRow = new List<double>();
                   for (int h = 0; h <= alpha.UpperAvgCount .GetUpperBound(1); h++)
                   {


                       thisRow.Add(alpha.UpperAvgCount [g, h]);


                   }

                   WindART.Utils.AddColtoDataTable<double>(g.ToString(), thisRow, dt1);
               }
               WindART.Utils.OutputFile(dt1, @"\\BP1\Public\JesseRichards\WindArt\Ekho\Testing\TestData\ToPatrick\" + file + "_UpperAvgCount.csv");
            
               //lower count
               WindART.Utils.AddColtoDataTable<double>("---", alpha.Yaxis.AxisValues.ToList(), dt1);
               for (int g = 0; g <= alpha.LowerAvgCount.GetUpperBound(0); g++)
               {

                   List<double> thisRow = new List<double>();
                   for (int h = 0; h <= alpha.LowerAvgCount.GetUpperBound(1); h++)
                   {

                       thisRow.Add(alpha.LowerAvgCount[g, h]);


                   }

                   WindART.Utils.AddColtoDataTable<double>(g.ToString(), thisRow, dt1);
               }
               WindART.Utils.OutputFile(dt1, @"\\BP1\Public\JesseRichards\WindArt\Ekho\Testing\TestData\ToPatrick\" + file + "_LowerAvgCount.csv");
         
           }

           

           
       }
       
        //apply any shear to any windspeed to calculate a derived height
       
       [Test]
       public void DeriveHubHeight_MonthbyHour()
       {
           getFileData();
           //getTwoConfigsData();
           Comps();
           SingleConfigViewModel scvm = (SingleConfigViewModel)bdsv;
           DataTable dataview = bdsv.DownloadedData[0];
           Console.WriteLine("testdate=" + testDate);

           int wd = scvm.ColumnCollection.WDComp(testDate);

           int upperwsidx = scvm.ColumnCollection.UpperWSComp(testDate);
           int lowerwsidx = scvm.ColumnCollection.LowerWSComp(testDate);
           int datetimeidx = collections[0].DateIndex;

           SessionColumn upperws = (SessionColumn)scvm.ColumnCollection[upperwsidx];
           SessionColumn lowerws = (SessionColumn)scvm.ColumnCollection[lowerwsidx];

           List<int> idx = new List<int> {upperwsidx, lowerwsidx, wd};
           ShearCoincidentRows r = new ShearCoincidentRows(idx, bdsv.DownloadedData[0]);

           TimeSpan t = new TimeSpan();
           DateTime start = DateTime.Now;

           coincidentlist = r.GetRows();

           t = DateTime.Now - start;
           Console.WriteLine(" time for coincident rows: " + t);

           AxisFactory factory = new AxisFactory();
           IAxis wdaxis = factory.CreateAxis(10);
           wdaxis.SessionColIndex = wd;

           factory = new AxisFactory();
           IAxis houraxis = factory.CreateAxis(AxisType.Hour);
           houraxis.SessionColIndex = datetimeidx;

           IAxis monthaxis = factory.CreateAxis(AxisType.Month);
           monthaxis.SessionColIndex = datetimeidx;

           
           Console.WriteLine(" coincident list " + coincidentlist.Count);
           //ISessionColumn uppeWS=collection .
           AlphaFactory afactory = new AlphaFactory();
           List<AbstractAlpha > alphaCollection = new List<AbstractAlpha>()
           {
                
                {afactory .CreateAlpha (coincidentlist, upperws, lowerws, monthaxis, houraxis)}
                //{afactory .CreateAlpha (coincidentlist, upperws, lowerws, wdaxis, houraxis)},
                //{afactory .CreateAlpha (coincidentlist, upperws, lowerws, wdaxis, monthaxis)}

                     
           };
           
           alphaCollection.ForEach (c=>c.CalculateAlpha());
           
           //get source data
           MergedWSFactory mergeFactory = new MergedWSFactory();
           AbstractMergeWS ws = mergeFactory.CreateMergedWS();
           //DataTable tbl = GetTable();

           
           Dictionary<int,Dictionary<double, List<double>>> param = new Dictionary<int,Dictionary<double, List<double>>>()
           {
              {2,new Dictionary<double,List<double>>(){ {lowerws.Height , WindART.Utils.ExtractDataTableColumn<double>(lowerwsidx ,bdsv.DownloadedData[0])}}},
              {1,new Dictionary<double,List<double>>(){ {upperws.Height , WindART.Utils.ExtractDataTableColumn<double>(upperwsidx ,bdsv.DownloadedData[0])}}}
              
           };

           double[,] result = ws.Merge(param);

           AbstractDeriveWS derived = new DerivedWSFactory().CreateDerivedWSObject();
           
           //List<double> newWS=derived.DeriveNewWS(collections[0], alphaCollection[0], result, bdsv.DownloadedData[0], 80.0);
           //Console.WriteLine(" source ws count: " + result.GetLength(1) + " new ws count: " + newWS.Count);
           
          List<List<double>> newWS =new List<List<double>>();
           
              newWS.Add ( derived.DeriveNewWS(collections[0], alphaCollection[0], result, bdsv.DownloadedData[0], 80.0));
              newWS.Add(derived.DeriveNewWS(collections[0], alphaCollection[0], result, bdsv.DownloadedData[0], 65.0));
              newWS.Add(derived.DeriveNewWS(collections[0], alphaCollection[0], result, bdsv.DownloadedData[0], 30.0));
          
          //load in answers to multicofigdata
          getMultConfigTestData();
          int useconfig = 1;
        for (int j = 0; j <=newWS.Count -1; j++)
              {
                  for (int i = 0; i <= MultiConfigData[useconfig].Rows.Count - 1; i++)
                  {

                      double thisVal = double.Parse (MultiConfigData[useconfig].Rows[i][j+1].ToString());
                      
                          if (thisVal > 0)
                          {
                              if (Math.Abs(newWS[j][i] - double.Parse(MultiConfigData[useconfig].Rows[i][j+1].ToString())) < .01)
                              // if (Math.Abs(newWS[j][i] - double.Parse(MultiConfigData[0].Rows[i][j+1].ToString())) < .01)
                              { Assert.Pass(); }
                              else
                              {
                                  Console.WriteLine("column is " + j + "  row is " + i + "   " + MultiConfigData[useconfig].Rows[i][0].ToString() + "  " + newWS[j][i] + "   " + MultiConfigData[0].Rows[i][j + 1].ToString());
                                  Assert.Fail("Values did not match");
                              }
                          }
                      
                  }
          }


           //newWS = derived.DeriveNewWS(collections[0], alphaCollection[2], result, bdsv.DownloadedData[0], 80.0);
           //Console.WriteLine(" source ws count: " + result.GetLength(1) + " new ws count: " + newWS.Count);
           

           
           //load in answers and compare--

       }
       [Test]
       public void DeriveHubHeight_MonthbyHour_MultiConfig()
       {
           //calculate config 2's derived heights with config 1's alphas
           //getFileData();
           getMultiConfigsData();

           //generate composites for each config dataset 
           Comps();

           List<AbstractAlpha> alphaCollection = new List<AbstractAlpha>();

          foreach(SingleConfigViewModel scvm in multiconfigs)
          {
              
           //calculate alphas for each config 
           DateTime useDate = scvm.ColumnCollection.DataSetStart;
           Console.WriteLine("Dataset start " + useDate);
           int wd = scvm.ColumnCollection.WDComp(useDate );
           int upperwsidx = scvm.ColumnCollection.UpperWSComp(useDate);
           int lowerwsidx = scvm.ColumnCollection.LowerWSComp(useDate);
           int datetimeidx = scvm.ColumnCollection.DateIndex;

           
           ISessionColumn upperws = scvm.ColumnCollection[upperwsidx];
           ISessionColumn lowerws = scvm.ColumnCollection[lowerwsidx];


           Console.WriteLine(upperws.ColName + " " + lowerws.ColName);  


           List<int> idx = new List<int> { upperwsidx, lowerwsidx, wd };
           ShearCoincidentRows r = new ShearCoincidentRows(idx, scvm.DownloadedData [0]);

           TimeSpan t = new TimeSpan();
           DateTime start = DateTime.Now;

           coincidentlist = r.GetRows();

           t = DateTime.Now - start;
           Console.WriteLine(" time for coincident rows: " + t);
            
           //create axes 
           AxisFactory factory = new AxisFactory();
           IAxis wdaxis = factory.CreateAxis(10);
           wdaxis.SessionColIndex = wd;

           factory = new AxisFactory();
           IAxis houraxis = factory.CreateAxis(AxisType.Hour);
           houraxis.SessionColIndex = datetimeidx;

           IAxis monthaxis = factory.CreateAxis(AxisType.Month);
           monthaxis.SessionColIndex = datetimeidx;


           Console.WriteLine(" coincident list " + coincidentlist.Count);
           //ISessionColumn uppeWS=collection .
           AlphaFactory afactory = new AlphaFactory();
           
                //initialize  the alpha grid 
               alphaCollection .Add( afactory .CreateAlpha (coincidentlist, upperws, lowerws, monthaxis, wdaxis));
                //{afactory .CreateAlpha (coincidentlist, upperws, lowerws, wdaxis, houraxis)},
                //{afactory .CreateAlpha (coincidentlist, upperws, lowerws, wdaxis, monthaxis)}

                     
           
              //create the alpha grids 
               
              alphaCollection.ForEach(c => c.CalculateAlpha());
              
          }//each single config view model 
           
           //-----
           //this section applies an alpha to each config's data 
           //------------

          
           //get source data for deriving ws 
           MergedWSFactory mergeFactory = new MergedWSFactory();

           AbstractMergeWS ws = mergeFactory.CreateMergedWS();
           
           //map alpha grid to configs for deriving heights 
           List<int[,]> configMapping = new List<int[,]>()
           {
               {new int[1,2]{{0,0}}},
               
           };


           foreach (int[,] config in configMapping)
           {

                        SingleConfigViewModel thisScvm=multiconfigs[config[0, 0]];
                       DateTime DeriveDate = thisScvm.ColumnCollection.DataSetStart;

                       int DrvUpperwsidx = thisScvm.ColumnCollection.UpperWSComp(DeriveDate);
                       int DrvLowerwsidx = thisScvm.ColumnCollection.LowerWSComp(DeriveDate);

                       SessionColumn DrvUpperws = (SessionColumn)thisScvm.ColumnCollection[DrvUpperwsidx];
                       SessionColumn DrvLowerws = (SessionColumn)thisScvm.ColumnCollection[DrvLowerwsidx];


                           //prepare parameters for obtaining data column to apply alpha to 
                       Dictionary<int, Dictionary<double, List<double>>> param = new Dictionary<int, Dictionary<double, List<double>>>()
           {
              {2,new Dictionary<double,List<double>>(){ {DrvLowerws.Height , WindART.Utils.ExtractDataTableColumn<double>(DrvLowerwsidx ,bdsv.DownloadedData[0])}}},
              {1,new Dictionary<double,List<double>>(){ {DrvUpperws.Height , WindART.Utils.ExtractDataTableColumn<double>(DrvUpperwsidx ,bdsv.DownloadedData[0])}}}
              
           };

                           
                       double[,] result = ws.Merge(param);

                       AbstractDeriveWS derived = new DerivedWSFactory().CreateDerivedWSObject();

                       //List<double> newWS=derived.DeriveNewWS(collections[0], alphaCollection[0], result, bdsv.DownloadedData[0], 80.0);
                       //Console.WriteLine(" source ws count: " + result.GetLength(1) + " new ws count: " + newWS.Count);
                      
                       
                       
                     
                      
                     //Add the new column to the datatable 
                    foreach(double d in new List<double>(){80.0})
                    {    string newDerivedCol= (d).ToString () + "m_12by24_WindSpeed";
                        
                        Console.WriteLine ("calculating " + d + " m derived ws");
                           
                          WindART.Utils.AddColtoDataTable<double>(newDerivedCol
                          ,derived.DeriveNewWS(thisScvm.ColumnCollection, alphaCollection[config[0, 1]], result, thisScvm.DownloadedData[0], d),
                          thisScvm.DownloadedData [0]);
                      
                    //add the column definition and config 
                    int idx=thisScvm.DownloadedData [0].Columns [newDerivedCol ].Ordinal;
                    thisScvm.ColumnCollection .Columns.Add(new SessionColumn (idx, newDerivedCol ){ColumnType =SessionColumnType.WSAvgShear ,IsCalculated =true });
                    //add config 
                    Console.WriteLine(thisScvm.ColumnCollection.DataSetStart  + " " + thisScvm.ColumnCollection.DataSetEnd) ;
                    thisScvm.ColumnCollection [idx].addConfig (new SensorConfig(){Height=d,StartDate =thisScvm.ColumnCollection.DataSetStart,EndDate =thisScvm.ColumnCollection .DataSetEnd});
                    }
           }
           
//________________________________________________________________________________________________________________________________________________________________________________________
           //ADD NEW COLS TO UNIFIED DATASET

           //Combine all datasets--we can only include columns that have been marked across configs 
           //upperwscomp, lower ws comp, sheared up ws 
           
           DataTable newTable = new DataTable();
           newTable.Columns.Add(new DataColumn() { ColumnName = "DateTime", DataType = typeof(DateTime) });
           SessionColumnCollection newCols = new SessionColumnCollection(newTable);
           List<DateTime> dateCol = new List<DateTime>();
           Dictionary<string, List<double>> numericData = new Dictionary<string, List<double>>();
           
           
           foreach (SingleConfigViewModel scvm in multiconfigs)
           {

               //add dates to temp array
               if(dateCol.Count>0)
               {
                   Console.WriteLine("adding dates to existing list");
                   dateCol.AddRange (WindART.Utils.ExtractDataTableColumn <DateTime>(0,scvm.DownloadedData [0]));
               }
               else
               {
                Console.WriteLine("Adding dates to initial list");
                dateCol=WindART.Utils.ExtractDataTableColumn <DateTime>(0,scvm.DownloadedData [0]);
               }

                var commonCols = from SessionColumn s in scvm.ColumnCollection.Columns.AsEnumerable()
                                where s.IsCalculated && s.ColumnType == SessionColumnType.WSAvgShear
                                select s;
                Console.WriteLine("Config " + scvm.ColumnCollection.DataSetStart + " " + commonCols.Count() + " calculated columns");
               foreach (SessionColumn sc in commonCols.ToList())
               {
                   //if the unified config already contains a calculated column at the same ht and type then extend its  end date otherwise add it
                   SessionColumn sc1 = (SessionColumn)newCols.Columns.SingleOrDefault (c => c.ColumnType == SessionColumnType.WSAvgShear & 
                       c.Configs[0].Height == sc.Configs[0].Height);
                   if (sc1 != null)
                   {
                       //extend the column collection and config dates 
                       newCols.DataSetEnd  = sc.Configs[0].EndDate;
                       sc1.Configs[0].EndDate = sc.Configs[0].EndDate;
                   }
                   else
                   {
                       //or add the column if its not yet in the unified config 
                       newCols.Columns.Add(sc);
                   }
               }
               Console.WriteLine("Copied columns from old configs");
               //build the common data 
               ////get the index of each calculated,wsavg column 
               
               var calculatedCols = from SessionColumn s in scvm.ColumnCollection.Columns.AsEnumerable()
                                    where s.ColumnType == SessionColumnType.WSAvgShear && s.IsCalculated
                                    select s;
               
               
                  
                   foreach (SessionColumn scol in calculatedCols)
                   {
                       if(numericData.ContainsKey(scol.ColName))
                       {
                           //add to an existing key
                           numericData[scol.ColName ].AddRange(WindART.Utils.ExtractDataTableColumn <double>(scol.ColIndex ,scvm.DownloadedData [0]));
                       }
                       else
                       {//add the data for the first time 
                           numericData.Add(scol.ColName, WindART.Utils.ExtractDataTableColumn<double>(scol.ColIndex, scvm.DownloadedData[0]));
                       }
                   }
                    
                   
               
              

           }
           Console.WriteLine("Built work data arrays");
           //load in answers to multicofigdata--adds as another dataset in multiconfigs 
           //getMultConfigTestData();
           Assert.Greater(dateCol.Count, 1000);

            
           WindART.Utils.AddColtoDataTable<DateTime>("DateTime", dateCol, newTable);

           foreach (KeyValuePair<string, List<double>> kv in numericData)
           {
               WindART.Utils.AddColtoDataTable<double>(kv.Key, kv.Value,newTable);

           }

            string filename=@"C:\Users\Oakland\Desktop";
               WindART.Utils.OutputFile(newTable,filename);

       }
       [Test]
       public void DeriveHubHeight_MonthbyWD()
       {
       }
       [Test]
       public void DeriveHubHeight_MonthbyWS()
       { }
       [Test]
       public void DeriveHubHeight_HourbyWD()
       {
       }
       [Test]
        public void DeriveHubHeight_HourbyWS()
       {
       }
        [Test]
       public void DeriveHubHeight_WDbyWS()
       { }

        //derive common ws hieghts 
        [Test]
        public void DeriveCommonHeight()
        {
            double htToShearTo = 80;
            Dictionary<string, List<double>> tempCommonHtSet = new Dictionary<string, List<double>>();
            
            //add multiple configs to session
            getMultiConfigsData();

            foreach (SingleConfigViewModel scvm in multiconfigs)
            {
                WindDirectionComposite wdcomp = new WindDirectionComposite(scvm.ColumnCollection , scvm.DownloadedData[0]);
                wdcomp.CalculateComposites();

                WindSpeedComposite wscomp = new WindSpeedComposite(scvm.ColumnCollection , scvm.DownloadedData[0]);
                wscomp.CalculateComposites();

                testDate = scvm.DataSetStartDate.AddDays(4);

                int wd = scvm.ColumnCollection.WDComp(testDate);
                Console.WriteLine("wd col " + wd);
                int upperwsidx = scvm.ColumnCollection.UpperWSComp(testDate);
                int lowerwsidx = scvm.ColumnCollection.LowerWSComp(testDate);
                int datetimeidx = scvm.ColumnCollection.DateIndex;

                Console.WriteLine("upper ws " + upperwsidx);
                Console.WriteLine(" lower ws " + lowerwsidx);



                SessionColumn upperws = (SessionColumn)scvm.ColumnCollection[upperwsidx];
                SessionColumn lowerws = (SessionColumn)scvm.ColumnCollection[lowerwsidx];

                List<int> idx = new List<int> { upperwsidx, lowerwsidx, wd };
                ShearCoincidentRows r = new ShearCoincidentRows(idx, scvm.DownloadedData[0]);

                TimeSpan t = new TimeSpan();
                DateTime start = DateTime.Now;

                coincidentlist = r.GetRows();

                t = DateTime.Now - start;
                Console.WriteLine(" time for coincident rows: " + t);

                AxisFactory factory = new AxisFactory();
                IAxis wdaxis = factory.CreateAxis(10);
                wdaxis.SessionColIndex = wd;

                factory = new AxisFactory();
                IAxis houraxis = factory.CreateAxis(AxisType.Hour);
                houraxis.SessionColIndex = datetimeidx;

                IAxis monthaxis = factory.CreateAxis(AxisType.Month);
                monthaxis.SessionColIndex = datetimeidx;


                Console.WriteLine(" coincident list " + coincidentlist.Count);
                //ISessionColumn uppeWS=collection .
                AlphaFactory afactory = new AlphaFactory();
                List<AbstractAlpha> alphaCollection = new List<AbstractAlpha>()
           {
                
                {afactory .CreateAlpha (coincidentlist, upperws, lowerws, monthaxis, houraxis)},
               // {afactory .CreateAlpha (coincidentlist, upperws, lowerws, wdaxis, houraxis)},
                //{afactory .CreateAlpha (coincidentlist, upperws, lowerws, wdaxis, monthaxis)}

                     
           };

                alphaCollection.ForEach(c => c.CalculateAlpha());

                //get source data
                MergedWSFactory mergeFactory = new MergedWSFactory();
                AbstractMergeWS ws = mergeFactory.CreateMergedWS();
                DataTable tbl = GetTable();


                Dictionary<int, Dictionary<double, List<double>>> param = new Dictionary<int, Dictionary<double, List<double>>>()
           {
              {2,new Dictionary<double,List<double>>(){ {lowerws.Height , WindART.Utils.ExtractDataTableColumn<double>(lowerwsidx ,bdsv.DownloadedData[0])}}},
              {1,new Dictionary<double,List<double>>(){ {upperws.Height , WindART.Utils.ExtractDataTableColumn<double>(upperwsidx ,bdsv.DownloadedData[0])}}}
              
           };
                double[,] result = ws.Merge(param);

                AbstractDeriveWS derived = new DerivedWSFactory().CreateDerivedWSObject();

                //List<double> newWS=derived.DeriveNewWS(collections[0], alphaCollection[0], result, bdsv.DownloadedData[0], 80.0);
                //Console.WriteLine(" source ws count: " + result.GetLength(1) + " new ws count: " + newWS.Count);

                List<double> newWS = derived.DeriveNewWS(scvm.ColumnCollection , alphaCollection[0], result, scvm.DownloadedData[0], htToShearTo );
                Console.WriteLine(" source ws count: " + result.GetLength(1) + " new ws count: " + newWS.Count);
                tempCommonHtSet.Add(scvm.DisplayName, newWS);

                //newWS = derived.DeriveNewWS(collections[0], alphaCollection[2], result, bdsv.DownloadedData[0], 80.0);
                //Console.WriteLine(" source ws count: " + result.GetLength(1) + " new ws count: " + newWS.Count);
           
            }

            tempCommonHtSet.Keys.ToList().ForEach(c => Console.WriteLine(c));


        }
        [Test]
        public void CompareFiles()
        {
            getTestData();

            bool passes=true;
            double out1 = 0.0;
            double out2 = 0.0;

            for (int i = 0; i < shearTestGrids["0302Answers.csv"].Rows.Count; i++)
            {
                foreach (DataColumn dc in shearTestGrids["0302Answers.csv"].Columns)
                {
                    if (dc.ColumnName != "DateTime")
                    {
                        if (Double.TryParse(shearTestGrids["0302Answers.csv"].Rows[i][dc.ColumnName].ToString(), out out1) &
                            Double.TryParse(shearTestGrids["0302MultiConfigResults.csv"].Rows[i][dc.ColumnName].ToString(), out out2))
                        {
                            if (Math.Abs(out1 - out2) > .01)
                            {
                                passes = false;
                                Console.WriteLine(shearTestGrids["0302Answers.csv"].Rows[i][0].ToString() + "    "
                                    + Double.Parse(shearTestGrids["0302MultiConfigResults.csv"].Rows[i][dc.ColumnName].ToString() + "   " +
                                    shearTestGrids["0302MultiConfigResults.csv"].Rows[i][dc.ColumnName].ToString()));
                            }
                            //else
                            //{
                            //    Console.WriteLine(dc.ColumnName + " " + out1.ToString() + "  " + out2.ToString());
                            //}

                        }
                        else
                        {
                            Console.WriteLine(shearTestGrids["0302Answers.csv"].Rows[i][0].ToString() +  " did not parse " + " Answer Value:" + shearTestGrids["0302Answers.csv"].Rows[i][dc.ColumnName].ToString() + "  "
                                + " MultiConfig value: " + shearTestGrids["0302MultiConfigResults.csv"].Rows[i][dc.ColumnName].ToString());
                        }
                    }
                }
            }

            if (!passes) Assert.Fail();

        }
  
    }
}
