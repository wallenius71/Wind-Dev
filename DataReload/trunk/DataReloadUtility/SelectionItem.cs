using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace DataReloadUtility
{
    public class SelectionItem<T> : ViewModelBase 
    {

        private bool _isSelected;
        private T _selectedItem;

        public SelectionItem(T sheetType)
        {

            _selectedItem = sheetType;
            _isSelected = false;
        }



        public bool IsSelected
        {
            get;
            set;
        }

        public T SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                if (value.Equals(_selectedItem)) return;
                _selectedItem = value;
                OnPropertyChanged("SelectedItem");

            }

        }

        public override string ToString()
        {
            return   _selectedItem.ToString();
        }
    }
}
