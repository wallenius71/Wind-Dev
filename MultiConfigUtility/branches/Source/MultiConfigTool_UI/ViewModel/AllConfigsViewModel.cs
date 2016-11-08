using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Collections;
using System.Windows.Input;
using System.Linq;
using WindART;
using System.IO;
using System.Windows.Threading;
using System.Data;
using System.Windows.Forms;

namespace MultiConfigTool_UI
{
    public class AllConfigsViewModel : ViewModelBase
    {

        RelayCommand _addDerivedWSCommand;
        RelayCommand _removeDerivedWSCommand;
        RelayCommand _outPutUnifiedCommand;

        

        ObservableCollection<SingleConfigViewModel  > _configs;
        ObservableCollection<SingleConfigShearViewModel > _singleConfigDerivedWS;
        ObservableCollection<SingleConfigViewModel> _possibleConfig;
        ObservableCollection<AbstractAlpha> _alphaCollection;
        UnifiedConfigSetUpViewModel _unifiedSetUp;
        ObservableCollection<double> _possibleUnifiedHeights;
        int _tabControlIndex;
        
        
       
        
        public ObservableCollection<SingleConfigViewModel > Configs
        {
            get
            {
                if (_configs == null)
                {
                    
                    _configs = new ObservableCollection<SingleConfigViewModel>();
                    _configs.CollectionChanged += this.OnConfigsChanged;
                    
                    

                }
                return _configs;
            }

            set
            {
                if (_configs.Count == 0)
                {
                    _configs.Add(new SingleConfigViewModel());
                    _configs = value;
                }
                else
                {
                    _configs = value;
                   
                }
                _configs.CollectionChanged += this.OnConfigsChanged;
            }
        }
        public ObservableCollection<SingleConfigViewModel> PossibleConfig
        {
            get
            {
                if (_possibleConfig == null)
                {
                    _possibleConfig = new ObservableCollection<SingleConfigViewModel>();
                    

                }
                return _possibleConfig;
            }

            set
            {
                _possibleConfig = value;
                OnPropertyChanged("PossibleConfig");
                
            }
        }
        public ObservableCollection<AbstractAlpha> AlphaCollection
        {
            get
            {
                if (_alphaCollection == null)
                {
                    _alphaCollection = new ObservableCollection<AbstractAlpha>();


                }
                return _alphaCollection;
            }

            set
            {
                _alphaCollection = value;

            }
        }
        public ObservableCollection<SingleConfigShearViewModel> SingleConfigDerivedWS
        {
            get
            {
                
                return _singleConfigDerivedWS;
            }
            set
            {
                _singleConfigDerivedWS = value;
                _singleConfigDerivedWS.CollectionChanged += this.OnSingleConfigDerivedWSChanged;
                OnPropertyChanged("SingleConfigDerivedWS");
            }
        }
        public UnifiedConfigSetUpViewModel UnifiedSetUp
        {
            get
            {
                if (_unifiedSetUp == null) _unifiedSetUp = new UnifiedConfigSetUpViewModel(this);
                return _unifiedSetUp;
            }

            set
            {
                _unifiedSetUp = value;
                OnPropertyChanged("UnifiedSetUp");
            }
        }
        public ObservableCollection<double> PossibleUnifiedHeights
        {
            get
            {
                if (_possibleUnifiedHeights == null)
                    _possibleUnifiedHeights = new ObservableCollection<double>();
                return _possibleUnifiedHeights;
            }
            set
            {
                _possibleUnifiedHeights = value;
                OnPropertyChanged("PossibleUnifiedHeights");
            }
        }
        public int TabControlIndex
        {
            get {return _tabControlIndex ;}
            set {_tabControlIndex =value;
                 OnPropertyChanged ("TabControlIndex");
            }
        }

        
        //unified set up properties
       
                        
        public AllConfigsViewModel(List<SessionColumnCollection> Collections)
        {
            foreach (SessionColumnCollection collection in Collections)
            {
                SingleConfigViewModel sccv = new SingleConfigViewModel(collection);
                if(!Configs.Contains(sccv))
                Configs.Add(sccv);
                
            }
            UnifiedSetUp = new UnifiedConfigSetUpViewModel(this);
            SingleConfigDerivedWS=new ObservableCollection<SingleConfigShearViewModel> (){{new SingleConfigShearViewModel()}};
        }
        public AllConfigsViewModel(SingleConfigViewModel scvm)
        {

            if (Configs.Count == 0)
            {
                Configs.Add(new SingleConfigViewModel());
                Configs[0] = scvm;
            }
            else
            {
                if (!Configs.Contains(scvm))
                    Configs.Add(scvm);
            }
                UnifiedSetUp = new UnifiedConfigSetUpViewModel(this);
                SingleConfigDerivedWS = new ObservableCollection<SingleConfigShearViewModel>();
                SingleConfigDerivedWS.Add(new SingleConfigShearViewModel());

                
            }
        public AllConfigsViewModel()
        {
            UnifiedSetUp = new UnifiedConfigSetUpViewModel(this);

        }


        public ICommand AddDerivedWSCommand
        {
            get
            {
                if (_addDerivedWSCommand == null)
                    _addDerivedWSCommand = new RelayCommand(param => this.AddDerivedWS(), param => this.CanAddDerivedWS());

                return _addDerivedWSCommand;
            }
        }
        public ICommand RemoveDerivedWSCommand
        {
            get
            {
                if (_removeDerivedWSCommand == null)
                    _removeDerivedWSCommand = new RelayCommand(param => this.RemoveDerivedWS(), param => this.CanRemoveDerivedWS());

                return _removeDerivedWSCommand;
            }
        }
        

        void OnConfigsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (SingleConfigViewModel workspace in e.NewItems)
                {
                    
                    if (Configs.Count <2)
                        TabControlIndex = 0;
                    workspace.RequestClose += this.OnWorkspaceRequestClose;
                    workspace.CompositesUpdated += this.OnCompositeUpdated;
                    workspace.AlphaCollectionUpdated += this.OnAlphaUpdated;
                    workspace.AlphaRemoved += this.OnAlphaRemoved;
                }

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (SingleConfigViewModel workspace in e.OldItems)
                {
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
                    workspace.CompositesUpdated -= this.OnCompositeUpdated;
                    workspace.AlphaCollectionUpdated -= this.OnAlphaUpdated;
                    workspace.AlphaRemoved -= this.OnAlphaRemoved;
                }
        }
        void OnSingleConfigDerivedWSChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (SingleConfigShearViewModel workspace in e.NewItems)
                {
                    workspace.DerivedWSCollectionUpdated += this.scsvm_DerivedWSCollectionUpdated;
                    
                }

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (SingleConfigShearViewModel workspace in e.OldItems)
                {
                    workspace.DerivedWSCollectionUpdated -= this.scsvm_DerivedWSCollectionUpdated;
                    
                }
        }
        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            SingleConfigViewModel workspace = sender as SingleConfigViewModel;
            workspace.Dispose();
            this.Configs.Remove(workspace);
            PossibleConfig.Remove(workspace);
            UnifiedSetUp.ConfigHeaderRow.Remove(workspace.DisplayName);
            if (Configs.Count == 0) AlphaCollection = null;
            CreateUnifiedDataTable();
        }
        void OnCompositeUpdated(SingleConfigViewModel config)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render, new Action
                       (
                            delegate()
                            {
                                
                                PossibleConfig.Add(config );
                                CreateUnifiedDataTable();


                            }));
            
        }
        void OnAlphaUpdated(AbstractAlpha alpha)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render, new Action
                       (
                            delegate()
                            {
                                if (AlphaCollection.Where(
                                       c =>c.SourceDataSet==alpha.SourceDataSet 
                                    && c.AlphaFilter ==alpha.AlphaFilter 
                                    && c.Xaxis.BinWidth ==alpha.Xaxis.BinWidth
                                    && c.Yaxis.BinWidth ==alpha.Yaxis.BinWidth 
                                    && c.Xaxis.AxisType == alpha.Xaxis.AxisType 
                                    && c.Yaxis.AxisType == alpha.Yaxis.AxisType).Count()==0)
                                    
                                    AlphaCollection.Add(alpha);
                                


                            }));
        }
        void OnAlphaRemoved(AbstractAlpha alpha)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render, new Action
                       (
                            delegate()
                            {
                                
                                    AlphaCollection.Remove (alpha);



                            }));
        }
        
        void AddDerivedWS()
        {
            if (SingleConfigDerivedWS == null)
                SingleConfigDerivedWS = new ObservableCollection<SingleConfigShearViewModel>();
            SingleConfigShearViewModel scsvm = new SingleConfigShearViewModel();
            SingleConfigDerivedWS.Add(scsvm); 
        }
        bool CanAddDerivedWS()
        {
            return true;
        }
        void RemoveDerivedWS()
        {
            
            SingleConfigDerivedWS.RemoveAt(SingleConfigDerivedWS.IndexOf (SingleConfigDerivedWS.Last()));
        }
        bool CanRemoveDerivedWS()
        {
            if (SingleConfigDerivedWS != null)
            {
                if (SingleConfigDerivedWS.Count() > 0)
                    return true;
                else
                    return false;
            }
            return false;
        }
        void CreateUnifiedDataTable()
        {
                        
           List<double> allHeights = new List<double>();
            List<double> finalHeights = new List<double>();
            //add an initial list to unified columns
            if (Configs.Count == 0 )
            {
                PossibleUnifiedHeights.Clear();
                UnifiedSetUp.HubHeightRow = null;
                UnifiedSetUp.UpperHeightRow = null;
                UnifiedSetUp.LowerHeightRow = null;
                UnifiedSetUp.ConfigHeaderRow = null;
                return;
            }

            
            
            foreach(SessionColumn sc in Configs[0].ColumnCollection.Columns)
                {
                    if (sc.IsCalculated | (sc.IsComposite & sc.ColumnType==SessionColumnType.WSAvg ))
                    {
                        double height = sc.Height;
                        bool foundit=true;
                        foreach (SingleConfigViewModel scvm in Configs)
                        {
                            var res=scvm.ColumnCollection.Columns.Where(c=>c.IsCalculated || c.IsComposite ).ToList().ConvertAll<SessionColumn> 
                                (c=>(SessionColumn)c).Select(c=>c.Height  );
                           if( !res.Contains (height ))
                           {
                               foundit=false;
                           }
                           
                        }
                        if (foundit)
                        {
                            
                            allHeights.Add(height);
                        }
                    }
                }
            
            
            PossibleUnifiedHeights=new ObservableCollection<double> ( allHeights.Distinct ());
            if (UnifiedSetUp != null)
            {
                if (UnifiedSetUp.HubHeightRow != null)
                {
                    foreach (UnifiedHeightViewModel u in UnifiedSetUp.HubHeightRow)
                    {
                        u.getPosibleColumns();
                    }
                }

                if (UnifiedSetUp.UpperHeightRow != null)
                {
                    foreach (UnifiedHeightViewModel u in UnifiedSetUp.UpperHeightRow)
                    {
                        u.getPosibleColumns();
                    }
                }
                if (UnifiedSetUp.LowerHeightRow != null)
                {
                    foreach (UnifiedHeightViewModel u in UnifiedSetUp.LowerHeightRow)
                    {
                        u.getPosibleColumns();
                    }
                }
            }
        }
        void scsvm_DerivedWSCollectionUpdated(SingleConfigViewModel   scvm)
        {
            this.Dispatcher.Invoke(DispatcherPriority.Render, new Action
                       (
                            delegate()
                            {
                                CreateUnifiedDataTable();


                            }));
        }


    }
}    
