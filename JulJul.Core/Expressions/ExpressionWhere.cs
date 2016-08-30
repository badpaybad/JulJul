using System;
using System.Linq.Expressions;

namespace JulJul.Core.Expressions
{
    public class ExpressionWhere<TSource>  
    {
        public Expression<Func<TSource, bool>> Expression { get; }

        public ExpressionWhere(Expression<Func<TSource, bool>> predicate)
        {

            Expression = predicate;
        }
    }
}