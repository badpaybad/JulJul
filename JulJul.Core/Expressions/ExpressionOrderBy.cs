using System;
using System.Linq.Expressions;

namespace JulJul.Core.Expressions
{
    public class ExpressionOrderBy<TSource,TKey>
    {
        public Expression<Func<TSource, TKey>> Expression { get; }

        public bool IsDesc { get; }
        public ExpressionOrderBy(Expression<Func<TSource,TKey>> orderby,   bool isDesc)
        {
            Expression = orderby;
            IsDesc = isDesc;
        }
    }
}