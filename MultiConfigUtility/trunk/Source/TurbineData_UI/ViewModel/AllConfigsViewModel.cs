using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Input;
using System.Linq;
using System.Text;
using WindART;

namespace Analysis_UI
{
    public class AllConfigsViewModel : ViewModelBase
    {
        

        ObservableCollection<SingleConfigViewModel> _configs;
        public ObservableCollection<SingleConfigViewModel> Configs
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
                
                _configs = value;
                
            }
        }

        

        void OnConfigsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (SingleConfigViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (SingleConfigViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }

        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            SingleConfigViewModel workspace = sender as SingleConfigViewModel;
            workspace.Dispose();
            this.Configs.Remove(workspace);
        }

        public AllConfigsViewModel(List<SessionColumnCollection> Collections)
        {
            foreach (SessionColumnCollection collection in Collections)
            {
                SingleConfigViewModel sccv = new SingleConfigViewModel(collection);
                if(!Configs.Contains(sccv))
                Configs.Add(sccv);
            }
        
        }
        public AllConfigsViewModel()
        {
            //SingleConfigViewModel sccv = new SingleConfigViewModel();
            //if (!Configs.Contains(sccv))
            //    Configs.Add(sccv);
        }

    }
}    
