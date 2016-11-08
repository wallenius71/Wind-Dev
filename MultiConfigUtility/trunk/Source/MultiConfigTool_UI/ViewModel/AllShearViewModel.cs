using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace MultiConfigTool_UI
{
    public class AllShearViewModel : ViewModelBase
    {

        
        ObservableCollection<ShearGridViewModel> _shearCollection;

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
            ShearCollection = new ObservableCollection<ShearGridViewModel>();
        }
                
        public ObservableCollection<ShearGridViewModel> ShearCollection
        {
            get
            {

                return _shearCollection;
            }

            set
            {
                
                _shearCollection = value;
                _shearCollection.CollectionChanged += this.OnConfigsChanged;
                OnPropertyChanged("ShearCollection");
                
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
        
       
    }
}    
