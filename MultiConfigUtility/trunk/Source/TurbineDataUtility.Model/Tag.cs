using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlTypes;
using WindART;


namespace TurbineDataUtility.Model
{
    public class Tag
    {
        string _tagName;
        tagType _type;
        int _tagIndex;
        DateTime _startDate;
        DateTime _endDate;
        SortedDictionary<DateTime, double> _data;
        SortedDictionary<DateTime, string> _errors;
        SortedDictionary<DateTime, bool> _dataInventory=new SortedDictionary<DateTime,bool> ();
        List<InventoryDateRanges> _missingDateRanges = new List<InventoryDateRanges>();


        public Tag()
        {
        }
        public Tag(tagType type)
        {
            _type=type;
        }
        public Tag(string tagname, tagType type)
        {
            _tagName = tagname;
            _type = type;
        }
        public Tag(string tagname,DateTime startdate, DateTime enddate)
        {
            _tagName = tagname;
            _startDate = startdate;
            _endDate = enddate;

        }

        public SortedDictionary<DateTime,string> Errors
        {
            get { return _errors; }
            set
            {
                _errors = value;
            }
        }
        public SortedDictionary<DateTime, double> Data
        {
            get
            {
                return _data;
            }
            set
            {
                _data = value;
                GetMissingRanges();
                
            }
        }
        public List<InventoryDateRanges> DataInventory
        {
            get
            {
                return _missingDateRanges;
            }

            
            
        }

        protected void GetMissingRanges()
        {
            
                //create full date sequence here 
            
            if (_data == null) return;

                _data = FillMissingDates(_data);


                DateTime last = _data.Keys.Max();
                DateTime first = _data.Keys.Min();

                List<InventoryDateRanges> missingRanges = new List<InventoryDateRanges>();
                if (_data != null)
                {
                    int MissCount = 0;
                    bool previousMissing = false;
                    DateTime startdate = first;
                    DateTime prevDate = default(DateTime);
                    foreach (KeyValuePair<DateTime, double> kv in _data)
                    {
                        double thisVal = kv.Value;
                        DateTime thisDate = kv.Key;
                        
                        //Console.WriteLine(kv.Key + " missing: " + (thisVal > -8000.00) + " previous missing " + previousMissing);
                        //Console.WriteLine("    this " + thisVal + " prev " + prevVal);

                        if (thisVal > -8000.00)
                        {
                            if (previousMissing )
                            {
                                //end of missing range 

                                InventoryDateRanges singleMissingRange = new InventoryDateRanges();
                                singleMissingRange.StartDate = startdate;
                                singleMissingRange.Missing = true;
                                singleMissingRange.EndDate = prevDate;
                                singleMissingRange.IntervalCount = MissCount;
                                missingRanges.Add(singleMissingRange);

                                //and beginning of good range
                                
                                MissCount = 1;
                                startdate = kv.Key;
                                previousMissing = false;
                                prevDate = thisDate;

                            }
                            else
                            {
                                if (kv.Key == last)
                                {
                                    //end of missing range 

                                    InventoryDateRanges GoodRange = new InventoryDateRanges()
                                    {
                                        Missing = false,
                                        StartDate = startdate,
                                        EndDate = prevDate,
                                        IntervalCount = MissCount
                                    };
                                    //end of the line
                                    missingRanges.Add(GoodRange);
                                    
                                }
                                else
                                {
                                    //normal value
                                    previousMissing = false;
                                    prevDate = thisDate;
                                    MissCount++;
                                }
                            }
                        }
                        else  //current value is missing 
                        {

                            if (previousMissing)
                            {
                                
                                if (kv.Key ==last)
                                {
                                    //end of missing range and end of the line 

                                    InventoryDateRanges singleMissingRange = new InventoryDateRanges();
                                    singleMissingRange.Missing = true;
                                    singleMissingRange.StartDate = startdate;
                                    singleMissingRange.EndDate = prevDate ;
                                    singleMissingRange.IntervalCount = MissCount;
                                    missingRanges.Add(singleMissingRange);
                                    
                                }
                                else
                                {
                                    //continue missing range
                                    MissCount++;
                                    previousMissing = true;
                                    prevDate = thisDate;
                                }
                                

                            }
                            else
                            {

                                //end of good range
                                InventoryDateRanges goodrange = new InventoryDateRanges()
                                {
                                    StartDate =startdate,
                                    EndDate =prevDate ,
                                    Missing =false,
                                    IntervalCount =MissCount                                    
                                    
                                };
                                missingRanges.Add(goodrange);

                                //start of missing range
                                startdate = kv.Key;
                                MissCount=1;
                                previousMissing = true;
                                prevDate = thisDate;
                                

                            }
                        }
                     }
                }
                else
                {
                    //TODO:if there is no data put entire range here 

                }
                //Console.WriteLine(missingRanges.Count + " missing date range objects in final result");
                _missingDateRanges = missingRanges;
            
            
        }
        public DateTime StartDate { get { return _startDate; } set { _startDate = value; } }
        public DateTime EndDate { get { return _endDate; } set { _endDate = value; } }
        public string TagName
        {
            get
            {
                return _tagName;
            }
            set
            {
                _tagName = value;
            }
        }
        public tagType TagType
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }

        }
        public int TagIndex
        {
            get
            {
                return _tagIndex;

            }
            set
            {
                
                    _tagIndex = value;
                
            }
        }
        public string TagNameElement( int section)
        {
            string answer = string.Empty;
            //return the nth tag element specified 
            if (_tagName != null && _tagName.Contains('.'))
            {

                string[] thisArray =  _tagName.Split('.');
                answer = thisArray[section].ToUpper();
            }
            if (answer.Length > 0)
                return answer;
            else
                return string.Empty;
        }

        public string LastTagNameElement()
        {
            string answer = string.Empty;
            //return the nth tag element specified 
            if (_tagName != null && _tagName.Contains('.'))
            {

                string[] thisArray = _tagName.Split('.');
                answer = thisArray[thisArray.Length -1];
            }
            if (answer.Length > 0)
                return answer;
            else
                return string.Empty;
        }
        protected SortedDictionary<DateTime,double> FillMissingDates(SortedDictionary<DateTime,double> originaldata)
        {
            DateListTimeSequence datelist = new DateListTimeSequence(originaldata.Keys.ToList (), _startDate, _endDate);
            foreach (DateTime dt in datelist.GetMissingTimeStamps())
            {
                originaldata.Add(dt, -9999.99);
            }

            return originaldata;
        }
        public override string ToString()
        {
            return this.TagName;
        }
        public double Height { get; set; }
        public double Orientation { get; set; }
        public SqlInt16  SensorType { get; set; }
    }

    public class InventoryDateRanges
    {
        public bool Missing { get; set; }
        public DateTime StartDate {get;set;}
        public DateTime EndDate{get;set;}
        public int IntervalCount{get;set;}

        
    }
}
