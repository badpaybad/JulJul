using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using JulJul.Core.Expressions;

namespace JulJul.Core
{
    public abstract class EfAbstractRepository<T> : IRepository<T> where T : AbstractEntity, new()
    {
        public IEnumerable<T> SelectAll()
        {
            using (var db = new EfDbContext())
            {
                return db.Set<T>().ToList();
            }
        }

        public T Select(long id)
        {
            using (var db = new EfDbContext())
            {
                return db.Set<T>().FirstOrDefault(i => i.Id == id);
            }
        }

        public T SelectSingle(ExpressionWhere<T> whereExpression)
        {
            using (var db = new EfDbContext())
            {
                return db.Set<T>().Single(whereExpression.Expression);
            }
        }

        public IEnumerable<T> SelectBy(ExpressionWhere<T> whereExpression)
        {
            using (var db = new EfDbContext())
            {
                return db.Set<T>().Where(whereExpression.Expression).ToList();
            }
        }

        public IEnumerable<T> SelectBy<TKey>(ExpressionWhere<T> whereExpression,
            ExpressionOrderBy<T, TKey> orderExpression,
            int skip, int take, out long total)
        {
            total = 0;
            using (var db = new EfDbContext())
            {
                var dbSet = db.Set<T>();

                total = dbSet.Where(whereExpression.Expression).LongCount();

                if (orderExpression.IsDesc)
                    return dbSet.Where(whereExpression.Expression)
                        .OrderByDescending(orderExpression.Expression)
                        .Skip(skip).Take(take).ToList();

                return dbSet.Where(whereExpression.Expression)
                    .OrderBy(orderExpression.Expression)
                    .Skip(skip).Take(take).ToList();
            }
        }

        public IEnumerable<TView> SelectViewBy<TView>(ExpressionSelection<T, TView> selection,
            ExpressionWhere<T> whereExpression)
        {
            using (var db = new EfDbContext())
            {
                var queryable = db.Set<T>().Where(whereExpression.Expression)
                    .Select(selection.Expression);
                return queryable.ToList();
            }
        }

        public IEnumerable<TView> SelectViewBy<TView, TKey>(ExpressionSelection<T, TView> selection,
            ExpressionOrderBy<T, TKey> orderExpression,
            ExpressionWhere<T> whereExpression, int skip, int take, out long total)
        {
            total = 0;
            using (var db = new EfDbContext())
            {
                var dbSet = db.Set<T>();

                IQueryable<T> queryable = dbSet.Where(whereExpression.Expression);
                total = queryable.LongCount();

                if (orderExpression.IsDesc)
                {
                    var viewsDesc = queryable
                        .OrderByDescending(orderExpression.Expression)
                        .Skip(skip)
                        .Take(take)
                        .Select(selection.Expression);

                    return viewsDesc.ToList();
                }

                var views = queryable
                    .OrderBy(orderExpression.Expression)
                    .Skip(skip)
                    .Take(take)
                    .Select(selection.Expression);

                return views.ToList();
            }
        }

        public IEnumerable<TView> Paging<TView, TKey>(ExpressionViewPaging<T, TView, TKey> expression, out long total)
        {
            if (expression.Selection == null)
                throw new Exception("Selection must declare to identify TView will be return");
            using (var db = new EfDbContext())
            {
                var dbSet = db.Set<T>();
                IQueryable<T> query = dbSet;
                total = 0;

                if (expression.Where != null)
                {
                    total = query.Where(expression.Where).LongCount();
                    query = query.Where(expression.Where);
                }
                else
                {
                    total = query.LongCount();
                }

                if (expression.Order != null)
                {
                    if (expression.IsDesc)
                        query = query.OrderByDescending(expression.Order);
                    else query = query.OrderBy(expression.Order);

                    if (expression.Skip >= 0)
                    {
                        query = query.Skip(expression.Skip);
                    }
                }

                if (expression.Take > 0)
                {
                    query = query.Take(expression.Take);
                }

                IQueryable<TView> queryView = query.Select(expression.Selection);

                return queryView.ToList();
            }
        }

        public IEnumerable<T> Paging<TKey>(ExpressionPaging<T, TKey> expression, out long total)
        {
            using (var db = new EfDbContext())
            {
                var dbSet = db.Set<T>();
                IQueryable<T> query = dbSet;
                total = 0;

                if (expression.Where != null)
                {
                    total = query.Where(expression.Where).LongCount();
                    query = query.Where(expression.Where);
                }
                else
                {
                    total = query.LongCount();
                }

                if (expression.Order != null)
                {
                    if (expression.IsDesc)
                        query = query.OrderByDescending(expression.Order);
                    else query = query.OrderBy(expression.Order);

                    if (expression.Skip >= 0)
                    {
                        query = query.Skip(expression.Skip);
                    }
                }

                if (expression.Take > 0)
                {
                    query = query.Take(expression.Take);
                }

                return query.ToList();
            }
        }

        public bool TryInsert(T entity)
        {
            using (var db = new EfDbContext())
            {
                db.Set<T>().Add(entity);
                var affected = db.SaveChanges();
                return affected > 0;
            }
        }

        public bool TryUpdate(T entity)
        {
            using (var db = new EfDbContext())
            {
                var entry = db.Entry(entity);
                var dbSet = db.Set<T>();
                if (entry.State == EntityState.Detached)
                {
                    dbSet.Attach(entity);
                }

                db.Entry(entity).State = EntityState.Modified;

                var affected = db.SaveChanges();
                return affected > 0;
            }
        }

        public bool TryDelete(T entity)
        {
            return TryDelete(entity.Id);
        }

        public bool TryDelete(long id)
        {
            using (var db = new EfDbContext())
            {
                var dbSet = db.Set<T>();
                var temp = dbSet.FirstOrDefault(i => i.Id == id);
                if (temp == null) return true;
                dbSet.Remove(temp);
                var affected = db.SaveChanges();
                return affected > 0;
            }
        }
    }
}