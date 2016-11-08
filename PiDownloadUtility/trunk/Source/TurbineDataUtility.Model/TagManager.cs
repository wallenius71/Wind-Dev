using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurbineDataUtility.Model
{
    public class TagManager
    {
        private List<Tag> _tags;
        public string SetName {get;set;}

        public TagManager(List<Tag> tags)
        {
            _tags = tags;
            if (ProjectList == null)
                CreateProjectList();

        }
        public TagManager(List<Tag> tags, string setname)
        {
            _tags = tags;
            SetName = setname;

        }

        public Tag this[string tagname]
        {
            get
            {
                foreach (Tag tag in _tags)
                {
                    if (tagname == tag.TagName ) return tag;
                }
                return null;
            }
        }

        #region properties

        public List<Tag> Tags
        {
            get
            {
                return _tags;

            }
            private set
            {
                
                _tags = value;
            }
        }
        public Dictionary<string,Project> ProjectList { get; private set; }

        #endregion
        private void CreateProjectList()
        {
            //assign tags to their projects
            Dictionary<string, Project> thisProjList = new Dictionary<string, Project>();
            
            if (_tags != null)
            {
                foreach (Tag t in _tags)
                {
                    
                    
                        string  ThisProject= Utils.GetTagElement(t.TagName, 1);
                        if (ThisProject != string.Empty)
                        {
                            Project thisProj = new Project(ThisProject);

                            if (!thisProjList.Keys.Contains(thisProj.name))
                            {
                                thisProj.tags = new List<Tag>() { t };
                                thisProjList.Add(thisProj.name, thisProj);

                            }
                            else
                            {
                                thisProjList[thisProj.name].tags.Add(t);
                            }
                        }
                        else
                            continue;
                    
                }
            }
            //sort the tags on type
            
            ProjectList = thisProjList;
        }

        
       
    }
}
