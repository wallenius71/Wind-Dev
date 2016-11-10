using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Linq.Expressions;
using System.Windows.Forms;
namespace WindART
{
    public class RowFilter:AbstractRowFilter 
    {
        public RowFilter (IAxis xAxis,IAxis yAxis, List<DataRow> data)
        {
            _xAxis=xAxis;
            _yAxis = yAxis;
            _data = data;
        }
        public RowFilter(List<DataRow> data)
        {
            _data = data;
        }
        public override List<double> Filter(object xAxisElement, object yAxisElement, int xCol, int yCol, int retCol)
        {
           //get axis value for each point, build where statement, return result set 
            //Console.WriteLine((DateTime)xElement + " " + (DateTime)yElement);
            
            
            WhereClauseFactory criteriaBuilder = new WhereClauseFactory();
            
            ParameterExpression param = Expression.Parameter(typeof(DataRow), "row");
            Expression full = default(Expression);

            Expression<Func<DataRow, bool>> cumulative = criteriaBuilder.CreateWhereClause(_xAxis, xAxisElement, xCol);
           
            full = Expression.Invoke(cumulative, param);



            Expression<Func<DataRow, bool>> exp = criteriaBuilder.CreateWhereClause(_yAxis, yAxisElement, yCol);

            full = Expression.AndAlso(full, Expression.Invoke(exp, param));

            Expression<Func<DataRow, bool>> f = Expression.Lambda<Func<DataRow, bool>>(full, param);
            
            Func<DataRow, bool> final = (Func<DataRow, bool>)f.Compile();

            try
            {
                //
                var result = _data.AsEnumerable().Where(c => final(c)&& c.Field<double>(retCol) >= 0 ).Select(c => c.Field<double>(retCol));

                List<double> res = result.ToList();

                //t = DateTime.Now - start;
                //Console.WriteLine("             run Linq: " + t);
                return res;
            }

            catch (InvalidCastException)
            {
               MessageBox.Show(" A Data value was not found in the correct format. Please make sure the input data file values are formatted as decimal values.");
               return null;
            }

           
        }
        public override List<double> Filter(object xAxisElement, object yAxisElement, int xCol, int yCol, int retCol,DateTime start, DateTime end,int dateCol)
        {
            //get axis value for each point, build where statement, return result set 
            //Console.WriteLine((DateTime)xElement + " " + (DateTime)yElement);


            WhereClauseFactory criteriaBuilder = new WhereClauseFactory();

            ParameterExpression param = Expression.Parameter(typeof(DataRow), "row");
            Expression full = default(Expression);

            Expression<Func<DataRow, bool>> cumulative = criteriaBuilder.CreateWhereClause(_xAxis, xAxisElement, xCol);

            full = Expression.Invoke(cumulative, param);

            Expression<Func<DataRow, bool>> exp = criteriaBuilder.CreateWhereClause(_yAxis, yAxisElement, yCol);

            full = Expression.AndAlso(full, Expression.Invoke(exp, param));

            Expression<Func<DataRow, bool>> exp1 = criteriaBuilder.CreateWhereClause(start,end, dateCol);

            full = Expression.AndAlso(full, Expression.Invoke(exp1, param));

            Expression<Func<DataRow, bool>> f = Expression.Lambda<Func<DataRow, bool>>(full, param);

            Func<DataRow, bool> final = (Func<DataRow, bool>)f.Compile();

            try
            {
            var result = _data.AsEnumerable().Where(c => final(c) && c.Field<double>(retCol) >= 0).Select(c => c.Field<double>(retCol));

            List<double> res = result.ToList();

            //t = DateTime.Now - start;
            //Console.WriteLine("             run Linq: " + t);
            return res;
            }

            catch (InvalidCastException)
            {
               MessageBox.Show(" A Data value was not found in the correct format. Please make sure the input data file values are formatted as decimal values.");
               return null;
            }

        }
        public override List<double> Filter(object xAxisElement,  int xCol,DateTime start, DateTime end, int dateCol,  int retCol)
        {
            //get axis value for each point, build where statement, return result set 
            //Console.WriteLine((DateTime)xElement + " " + (DateTime)yElement);


            WhereClauseFactory criteriaBuilder = new WhereClauseFactory();

            ParameterExpression param = Expression.Parameter(typeof(DataRow), "row");
            Expression full = default(Expression);

            Expression<Func<DataRow, bool>> cumulative = criteriaBuilder.CreateWhereClause(_xAxis, xAxisElement, xCol);

            full = Expression.Invoke(cumulative, param);

            Expression<Func<DataRow, bool>> exp1 = criteriaBuilder.CreateWhereClause(start, end, dateCol);

            full = Expression.AndAlso(full, Expression.Invoke(exp1, param));

            Expression<Func<DataRow, bool>> f = Expression.Lambda<Func<DataRow, bool>>(full, param);

            Func<DataRow, bool> final = (Func<DataRow, bool>)f.Compile();

            try
            {
            var result = _data.AsEnumerable().Where(c => final(c) && c.Field<double>(retCol) >= 0).Select(c => c.Field<double>(retCol));

            List<double> res = result.ToList();

            //t = DateTime.Now - start;
            //Console.WriteLine("             run Linq: " + t);
            return res;
            }

            catch (InvalidCastException)
            {
               MessageBox.Show(" A Data value was not found in the correct format. Please make sure the input data file values are formatted as decimal values.");
               return null;
            }

        }
        public override List<double> Filter(DateTime start, DateTime end, int dateCol, int retCol)
        {
            //get axis value for each point, build where statement, return result set 
            //Console.WriteLine((DateTime)xElement + " " + (DateTime)yElement);


            WhereClauseFactory criteriaBuilder = new WhereClauseFactory();

            ParameterExpression param = Expression.Parameter(typeof(DataRow), "row");
            Expression full = default(Expression);

            Expression<Func<DataRow, bool>> cumulative = criteriaBuilder.CreateWhereClause(start,end, dateCol);
            
            full = Expression.Invoke(cumulative, param);

           Expression<Func<DataRow, bool>> f = Expression.Lambda<Func<DataRow, bool>>(full, param);

            Func<DataRow, bool> final = (Func<DataRow, bool>)f.Compile();

            try
            {
            var result = _data.AsEnumerable().Where(c => final(c) && c.Field<double>(retCol) >= 0).Select(c => c.Field<double>(retCol));

            List<double> res = result.ToList();

            //t = DateTime.Now - start;
            //Console.WriteLine("             run Linq: " + t);
            return res;
            }

            catch (InvalidCastException)
            {
                MessageBox.Show(" A Data value was not found in the correct format. Please make sure the input data file values are formatted as decimal values.");
                return null;
            }


        }

    }
}
