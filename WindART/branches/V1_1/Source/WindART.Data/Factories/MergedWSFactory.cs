using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class MergedWSFactory
    {
        public AbstractMergeWS CreateMergedWS()
        {
            return new MergeWS();
        }
    }
}
