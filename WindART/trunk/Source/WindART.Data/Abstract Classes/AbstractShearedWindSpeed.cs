using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace WindART
{
    public abstract class AbstractShearedWindSpeed
    {
        protected AbstractDeriveWS _deriveWSStrategy;

        
        public abstract void CreateNewWS();
        
        
        
    }
}
