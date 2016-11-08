using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Data;
using WindART.Properties;

namespace WindART
{
    public class ShearCoincidentRows
    {

        DataTable _data;
        List<int> _filteridx;


        public ShearCoincidentRows(List<int> filteridx, DataTable data)
        {
            _data = data;
            _filteridx = filteridx;
        }

        public List<DataRow> GetRows()
        {
                              
        
            int count = 0;
            Expression<Func<DataRow,bool>> build = null;
                      
            foreach (int i in _filteridx)
            {
                
                int j = count;
                if (count == 0)
                {   
                    int ii = i;
                     
                     build = (c => (c.Field<double?>(ii) >= 0));
                }
                else
                {
                    int ii = i;
                    
                    build = build.and(c => (c.Field<double?>(ii) >= 0));
                    
                }

                count++;
                  
            }

            var final = build.Compile();
            var finalresult = _data.AsEnumerable().Where(c=>final(c));
                         
                         

            return finalresult.ToList();     

            
            
        }

        
  
        
    }

    public static class extensions
    {
        public static Expression<TDelegate> AndAlso<TDelegate>(this Expression<TDelegate> left, Expression<TDelegate> right)
        {
            return Expression.Lambda<TDelegate>( Expression.And(left,right), left.Parameters);
        } 

    }
}
