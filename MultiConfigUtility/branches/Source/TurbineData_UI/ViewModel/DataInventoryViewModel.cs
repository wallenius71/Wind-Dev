using System;
using System.Collections.Generic;
using System.Collections .ObjectModel ;
using System.Linq;
using System.Text;
using System.Data;
using TurbineDataUtility.Model;
using System.Windows.Input;
using System.IO;

namespace Analysis_UI
{
    public class DataInventoryViewModel:ViewModelBase 
    {
        ObservableCollection<Tag> _tags;
        string _outputFileLocation;
        
        RelayCommand _outputFileCommand;
        
        
        public DataInventoryViewModel(List<Tag> tags)
        {
            if (tags != null)
            {
                Tags = new ObservableCollection<Tag>(tags);
            }
        }

        public ObservableCollection<Tag> Tags
        {
            get { return _tags; }
            set
            {
                _tags = value;
                
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

        

        public ICommand OutputFileCommand
        {
            get
            {
                if (_outputFileCommand == null)
                    _outputFileCommand = new RelayCommand(param => this.OutputFile(), param => this.CanOutputFile());

                return _outputFileCommand;
            }
        }
         
        public virtual bool CanOutputFile()
        {
            return true;

        }
        public void OutputFile()
        {
            OutPutFileLocation = TurbineDataUtility.Model.Utils.GetFolder();
            try
            {
                string filename = OutPutFileLocation + @"\" + Tags[0].TagName.Substring(0, 16) + "_" + "_MissingDates_" + DateTime.Now.ToShortDateString().Replace(@"/", "") + ".csv";
                // Create the CSV file to which grid data will be exported.
                StreamWriter sw = new StreamWriter(filename, false);
                
                foreach (Tag t in Tags)
                {
                    // Create the CSV file to which grid data will be exported.
                    
                    sw.Write(t.TagName);
                    sw.Write(sw.NewLine);
                        foreach (InventoryDateRanges dr in t.DataInventory)
                        {
                            if (dr.Missing)
                            {
                                sw.Write(",");
                                sw.Write(dr.StartDate);
                                sw.Write(",");
                                sw.Write(dr.EndDate);
                                sw.Write(",");
                                sw.Write(dr.IntervalCount);
                                sw.Write(sw.NewLine);
                            }


                        }
                      

                    }
                sw.Close();
                }
            
            catch (Exception e)
            {
                throw e;
            }
        }
        

        
    }

}
