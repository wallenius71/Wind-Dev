using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel ;

namespace MultiConfigTool_UI
{
    public class SelectionItem<T> : ViewModelBase
    {
        
        
        private T _selectedItem;
        private bool _isSelected;

        public SelectionItem(T sheetType)
        {

            _selectedItem = sheetType;
            
            
        }

        #region properties

        public bool IsSelected
        {
            get { return _isSelected; }
            set {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
                }
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
        #endregion
        }



    }
    public class SelectionList<T>:ObservableCollection <SelectionItem<T>> 
    {
        public SelectionList(IEnumerable<T> col)
            : base(toSelectionItemEnumerable(col))
        {

        }

        public IEnumerable<T> selectedItems
        {
            get { return this.Where(x => x.IsSelected).Select(x => x.SelectedItem ); }
        }

        public IEnumerable<T> AllItems
        {
            get { return this.Select(x => x.SelectedItem ); }
        }

        private static IEnumerable<SelectionItem<T>> toSelectionItemEnumerable(IEnumerable<T> items)
        {
            List<SelectionItem<T>> list = new List<SelectionItem<T>>();
            foreach (T item in items)
            {
                SelectionItem<T> selectionItem = new SelectionItem<T>(item);
                list.Add(selectionItem);
            }
            return list;
        }

    }
}
