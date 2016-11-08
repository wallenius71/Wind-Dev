using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace TurbineDataUtility.Model
{
    public class Project:INotifyPropertyChanged 
    {
        

        public string name{get;private set;}
        public List<Tag> tags { get;  set; }

        
    
        public Project(string projname)
        {
            name = projname;
            tags = new List<Tag>();
        }
        public List<List<Tag>> TagGroups()
        {
            //return different tag types as seperate lists
            return null;

        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void SortByType()
        {

            tags = tags.OrderBy(t => t.TagNameElement (5) + t.LastTagNameElement () ).ToList ();
            OnPropertyChanged("tags");
        }
        
        protected virtual void OnPropertyChanged(string propertyName)
        {

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
    }
}
