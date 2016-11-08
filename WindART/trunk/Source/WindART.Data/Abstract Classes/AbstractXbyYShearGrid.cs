using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public abstract class AbstractXbyYShearGridAlgorithm:ICreateGridAlgorithm 
    {
        protected IAxis _x;
        protected IAxis _y;
        protected double[,] _valuegrid;

        public List<List<SummaryGridColumn>> CreateGrid()
        {
            List<List<SummaryGridColumn>> workgrid = new List<List<SummaryGridColumn>>(_x.AxisValues .Length );
                for (int x = 0; x < _x.AxisValues.Length; x++)
                {
                    //create a row and add it to the grid
                    List<SummaryGridColumn> row = new List<SummaryGridColumn>();

                    for (int y=0; y < _y.AxisValues .Length;y++)
                      {
                          if (x == 0)
                          {
                              row.Add(new SummaryGridColumn(new AxisValueGridColumn(_x, x)));
                          }
                          else
                          {
                              if (y == 0)
                              {
                                  row.Add(new SummaryGridColumn(new AxisValueGridColumn(_y, y)));
                              }
                              else
                              {
                                  row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(_valuegrid[x, y])));
                              }
                              
                          }
                        workgrid.Add(row);
                    }
                }

                return workgrid;
            
        }
        
    }
}
