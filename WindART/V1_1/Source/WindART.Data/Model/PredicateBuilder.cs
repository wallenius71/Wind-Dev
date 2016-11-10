using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace WindART
{
    public static class PredicateBuilder
    {
        public static Expression<Func<T,bool>> True<T> () {return f=>true;}
        public static Expression<Func<T,bool>> False<T>() {return f=>false;}

        public static Expression<Func<T,bool>> and<T> (this Expression<Func<T,bool>> expr1, 
                                                                Expression<Func<T,bool>> expr2)
        {
            var invokedExpr = Expression.Invoke(expr2, expr1.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(expr1.Body, invokedExpr), expr1.Parameters);
        }



    }
}
