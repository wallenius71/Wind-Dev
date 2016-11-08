using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions ;
using System.Text;
using System.Data;

namespace WindART
{
    public class WhereClauseFactory
    {
        public Expression<Func<DataRow,bool>> CreateWhereClause(IAxis axis,object Element,int Column)
        {
            AxisType type=axis.AxisType;

            //Console.WriteLine(type);
            switch (type)
            {
                case AxisType.WD:
                    {
                        int thisElement = (int)Element;
                        double xRangeStart = axis.GetRange(thisElement, Range.start);
                        double xRangeEnd = axis.GetRange(thisElement, Range.end);

                        // Console.WriteLine(thisElement  +  " start " + xRangeStart + " end " + xRangeEnd);

                        if (thisElement == 0)
                        {
                            Expression<Func<DataRow, bool>> result = c => (c.Field<double>(Column) >= xRangeStart && c.Field<double>(Column) <= 360)
                                | (c.Field<double>(Column) >= 0 && c.Field<double>(Column) < xRangeEnd);

                            return result;
                        }
                        else
                        {
                            Expression<Func<DataRow, bool>> result = c => (c.Field<double>(Column) >= xRangeStart && c.Field<double>(Column) < xRangeEnd);
                            return result;
                        }
                    }
                case AxisType.WS:
                    {
                        int thisElement = (int)Element;
                        double xRangeStart = axis.GetRange(thisElement, Range.start);
                        double xRangeEnd = axis.GetRange(thisElement, Range.end);

                       // Console.WriteLine(thisElement  +  " start " + xRangeStart + " end " + xRangeEnd);

                        
                        Expression<Func<DataRow, bool>> result = c => (c.Field<double>(Column) >= xRangeStart && c.Field<double>(Column) < xRangeEnd);
                        return result;
                         
                    }
                case AxisType.Month:
                    {
                        double xVal = axis.GetAxisValue(Element);
                        
                        Expression<Func<DataRow, bool>> result = c => c.Field<DateTime>(Column).Month == xVal;
                        return result;
                    }
                case AxisType.Hour:
                    {
                        
                        double xVal = axis.GetAxisValue(Element);
                        
                        Expression<Func<DataRow, bool>> result = c => c.Field<DateTime>(Column).Hour == xVal;
                        return result;
                    }

                default:
                    {
                        return null;
                    }
            }
        }
        public Expression<Func<DataRow, bool>> CreateWhereClause(DateTime start, DateTime end, int Column)
        {
            Expression<Func<DataRow, bool>> result = c => (c.Field<DateTime>(Column) >= start &&  c.Field<DateTime>(Column) <= end);
                                

            return result;

        }
    }
}
