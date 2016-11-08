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
    public class AllShearViewModel : ViewModelBase
    {
        

        ObservableCollection<ShearGridViewModel> _shearCollection;
        public ObservableCollection<ShearGridViewModel> ShearCollection
        {
            get
            {
                if (_shearCollection == null)
                {
                    _shearCollection = new ObservableCollection<ShearGridViewModel>();
                    _shearCollection.CollectionChanged += this.OnConfigsChanged;
                    
                }
                return _shearCollection;
            }

            set
            {
                
                _shearCollection = value;
                
            }
        }
        void OnConfigsChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null && e.NewItems.Count != 0)
                foreach (ShearGridViewModel workspace in e.NewItems)
                    workspace.RequestClose += this.OnWorkspaceRequestClose;

            if (e.OldItems != null && e.OldItems.Count != 0)
                foreach (ShearGridViewModel workspace in e.OldItems)
                    workspace.RequestClose -= this.OnWorkspaceRequestClose;
        }
        void OnWorkspaceRequestClose(object sender, EventArgs e)
        {
            ShearGridViewModel workspace = sender as ShearGridViewModel;
            workspace.Dispose();
            this.ShearCollection.Remove(workspace);
        }
        public AllShearViewModel(ObservableCollection<ShearGridViewModel> collection)
        {
            foreach (ShearGridViewModel  sheargridvm in collection)
            {
                
                if(!ShearCollection.Contains(sheargridvm ))
                ShearCollection.Add(sheargridvm);
            }
        
        }
        public AllShearViewModel()
        {
            
        }

    }
}    
