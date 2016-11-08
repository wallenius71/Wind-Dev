using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel ;
using WindART;

namespace MultiConfigTool_UI
{
    public class UnifiedHeightViewModel:ViewModelBase 
    {
        ObservableCollection<SessionColumn> _possibleColumns;
        SingleConfigViewModel _scvm;
        AllConfigsViewModel _acvm;
        double _height;

        public SessionColumn SelectedColumn
        {
            get;
            set;
        }
        public ObservableCollection<SessionColumn> PossibleColumns
        {
            get
            {

                if (_possibleColumns == null)
                    getPosibleColumns();
                    return _possibleColumns; 
            }
            set 
            { _possibleColumns = value;
                OnPropertyChanged("PossibleColumns");
            }
        }

        public  void getPosibleColumns()
        {

            var result = from s in _scvm.ColumnCollection.Columns
                         where ((s.IsComposite & s.ColumnType == SessionColumnType.WSAvg) | s.IsCalculated) 
                         select s;
           _possibleColumns =new ObservableCollection<SessionColumn> 
                ( result.DefaultIfEmpty().ToList().ConvertAll<SessionColumn>(c => (SessionColumn)c).Where(c=>_height==c.Height).ToList ());
           if (_possibleColumns.Count==1)
               SelectedColumn = PossibleColumns.First();
           else
               SelectedColumn = null;
           OnPropertyChanged("SelectedColumn");
            OnPropertyChanged("PossibleColumns");

        
        }

         public UnifiedHeightViewModel(SingleConfigViewModel scvm,  AllConfigsViewModel acvm, double height)
         {
             _height = height;
             _scvm = scvm;
             _acvm = acvm;
             _acvm.PossibleUnifiedHeights.CollectionChanged +=new System.Collections.Specialized.NotifyCollectionChangedEventHandler(PossibleUnifiedHeights_CollectionChanged);
             
             OnPropertyChanged("PossibleColumns");


         }

         void PossibleUnifiedHeights_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
         {
             OnPropertyChanged("PossibleColumns");
         }
    }
    }

