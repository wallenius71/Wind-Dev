using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WindART
{
    public class CreateXbyYShearSummmaryGrid:AbstractXbyYShearGridAlgorithm 
    {
        public CreateXbyYShearSummmaryGrid(IAxis x, IAxis y, double[,] gridvalues)
        {
            _valuegrid = gridvalues;
            _x = x;
            _y = y;
        }
    }
}
