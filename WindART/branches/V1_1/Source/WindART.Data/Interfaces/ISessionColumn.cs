using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace WindART
{
    public interface ISessionColumn
    {
        
        ObservableCollection <ISessionColumn> ChildColumns 
        { get; 
           set; }
        string DisplayName
        {
            get;
            
        }
        string Units
        {
            get;
            
            set;
            
        }
        ObservableCollection<ISensorConfig> Configs
        {
            get;
        }
        SessionColumnType ColumnType
        {
            get;
            set;
        }
        int PossibleRecords
        {
            get;
            
            set;
            

        }
        int ValidRecords
        {
            get;

            set;

        }
        double RecoveryRate
        {
            get;
            
            set;
            
        }
        string ColName
        {
            get;
            set;


        }
        int ColIndex
        {
            get;
            
        }
        bool IsComposite
        { get; set; }
        bool IsCalculated { get; set; }

        //method 

        ISensorConfig getConfigAtDate(DateTime date);
        void addConfig(ISensorConfig config);
        
        

    
    }
}
