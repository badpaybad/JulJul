using System;
using System.Linq.Expressions;

namespace JulJul.Core.Expressions
{
    public class ExpressionPaging<TSource, TKey>
    {
        public Expression<Func<TSource, TKey>> Order { get; }
        public Expression<Func<TSource, bool>> Where { get; }

        public int Skip { get; }
        public int Take { get; }
        public bool IsDesc { get; }

        public ExpressionPaging(Expression<Func<TSource, bool>> where = null,
            Expression<Func<TSource, TKey>> order = null, bool isDesc = false,
            int skip = 0, int take = 0)
        {
            
            Where = where;
            Order = order;
            Skip = skip;
            Take = take;
            IsDesc = isDesc;
        }
    }
}