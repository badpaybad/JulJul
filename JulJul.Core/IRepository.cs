﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using JulJul.Core.Expressions;

namespace JulJul.Core
{
    public interface IRepository<T> where T : class, IEntity
    {
        IEnumerable<T> SelectAll();
        T Select(long id);
        T SelectSingle(ExpressionWhere<T> whereExpression);

        IEnumerable<T> SelectBy(ExpressionWhere<T> whereExpression);

        IEnumerable<T> SelectBy<TKey>(ExpressionWhere<T> whereExpression,
            ExpressionOrderBy<T,TKey> orderExpression,
            int skip, int take, out long total);
        
        IEnumerable<TView> SelectViewBy<TView>(ExpressionSelection<T, TView> selection,
            ExpressionWhere<T> whereExpression);

        IEnumerable<TView> SelectViewBy<TView, TKey>(ExpressionSelection<T, TView> selection,
          ExpressionOrderBy<T,TKey> orderExpression,
            ExpressionWhere<T> whereExpression,
            int skip, int take, out long total);

        IEnumerable<TView> Paging<TView, TKey>(ExpressionViewPaging<T, TView, TKey> expression, out long total); 
        IEnumerable<T> Paging<TKey>(ExpressionPaging<T, TKey> expression, out long total); 

        bool TryInsert(T entity);
        bool TryUpdate(T entity);
        bool TryDelete(T entity);
        bool TryDelete(long id);
    }
}