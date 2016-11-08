using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TurbineDataUtility.Model
{
    public interface ITagTypeAssigner
    {
        tagType AssignType(Tag tag);
    }
}
