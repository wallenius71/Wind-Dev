﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using System.ComponentModel;
using WindART.DAL;
using TurbineDataUtility.Model;
using System.Data;

namespace Analysis_UI
{
    public abstract class BaseDataSourceView:ViewModelBase
    {

#region fields

        protected ObservableCollection<TagManager> _tagManagers;
        protected RelayCommand _downloadCommand;
        protected RelayCommand _setOutputFileLocationCommand;
        
        protected DataRepository  data;
        protected bool _isDownloading;
        protected BackgroundWorker worker;
        protected string _outputFileLocation;
        protected ObservableCollection<DataTable> _downloadedData = new ObservableCollection<DataTable>();
        protected string filename;
       
        
#endregion 

#region properties
        public DateTime _start;
        public DateTime _end;
        private bool _compressed;
        private bool _sampled;
        private bool _std;
        private bool _max;
        private bool _min; 

        public virtual DateTime start
        {
            get { return _start; }
            set { _start = value;
            OnPropertyChanged("start");
            }
        }
        public virtual DateTime end
        {
            get { return _end; }
            set { _end = value;
            OnPropertyChanged("end");
            }
        }
        public virtual bool Compressed
        {
            get { return _compressed; }
            set { 
                    _compressed = value;
                    OnPropertyChanged("Compressed");
                }
        }
        public virtual bool Sampled
        {
            get { return _sampled; }
            set
            {
                _sampled = value;
                OnPropertyChanged("Sampled");
            }
        }
        public virtual bool Std
        {
            get { return _std; }
            set
            {
                _std = value;
                OnPropertyChanged("Std");
            }
        }
        public virtual bool Max
        {
            get { return _max; }
            set
            {
                _max = value;
                OnPropertyChanged("Max");
            }
        }
        public virtual bool Min
        {
            get { return _min; }
            set
            {
                _min = value;
                OnPropertyChanged("Min");
            }
        }
        public string OutPutFileLocation
        {
            get
            {
                return _outputFileLocation;
            }
            set
            {
                _outputFileLocation = value;
                OnPropertyChanged("OutPutFileLocation");
            }
        }
        public ObservableCollection<TagManager> TagManagers
        {
            get
            {
                if (_tagManagers == null)
                {
                    _tagManagers = new ObservableCollection<TagManager>();
                }
                return _tagManagers;

            }

            set
            {
                _tagManagers = value;
            }
        }
        public abstract ObservableCollection<DataTable> DownloadedData 
        { get; 
            set; }
        
        


 #endregion 
        public ICommand SetOutputFileLocationCommand
        {
            get
            {
                if (_setOutputFileLocationCommand == null)
                    _setOutputFileLocationCommand = new RelayCommand(param => this.SetOutputFileLocation());

                return _setOutputFileLocationCommand;
            }
        }

        #region methods

        protected DataTable CreateDateTimeDataTable(TagManager  tagmanager)
        {
            DataTable thisData=new DataTable();
            
            
            //add datetime col 
            if (!thisData.Columns.Contains("DateTime"))
            {
                DataColumn newCol = new DataColumn();
                newCol.DataType = typeof(DateTime);
                newCol.ColumnName = "DateTime";
                thisData.Columns.Add(newCol);
            
            }
            List<DateTime> DateList=new List<DateTime> ();
            //add columns that are present in the downloaded set
            foreach (Tag tag in tagmanager.Tags)
                { 
                    if(tag.Equals (tagmanager.Tags.First()))
                     DateList= GetExpectedSequence(tag.StartDate , tag.EndDate , TimeSpan.FromMinutes (10));
                    
                    string AddColName = tag.TagName.Replace(".", "_");
                    if (!thisData.Columns.Contains(AddColName))
                    {
                        DataColumn newCol = new DataColumn();
                        newCol.DataType = typeof(string);
                        newCol.ColumnName = AddColName;
                        thisData.Columns.Add(newCol);

                    }
                }
                //add the new column if it does not exist
            
            foreach (DateTime dt in DateList)
            {
                //add a new row and the date no mattter what
                DataRow newRow = thisData.NewRow();
                    newRow["DateTime"] = dt;
                    
                foreach (Tag tag in tagmanager.Tags)
                {
                            //if we have data for the tag matching the date in the datesequence add it otheriwise add missing val
                    string tagname = tag.TagName.Replace(".", "_");
                    string AddVal = string.Empty;
                            
                    if (tag.Data !=null && tag.Data.ContainsKey(dt))
                            {
                                AddVal = tag.Data[dt].ToString();
                                

                            }
                            else
                                AddVal = "-9999";

                      if(thisData.Columns.Contains (tagname))
                      newRow[tagname] = AddVal;
                                       
                        
                    
                }
               
                thisData.Rows.Add(newRow);
                
            }
            thisData.AcceptChanges();
            return thisData;

        }
        protected List<DateTime> GetExpectedSequence(DateTime first, DateTime last, TimeSpan interval)
        {

            List<DateTime> result = new List<DateTime>();
            DateTime workdate = first;
            result.Add(workdate);
            while (workdate <= last)
            {
                workdate = workdate + interval;
                result.Add(workdate);
            }
            
            return result;


        }
        
        protected void SetOutputFileLocation()
        {
            OutPutFileLocation = Utils.GetFolder();
        }
        public virtual bool CanOutputFile()
        {
             return true;
            
        }

        #endregion


    }
}
