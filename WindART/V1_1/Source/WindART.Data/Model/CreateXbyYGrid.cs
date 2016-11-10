using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public  class CreateXbyYGrid:ICreateGridAlgorithm 
    {
        protected IAxis _x;
        protected IAxis _y;
        protected double[,] _valuegrid;

        public CreateXbyYGrid(IAxis x, IAxis y, double[,] gridvalues)
        {
            _valuegrid = gridvalues;
            _x = x;
            _y = y;
        }
        public List<List<SummaryGridColumn>> CreateGrid()
        {
            List<List<SummaryGridColumn>> workgrid = new List<List<SummaryGridColumn>>(_y.AxisValues .Length );
            
            
                for (int y = 0; y < _y.AxisValues.Length+1; y++)
                {
                    //create a row and add it to the grid
                    List<SummaryGridColumn> row = new List<SummaryGridColumn>();

                    for (int x=0; x < _x.AxisValues.Length+1;x++)
                      {
                          if (y == 0)
                          {
                              if(x==0)
                              {
                                  row.Add(new SummaryGridColumn(new ExplicitValueGridColumn (null)));
                              }
                              else
                              {
                                  //top header row 
                                row.Add(new SummaryGridColumn(new MonthAxisValueGridColumn (x-1)));
                              }

                          }
                          else
                          {
                              if (x == 0)
                              {
                                  //left side header row
                                  row.Add(new SummaryGridColumn(new AxisValueGridColumn  (_y, y-1)));
                              }
                              else
                              {
                                  row.Add(new SummaryGridColumn(new ExplicitValueGridColumn(_valuegrid[x-1, y-1])));
                              }
                              
                          }
                        
                    }

                    
                    workgrid.Add(row);
                    }

                return workgrid;
            
        }
        
    }
}
