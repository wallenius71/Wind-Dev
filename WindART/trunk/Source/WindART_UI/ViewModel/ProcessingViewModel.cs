using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WindART;

namespace WindART_UI
{
    public class ProcessingViewModel:ViewModelBase
    {
        AllConfigsViewModel _workspace;
        ViewModelBase _lowerWorkspace;
        public AllConfigsViewModel  WorkSpace
        {
            get
            {
                return _workspace;
            }
            set
            {
                _workspace= value;
                OnPropertyChanged("WorkSpace");
            }
        }
        public ViewModelBase LowerWorkSpace
        {
            get
            {
                return _lowerWorkspace;
            }
            set
            {
                _lowerWorkspace = value;
                OnPropertyChanged("LowerWorkSpace");
            }
        }
        public ProcessingViewModel()
        {
            WorkSpace = new AllConfigsViewModel();
            SingleConfigViewModel scvm = new SingleConfigViewModel(null);

            WorkSpace.Configs.Add(scvm);

        }

        public ProcessingViewModel(List<ISessionColumnCollection > collections)
        {
            WorkSpace = new AllConfigsViewModel(collections );
        }

    }
}
