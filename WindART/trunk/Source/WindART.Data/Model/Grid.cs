using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class Grid:IGrid 
    {
        //regular grid..simpler cousin of summarygrid
        ICreateGridAlgorithm  _algorithm;
        public Grid(ICreateGridAlgorithm  algorithm)
        {
            _algorithm = algorithm;
        }
      public List<List<SummaryGridColumn>> CreateGrid()
        {
            return _algorithm.CreateGrid();
        }

        
    }
}
