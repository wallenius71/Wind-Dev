using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class Create1byYGrid:ICreateGridAlgorithm 
    {
        protected IAxis _y;
        protected double[,] _valuegrid;
        
        public Create1byYGrid(IAxis y, double[,] gridvalues)
        {
            _valuegrid = gridvalues;
           _y = y;
        }
        public List<List<SummaryGridColumn>> CreateGrid()
        {
           List<List<SummaryGridColumn>> workgrid = new List<List<SummaryGridColumn>>(_y.AxisValues .Length );
            
            
                for (int y = 0; y < _y.AxisValues.Length+1; y++)
                {
                    //create a row and add it to the grid
                    List<SummaryGridColumn> row = new List<SummaryGridColumn>();

                    for (int x=0; x < 2;x++)
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
                                row.Add(new SummaryGridColumn(new ExplicitValueGridColumn ("1 by " + _y.AxisValues .Length + " Alpha")));
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

