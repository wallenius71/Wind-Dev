using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TurbineDataUtility.Model;

namespace TurbineDataUtility.Model
{
    public class TagGroup:List<Tag>
    {
        public string Key { get; set; }
    }
}
