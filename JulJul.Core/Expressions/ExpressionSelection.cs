using System;
using System.Linq.Expressions;

namespace JulJul.Core.Expressions
{
    public class ExpressionSelection<TSource, TView> 
    {
        public Expression<Func<TSource, TView>> Expression { get; }

        public ExpressionSelection(Expression<Func<TSource, TView>> selection)
        {
            Expression = selection;
        } 
    }
}