using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows;

namespace WindART
{
   public  class ShearedWS:AbstractShearedWindSpeed,INotifyPropertyChanged 
    {
       private SessionColumnCollection _collection;
       private DateTime _dateStart;
       private DateTime _dateEnd;
       private AbstractAlpha _alpha;
       private double _height;
       private double[,] _sourceWS;
       private Dictionary<int, SessionColumn> _shearFrom = new Dictionary<int, SessionColumn>();
       
       
       int _index;
       
       

       public ShearedWS(SessionColumnCollection columnCollection)
       {
           ColumnCollection = columnCollection;
           DateStart = columnCollection.DataSetStart;
           DateEnd = columnCollection.DataSetEnd;
       }

       

       public ShearedWS()
       {
       }

       
       public DateTime DateStart
       {
           get
           {
               if (_dateStart == null)
               {
                   _dateStart = _collection.DataSetStart;
                   return _dateStart;
               }
               return _dateStart;
          }
           set {
               if (value < _collection.DataSetStart)
               {
                   MessageBox.Show("Start date can not preceed config start date");

               }
               else
               {
                   _dateStart = value;
                   OnPropertyChanged("DateStart");
               }
           }
       }
       public DateTime DateEnd
       {
           get
           {
               if (_dateEnd == null)
               {
                   _dateEnd = _collection.DataSetEnd;
                   return _dateEnd;
               }
               return _dateEnd;
           }
           set
           {
               if (value < _collection.DataSetEnd)
               {
                   MessageBox.Show("End date can not exceed config end date");

               }
               else
               {
                   _dateEnd = value;
                   OnPropertyChanged("DateEnd");
               }
           }
       }
       public AbstractAlpha Alpha
       {
           get
           {
               return _alpha;
           }
           set
           {
               _alpha = value;
               OnPropertyChanged("Alpha");
           }
       }
       public double ShearHt
       {
           get { return _height; }
           set
           {
               _height = value;
               OnPropertyChanged("ShearHt");
           }
       }
       public Dictionary<int, SessionColumn> ShearFrom
       {
           get { return _shearFrom; }
           set { 
                   _shearFrom=value;
                    OnPropertyChanged("ShearFrom");
               }
       }
       public int Index
       {
           get { return _index; }
           set 
           { 
               _index = value;
               OnPropertyChanged("Index");
           }
       }
       public SessionColumnCollection ColumnCollection
       {
           get { return _collection; }
           set
           {
               _collection = value;
               DateStart = _collection.DataSetStart;
               DateEnd = _collection.DataSetEnd;
               OnPropertyChanged("ColumnCollection");
           }
       }
      
       
       
       public override void CreateNewWS()
       {
           _sourceWS = MergeWS();
           DerivedWSFactory WsFactory = new DerivedWSFactory();
           _deriveWSStrategy  = WsFactory.CreateDerivedWSObject();

           //since alpha could be from different dataset colindexes should not be coupled with alpha
           //for now set the alpha axis session column indices to the dataset at hand

           if (_height!=default(double) & _alpha !=null)
           {
                
                string newHeightName=_height.ToString() + "_" + _alpha.Name + "_WSAvg" + getShearFromHeights ();
                lock (_collection.Data)
                {
                    //create new ws and add to session data 
                    Utils.AddColtoDataTable<double>(newHeightName, _deriveWSStrategy.DeriveNewWS(_collection, _alpha, _sourceWS, _collection.Data, _height), _collection.Data);
                }
               //add the column definition and config 
               Index=ColumnCollection.Data.Columns[newHeightName].Ordinal;
               ColumnCollection .Columns.Add(new SessionColumn (_index, newHeightName ){ColumnType =SessionColumnType.WSAvgShear ,IsCalculated =true,Height=_height });
               
               //calculate recovery 
               ColumnCollection.CalculateRecoveryRate(_index);

             }
       }
       private string getShearFromHeights()
       {
           string result="_ShrdFrom_";
           for (int i = 1; i <= ShearFrom.Count; i++)
           {    
               
                result+= ShearFrom[i].Height + "m_";
                if (i < ShearFrom.Count) result += "->";
           }
           return result;
       }
       private double[,] MergeWS()
       {
           //uses the upper and lower ws comps found in the collection and merges them to create a source windspeed 
           //set for applying the alpha to. 
           MergedWSFactory mergeFactory = new MergedWSFactory();
           AbstractMergeWS ws = mergeFactory.CreateMergedWS();
           
           var param = GetMergeWSParam();

           double[,] result = ws.Merge(param);
           return result;
       }
       private Dictionary<int, Dictionary<double, List<double>>> GetMergeWSParam()
       {
           Dictionary<int, SessionColumn> colOrder = new Dictionary<int, SessionColumn>();
           //assemble parameters for merge ws conditionally 
           if (_shearFrom.Values.Contains(null))
           {
               SessionColumn DrvUpperws = (SessionColumn)_collection[_collection.UpperWSComp(_dateStart)];
               SessionColumn DrvLowerws = (SessionColumn)_collection[_collection.LowerWSComp(_dateStart)];
                colOrder.Add( 1, DrvUpperws);
                colOrder.Add(2, DrvLowerws );
                _shearFrom = colOrder;
           }
           else
           {
               colOrder = _shearFrom;
           }


           Dictionary<int, Dictionary<double, List<double>>> param = new Dictionary<int, Dictionary<double, List<double>>>();
           
           foreach(KeyValuePair<int,SessionColumn > kv in _shearFrom)
           {
               param.Add(kv.Key,new Dictionary<double,List<double>>()
               {{kv.Value.Height  , WindART.Utils.ExtractDataTableColumn<double>(kv.Value.ColIndex ,_collection.Data )}});
                                      
           }
           return param;
       }
       #region INotifyPropertyChanged Members
       
       public event PropertyChangedEventHandler PropertyChanged;

       protected void OnPropertyChanged(string PropertyName)
       {
           if (PropertyChanged != null)
               PropertyChanged(this, new PropertyChangedEventArgs(PropertyName));
       }

       #endregion
    }
}
