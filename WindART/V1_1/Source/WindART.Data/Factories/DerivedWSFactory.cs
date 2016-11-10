using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
   public  class DerivedWSFactory
    {
       public AbstractDeriveWS CreateDerivedWSObject()
       {
           return new DerivedWS();
       }
    }
}
