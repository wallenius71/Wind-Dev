using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART;
using System.Data;
using System.Windows.Input;

namespace MultiConfigTool_UI
{
    public class ShearGridViewModel :  ViewModelBase
    {
        RelayCommand _closeCommand;
        private Alpha  _gridCollection;
        
        public ShearGridViewModel(Alpha  gridcollection, string gridname)
        {
            DisplayName = gridname;
            GridCollection = gridcollection;
        }
        public event EventHandler RequestClose;
        public ICommand CloseCommand
        {
            get
            {
                if (_closeCommand == null)
                    _closeCommand = new RelayCommand(param => this.OnRequestClose());

                return _closeCommand;
            }
        }

        void OnRequestClose()
        {
            EventHandler handler = this.RequestClose;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
        public Alpha GridCollection
        {
            get
            {
                return _gridCollection;
            }
            set
            {
                _gridCollection = value;
                OnPropertyChanged("GridCollection");
            }
        }
        
        public DataTable UpperAvg
        {
            get { 
                    DataTable dt=Utils.MakeDataTable<double>(GridCollection.UpperAvg);
                    return dt;
                 }
        }
        public DataTable LowerAvg
        {
            get { return Utils.MakeDataTable<double>(GridCollection.LowerAvg); }
        }
        public DataTable Alpha
        {
            get { return Utils.MakeDataTable<double>(GridCollection.Alpha); }
        }

        public DataTable UpperAvgCount
        {
            get { return Utils.MakeDataTable<int>(GridCollection.UpperAvgCount); }
        }

        public DataTable LowerAvgCount
        {
            get { return Utils.MakeDataTable<int>(GridCollection.LowerAvgCount); }
        }
        
    }
}
