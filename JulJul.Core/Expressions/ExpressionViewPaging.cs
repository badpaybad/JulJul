using System;
using System.Linq.Expressions;

namespace JulJul.Core.Expressions
{
    public class ExpressionViewPaging<TSource, TView, TKey>
    {
        public Expression<Func<TSource, TKey>> Order { get; }
        public Expression<Func<TSource, TView>> Selection { get; }
        public Expression<Func<TSource, bool>> Where { get; }

        public int Skip { get; }
        public int Take { get; }
        public bool IsDesc { get; }

        public ExpressionViewPaging(Expression<Func<TSource, TView>> selection = null,
            Expression<Func<TSource, bool>> where = null,
            Expression<Func<TSource, TKey>> order = null, bool isDesc = false,
            int skip = 0, int take = 0)
        {
            Selection = selection;
            Where = where;
            Order = order;
            Skip = skip;
            Take = take;
            IsDesc = isDesc;
        }
    }
}